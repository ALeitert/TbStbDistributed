using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TbStb.Client
{
    public partial class ClientForm : Form
    {
        CryptoClient client;

        private bool logActive = true;
        private delegate void LogDelegate(string text);

        private BackgroundWorker bgw;
        private delegate void ProcessMessageDelegate(string msg);

        public ClientForm()
        {
            InitializeComponent();

            bgw = new BackgroundWorker();
            bgw.DoWork += bgw_DoWork;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            bgw.WorkerSupportsCancellation = true;
        }

        private void Log(string text)
        {
            if (!logActive) return;

            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new LogDelegate(Log), new object[] { text });
            }
            else
            {
                string logTxt = string.Format("{0:HH:mm:ss}  {1}", DateTime.Now, text);
                lstLog.Items.Add(logTxt);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }

            client = new CryptoClient();
            client.MessageReceived += Socket_MessageReceived;
            client.ConnectionEnded += Socket_ConnectionEnded;

            bool connected = client.Connect(new IPEndPoint(IPAddress.Parse(txtIP.Text), 4242));
            if (connected)
            {
                Log("Connected to server.");
            }
            else
            {
                Log("Could not connect to server.");
            }
        }

        private void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.RawMessage);
            ProcessMessage(msg);
        }

        private void Socket_ConnectionEnded(object sender, EventArgs e)
        {
            Log("Server Disconnected.");
        }

        private void ProcessMessage(string msg)
        {
            // Ensure method is called in main thread.
            if (this.InvokeRequired)
            {
                this.Invoke(new ProcessMessageDelegate(ProcessMessage), new object[] { msg });
                return;
            }

            // Message format: "graphName|task|vId|rho"
            //   graphName: The graph to perform computations on.
            //        task: The task to do. Either "partner" to determine potential partners,
            //              or "clDist" to determine the distance to the clusters of a layering partition.
            //         vId: The vertex to perform the task on.
            //         rho: The radius when checking potential partners. Not given for task "clDist".


            string[] msgParts = msg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (msgParts == null || msgParts.Length < 3)
            {
                Log("Invalid message from server: " + msg);
            }
            else
            {
                Log("Server: " + msg);
            }

            // Give message to background worker.
            bgw.RunWorkerAsync(msgParts);
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] msgParts = (string[])e.Argument;

            string graph = msgParts[0];
            string task = msgParts[1];
            int vId = int.Parse(msgParts[2]);

            byte[] result = null;

            switch (msgParts[1])
            {
                case "partner":
                    int rho = int.Parse(msgParts[3]);
                    result = FindPotentialPartners(graph, vId, rho);
                    break;

                case "clDist":
                    result = FindDistanceToClusters(graph, vId);
                    break;

                default:
                    // Invalid message; do nothing.
                    break;
            }

            e.Result = result;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calls the PotentialPartner program to compute the potential partners of the given vertex.
        /// </summary>
        /// <returns>
        /// The message to the server as byte-array.
        /// </returns>
        private byte[] FindPotentialPartners(string graph, int vId, int rho)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the "distance" of the given vertex to all clusters in a layering partition.
        /// </summary>
        /// <returns>
        /// The message to the server as byte-array.
        /// </returns>
        private byte[] FindDistanceToClusters(string graph, int vId)
        {
            throw new NotImplementedException();
        }
    }
}
