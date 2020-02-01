using System;
using System.Collections.Generic;
using System.Threading;

namespace TbStb.Server
{
    enum AlgorithmState
    {
        Unknown,
        Initialised,
        Running,
        Done
    }

    abstract class Algorithm
    {
        private int result = -1;

        private List<ClientBase> allClients = new List<ClientBase>();
        private List<int> vertAssigned = new List<int>();

        private Thread msgProcThread = null;
        private object msgProcLock = new object();
        private Queue<int> msgIdQ = new Queue<int>();
        private Queue<byte[]> msgQ = new Queue<byte[]>();


        private Thread assignTasksThread = null;
        private object assignTasksLock = new object();
        private Queue<int> freeClientsQ = new Queue<int>();
        private int[] vertexQ = null;
        private int vertexQPtr = -1;
        private Queue<int> redoIdsQ = new Queue<int>();
        private int currentlyProcessing = 0;


        public event EventHandler AlgorithmCompleted;



        public Algorithm()
        {
            State = AlgorithmState.Initialised;
        }

        public Algorithm(IEnumerable<ClientBase> clients) : this()
        {
            IEnumerator<ClientBase> clEnum = clients.GetEnumerator();

            while (clEnum.MoveNext())
            {
                ClientBase current = clEnum.Current;
                allClients.Add(current);
                vertAssigned.Add(-1);
            }
        }


        public AlgorithmState State { get; protected set; }

        public int Result
        {
            get
            {
                if (State != AlgorithmState.Done)
                {
                    throw new InvalidOperationException();
                }

                return result;
            }
            protected set
            {
                result = value;
            }
        }

        public string GraphName { get; private set; }
        public int GraphSize { get; private set; }


        public void Start(Graph g)
        {
            if (State != AlgorithmState.Initialised)
            {
                throw new InvalidOperationException();
            }

            State = AlgorithmState.Running;
            GraphName = g.Name;
            GraphSize = g.Vertices;

            Thread preT = new Thread(Preprocess);
            preT.Start(g);
        }

        private void Preprocess(object graph)
        {
            Graph g = (Graph)graph;
            int[] vertices = g.FindLargestCC();

            graph = null;
            g = null;
            GC.Collect();

            OnPreprocess();

            lock (assignTasksLock) // Just in case.
            {
                for (int i = 0; i < allClients.Count; i++)
                {
                    if (allClients[i] == null) continue;
                    freeClientsQ.Enqueue(i);
                }

                vertexQ = vertices;
                vertexQPtr = 0;

                vertices = null;

                assignTasksThread = new Thread(AssignTasks);
                assignTasksThread.Start();
            }
        }

        protected virtual void OnPreprocess() { }

        protected void Restart()
        {
            OnRestart();

            lock (assignTasksLock)
            {
                vertexQPtr = 0;

                assignTasksThread = new Thread(AssignTasks);
                assignTasksThread.Start();
            }
        }

        protected abstract void OnRestart();

        protected abstract bool Conclude();


        public void AddClient(ClientBase client)
        {
            if (client == null) return;

            allClients.Add(client);
            vertAssigned.Add(-1);

            if (State != AlgorithmState.Running) return;

            lock (assignTasksLock)
            {
                freeClientsQ.Enqueue(allClients.Count - 1);

                if (assignTasksThread == null)
                {
                    assignTasksThread = new Thread(AssignTasks);
                    assignTasksThread.Start();
                }
            }
        }

        public void RemoveClient(int clientId)
        {
            if (clientId >= allClients.Count || allClients[clientId] == null) return;

            ClientBase client = allClients[clientId];
            int vId = vertAssigned[clientId];

            lock (client)
            {
                allClients[clientId] = null;
                vertAssigned[clientId] = -1;
            }

            if (vId >= 0 && State == AlgorithmState.Running)
            {
                lock (assignTasksLock)
                {
                    currentlyProcessing--;
                    redoIdsQ.Enqueue(vId);

                    if (assignTasksThread == null)
                    {
                        assignTasksThread = new Thread(AssignTasks);
                        assignTasksThread.Start();
                    }
                }
            }
        }

        private void AssignTasks()
        {
            while (true)
            {
                ClientBase client;
                int clientId;
                int vId;

                lock (assignTasksLock)
                {
                    if
                    (
                        freeClientsQ.Count == 0 ||
                        (redoIdsQ.Count == 0 && vertexQPtr >= vertexQ.Length)
                    )
                    {
                        assignTasksThread = null;
                        return;
                    }


                    clientId = freeClientsQ.Dequeue();
                    client = allClients[clientId];

                    if (client == null) continue;


                    if (redoIdsQ.Count > 0)
                    {
                        vId = redoIdsQ.Dequeue();
                    }
                    else
                    {
                        vId = vertexQ[vertexQPtr];
                        vertexQPtr++;
                    }

                    currentlyProcessing++;
                }

                lock (client)
                {
                    byte[] task = GetNextTask(vId);
                    if (task == null || task.Length == 0) continue;

                    client.SendMessage(task);
                    vertAssigned[clientId] = vId;
                }
            }
        }

        protected abstract byte[] GetNextTask(int vId);


        public void NewClientMessage(int clientId, byte[] msg)
        {
            int vId;
            ClientBase client = allClients[clientId];

            // "Free" client.
            lock (client)
            {
                vId = vertAssigned[clientId];
                vertAssigned[clientId] = -1;
            }

            // Assign new task to client.
            lock (assignTasksLock)
            {
                currentlyProcessing--;
                freeClientsQ.Enqueue(clientId);

                if (assignTasksThread == null)
                {
                    assignTasksThread = new Thread(AssignTasks);
                    assignTasksThread.Start();
                }
            }

            // Process message.
            lock (msgProcLock)
            {
                msgIdQ.Enqueue(vId);
                msgQ.Enqueue(msg);

                if (msgProcThread == null)
                {
                    msgProcThread = new Thread(ProcessMessages);
                    msgProcThread.Start();
                }
            }
        }

        private void ProcessMessages()
        {
            while (true)
            {
                int vId;
                byte[] msg;

                lock (msgProcLock)
                {
                    if (msgQ.Count == 0)
                    {
                        msgProcThread = null;
                        break;
                    }

                    vId = msgIdQ.Dequeue();
                    msg = msgQ.Dequeue();
                }

                OnProcessMessages(vId, msg);
            }

            lock (assignTasksLock)
            {
                int totalToProcess =
                    currentlyProcessing +
                    redoIdsQ.Count +
                    vertexQ.Length - vertexQPtr;


                if (totalToProcess > 0) return;
            }

            // Done processing.
            if (Conclude())
            {
                State = AlgorithmState.Done;
                AlgorithmCompleted?.Invoke(this, new EventArgs());
            }
            else
            {
                Restart();
            }

        }

        protected abstract void OnProcessMessages(int vId, byte[] msg);



    }
}
