using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TbStb
{
    public class ServerBase
    {
        private Socket listenerSocket;

        int clientCounter = 0;

        private Dictionary<ClientBase, int> clientTable;
        private Dictionary<int, ClientBase> clientFromId;

        public event MessageFromClientEventHandler MessageFromClient;
        public event ClientConnectedEventHandler ClientConnected;
        public event ClientDisconnectedEventHandler ClientDisconnected;
        public event EventHandler ListenerClosed;

        public ServerBase()
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientTable = new Dictionary<ClientBase, int>();
            clientFromId = new Dictionary<int, ClientBase>();
        }

        public void Start(IPEndPoint ipe)
        {
            listenerSocket.Bind(ipe);
            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        public void Stop()
        {
            listenerSocket?.Close();

            List<ClientBase> clientList = new List<ClientBase>(clientTable.Keys);
            foreach (ClientBase client in clientList)
            {
                client?.Close();
            }
        }

        public void SendMessage(int clientId, byte[] message)
        {
            if (!clientFromId.ContainsKey(clientId))
            {
                throw new ArgumentOutOfRangeException("clientId");
            }

            ClientBase client = clientFromId[clientId];
            client.SendMessage(message);
        }

        private void ListenThread()
        {
            while (true)
            {
                try
                {
                    listenerSocket.Listen(0);

                    ClientBase client = CreateClient(listenerSocket.Accept());

                    clientCounter++;
                    clientTable.Add(client, clientCounter);
                    clientFromId.Add(clientCounter, client);

                    client.MessageReceived += Client_MessageReceived;
                    client.ConnectionEnded += Client_ConnectionEnded;

                    ClientConnected?.Invoke(this, new ClientConnectedEventArgs(clientCounter));
                }
                catch (SocketException)
                {
                    ListenerClosed?.Invoke(this, new EventArgs());
                    return;
                }
            }
        }

        protected virtual ClientBase CreateClient(Socket socket)
        {
            return new ClientBase(socket);
        }

        private void Client_ConnectionEnded(object sender, EventArgs e)
        {
            ClientBase client = (ClientBase)sender;
            int clientId = clientTable[client];

            clientTable.Remove(client);
            clientFromId.Remove(clientId);

            ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(clientCounter));
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            ClientBase client = (ClientBase)sender;
            int clientId = clientTable[client];

            MessageFromClientEventArgs args = new MessageFromClientEventArgs(e.RawMessage, clientId);

            MessageFromClient?.Invoke(this, args);
        }
    }

    public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
    public delegate void ClientDisconnectedEventHandler(object sender, ClientDisconnectedEventArgs e);
    public delegate void MessageFromClientEventHandler(object sender, MessageFromClientEventArgs e);

    public class ClientConnectedEventArgs : EventArgs
    {
        public int ClientId { get; protected set; }

        public ClientConnectedEventArgs(int clientId)
        {
            ClientId = clientId;
        }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public int ClientId { get; protected set; }

        public ClientDisconnectedEventArgs(int clientId)
        {
            ClientId = clientId;
        }
    }

    public class MessageFromClientEventArgs : EventArgs
    {
        public byte[] RawMessage { get; protected set; }
        public int ClientId { get; protected set; }

        public MessageFromClientEventArgs(byte[] message, int clientId)
        {
            RawMessage = message;
            ClientId = clientId;
        }
    }
}
