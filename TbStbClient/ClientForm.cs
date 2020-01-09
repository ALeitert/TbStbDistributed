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
            throw new NotImplementedException();
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
