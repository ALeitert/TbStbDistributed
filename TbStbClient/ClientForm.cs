using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        private Graph g = null;

        private Process partnerProcess = null;
        const string partnerExe = "PotentialPartners.exe";

        const string graphDir = @".\graphs";


        public ClientForm()
        {
            InitializeComponent();

            bgw = new BackgroundWorker();
            bgw.DoWork += bgw_DoWork;
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

                while (lstLog.Items.Count > 25)
                {
                    lstLog.Items.RemoveAt(0);
                }
            }
        }

        private void LogAppend(string text)
        {
            if (!logActive) return;

            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new LogDelegate(LogAppend), new object[] { text });
            }
            else
            {
                string lastItem = (string)lstLog.Items[lstLog.Items.Count - 1];
                lstLog.Items[lstLog.Items.Count - 1] = lastItem + text;
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
            client?.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Log("Connecting to server ... ");

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
                LogAppend("Success.");
                btnConnect.Enabled = false;
            }
            else
            {
                LogAppend("Failed.");
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
            btnConnect.Enabled = true;
        }

        private void ProcessMessage(string msg)
        {
            // Ensure method is called in main thread.
            if (this.InvokeRequired)
            {
                this.Invoke(new ProcessMessageDelegate(ProcessMessage), new object[] { msg });
                return;
            }

            // Give message to background worker.
            bgw.RunWorkerAsync(msg);
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            // Message format: "graphName|task|vId|rho"
            //   graphName: The graph to perform computations on.
            //        task: The task to do. Either "partner" to determine potential partners,
            //              or "clDist" to determine the distance to the clusters of a layering partition.
            //         vId: The vertex to perform the task on.
            //         rho: The radius when checking potential partners. Not given for task "clDist".


            string msg = (string)e.Argument;
            string[] msgParts = msg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (msgParts == null || msgParts.Length < 3)
            {
                Log("Invalid message from server: " + msg);
                return;
            }

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
                    result = FindMaxDistanceToClusters(graph, vId);
                    break;

                default:
                    Log("Invalid message from server: " + msg);
                    return;
            }


            // ---- Compile answer to server. ----

            byte[] answerPt1 = Encoding.UTF8.GetBytes(msg);
            byte[] answerPt2 = result;

            // +4 for integer stating the number of bytes in first part of the answer.
            int fullAnswerSize = 4 + answerPt1.Length + answerPt2.Length;
            byte[] fullAnswer = new byte[fullAnswerSize];

            byte[] sizePt1 = BitConverter.GetBytes(answerPt1.Length);

            int ansInd = 0;

            foreach (byte[] arr in new byte[][] { sizePt1, answerPt1, answerPt2 })
            {
                Array.Copy(arr, 0, fullAnswer, ansInd, arr.Length);
                ansInd += arr.Length;
            }

            // Free some memory directly.
            sizePt1 = null;
            answerPt1 = null;
            answerPt2 = null;
            result = null;
            GC.Collect();

            Log(string.Format("Sending message to server ({0:#,##0} bytes).", fullAnswerSize));
            client.SendMessage(fullAnswer);
        }

        private void LoadGraph(string name)
        {
            if (g != null && g.Name == name)
            {
                // Graph still loaded; do nothing.
                return;
            }

            string fullDir = Path.GetFullPath(graphDir);
            string fullPath = Path.Combine(fullDir, name);

            Log("Loading graph \"" + name + "\"");
            int startTime = Environment.TickCount;

            if (partnerProcess != null && !partnerProcess.HasExited)
            {
                partnerProcess.Kill();
            }
            partnerProcess = null;

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                g = new Graph(fs);
                g.Name = name;
            }

            LogAppend(string.Format(" ({0:#,##0} ms)", Environment.TickCount - startTime));
        }

        private class Base64Reader
        {
            private StreamReader reader = null;
            private List<byte> buffer = new List<byte>();

            private int buffPtr = 0;
            private bool bufferAll = true;

            public Base64Reader(StreamReader sr)
            {
                reader = sr;
            }

            public void StopBuffering()
            {
                bufferAll = false;

                // Create new buffer to ensure the internal array is not larger than needed.
                List<byte> newBuffer = new List<byte>();

                for (int i = buffPtr; i < buffer.Count; i++)
                {
                    newBuffer.Add(buffer[i]);
                }

                buffPtr = 0;
                buffer = newBuffer;
            }

            public int ReadInt32()
            {
                if (buffPtr >= buffer.Count)
                {
                    if (!bufferAll)
                    {
                        buffer.Clear();
                        buffPtr = 0;
                    }

                    string line = reader.ReadLine();
                    buffer.AddRange(Convert.FromBase64String(line));
                }

                byte[] data = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    data[i] = buffer[buffPtr];
                    buffPtr++;
                }

                return BitConverter.ToInt32(data, 0);
            }

            public byte[] GetBuffer()
            {
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Calls the PotentialPartner program to compute the potential partners of the given vertex.
        /// </summary>
        /// <returns>
        /// The message to the server as byte-array.
        /// </returns>
        private byte[] FindPotentialPartners(string graph, int vId, int rho)
        {
            LoadGraph(graph);

            if (partnerProcess == null)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.CreateNoWindow = true;
                psi.FileName = partnerExe;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;

                Log("Starting " + psi.FileName + ".");

                partnerProcess = Process.Start(psi);
                g.Print(partnerProcess.StandardInput);
            }

            Log(string.Format("Computing potential partners with vId = {0} and rho = {1} ... ", vId, rho));
            int startTime = Environment.TickCount;

            partnerProcess.StandardInput.WriteLine(string.Format("{0} {1}", vId, rho));
            Base64Reader reader = new Base64Reader(partnerProcess.StandardOutput);

            bool isValid = true;
            int ccs = reader.ReadInt32();

            for (int i = 0; i < ccs; i++)
            {
                int partners = reader.ReadInt32();

                if (isValid && partners == 0)
                {
                    isValid = false;
                    reader.StopBuffering();
                }

                for (int j = 0; j < partners; j++)
                {
                    int id = reader.ReadInt32();
                }
            }

            LogAppend(string.Format("Done ({0:#,##0} ms).", Environment.TickCount - startTime));

            if (isValid)
            {
                return reader.GetBuffer();
            }
            else
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// Determines the "distance" of the given vertex to all clusters in a layering partition.
        /// </summary>
        /// <returns>
        /// The message to the server as byte-array.
        /// </returns>
        private byte[] FindMaxDistanceToClusters(string graph, int vId)
        {
            LoadGraph(graph);

            int[][] layPart = g.LayeringPartition;
            byte[] result = new byte[layPart.Length * 4];

            Log(string.Format("Computing maximum distance to clusters with vId = {0} ... ", vId));
            int startTime = Environment.TickCount;

            BfsResult bfs = g.Bfs(vId);

            for (int i = 0; i < layPart.Length; i++)
            {
                int dis = -1;

                foreach (int uId in layPart[i])
                {
                    dis = Math.Max(bfs.Distances[uId], dis);
                }

                // Convert distance to byte[].
                byte[] num = BitConverter.GetBytes(dis);

                for (int j = 0; j < num.Length; j++)
                {
                    result[4 * i + j] = num[j];
                }
            }

            LogAppend(string.Format("Done ({0:#,##0} ms).", Environment.TickCount - startTime));
            return result;
        }

    }
}
