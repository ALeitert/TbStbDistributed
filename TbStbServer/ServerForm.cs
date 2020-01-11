using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;

namespace TbStb.Server
{
    public partial class ServerForm : Form
    {
        private bool logActive = true;
        private delegate void LogDelegate(string text);
        private delegate void AddClientsDelegate(ClientBase[] clients);
        private delegate void CloseClientDelegate(int clientId);

        private CryptoServer server;
        private List<ClientBase> clientList = new List<ClientBase>();

        Graph g = null;
        const string graphDir = @".\graphs";
        BackgroundWorker graphLoader;

        public ServerForm()
        {
            InitializeComponent();

            PropertyInfo pi = typeof(ListView).GetProperty("DoubleBuffered", (BindingFlags)(-1));
            pi.SetValue(ltvGraphs, true, new object[] { });
            pi.SetValue(ltvClients, true, new object[] { });
            pi.SetValue(ltvLog, true, new object[] { });

            graphLoader = new BackgroundWorker();
            graphLoader.DoWork += graphLoader_DoWork;
            graphLoader.RunWorkerCompleted += graphLoader_RunWorkerCompleted;
        }

        private Color GetBackColor(int index)
        {
            Color oddColor = Color.White;
            Color evenColor = Color.FromArgb(228, 232, 237);

            if (index % 2 == 1)
            {
                return oddColor;
            }
            else
            {
                return evenColor;
            }
        }

        private void Log(string text)
        {
            if (!logActive) return;

            if (ltvLog.InvokeRequired)
            {
                ltvLog.Invoke(new LogDelegate(Log), new object[] { text });
            }
            else
            {
                ListViewItem lvi = new ListViewItem();
                lvi.BackColor = GetBackColor(ltvLog.Items.Count);

                lvi.Text = DateTime.Now.ToString("HH:mm:ss");
                lvi.SubItems.Add(text);

                ltvLog.Items.Add(lvi);
            }
        }

        private void LogAppend(string text)
        {
            if (!logActive) return;

            if (ltvLog.InvokeRequired)
            {
                ltvLog.Invoke(new LogDelegate(LogAppend), new object[] { text });
            }
            else
            {
                ListViewItem lastItem = ltvLog.Items[ltvLog.Items.Count - 1];
                lastItem.SubItems[1].Text += text;
            }
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            UpdateGraphList();
        }

        private void ServerForm_Shown(object sender, EventArgs e)
        {
            StartServer();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
            server?.Stop();
        }

        private void StartServer()
        {
            // ---- Determine IP address. ----

            IPAddress[] ipList = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress selectedIP = null;

            foreach (IPAddress ipa in ipList)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    selectedIP = ipa;
                    break;
                }
            }

            if (selectedIP == null)
            {
                selectedIP = IPAddress.Parse("127.0.0.1");
            }

            Log("Selected IP: " + selectedIP.ToString());


            // ---- Start server. ----

            Log("Starting Server ... ");
            server = new CryptoServer();

            try
            {
                int port = 4242; // ToDo: Port
                IPEndPoint ip = new IPEndPoint(selectedIP, port);

                server.Start(ip);

                server.ClientConnected += Server_ClientConnected;
                server.MessageFromClient += Server_MessageFromClient;
                server.ClientDisconnected += Server_ClientDisconnected;
                server.ListenerClosed += Server_ListenerClosed;

                LogAppend("Success.");
                Log("Listening: " + selectedIP + ":" + port);
            }
            catch (Exception ex)
            {
                LogAppend("Failed.");
                Log(ex.GetType().ToString());
                Log(ex.Message);
            }

        }

        private void Server_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            IPEndPoint ep = (IPEndPoint)e.Client.Socket.RemoteEndPoint;
            Log("Client connected (" + ep.Address.ToString() + ")");


            // Add client (and potentially missing ones) to list.
            ClientBase[] missingClients = new ClientBase[e.ClientId - clientList.Count + 1];

            for (int i = 0; i < missingClients.Length; i++)
            {
                missingClients[i] = server[i + clientList.Count];
            }

            AddClients(missingClients);
        }

        private void Server_MessageFromClient(object sender, MessageFromClientEventArgs e)
        {
        }

        private void Server_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            CloseClient(e.ClientId);
        }

        private void Server_ListenerClosed(object sender, EventArgs e)
        {
            Log("Listener closed.");
        }

        private void AddClients(ClientBase[] clients)
        {
            if (!logActive) return;

            if (ltvClients.InvokeRequired)
            {
                ltvClients.Invoke(new AddClientsDelegate(AddClients), new object[] { clients });
                return;
            }

            ltvClients.SuspendLayout();

            for (int i = 0; i < clients.Length; i++)
            {
                ClientBase client = clients[i];
                IPEndPoint ipe = (IPEndPoint)client.Socket.RemoteEndPoint;
                IPAddress address = ipe.Address;
                string name = Dns.GetHostEntry(address).HostName;

                ListViewItem lvi = new ListViewItem();
                lvi.BackColor = GetBackColor(ltvClients.Items.Count);

                lvi.Text = address.ToString();
                lvi.SubItems.AddRange(new string[]
                {
                    string.Empty,
                    name
                });

                ltvClients.Items.Add(lvi);
                clientList.Add(client);
            }

            ltvClients.ResumeLayout();
        }

        private void CloseClient(int clientId)
        {
            if (!logActive) return;

            if (ltvClients.InvokeRequired)
            {
                ltvClients.Invoke(new CloseClientDelegate(CloseClient), new object[] { clientId });
                return;
            }


            clientList[clientId] = null;

            ListViewItem lvi = ltvClients.Items[clientId];
            lvi.ForeColor = Color.Gray;
            lvi.SubItems[1].Text = "disconnected";

            Log("Client disconnected (" + lvi.Text + ").");
        }

        private void UpdateGraphList()
        {
            string fullDir = Path.GetFullPath(graphDir);
            if (!Directory.Exists(fullDir)) return;


            // Keep all previous entries.
            ListViewItem[] prevItems = new ListViewItem[ltvGraphs.Items.Count];
            ltvGraphs.Items.CopyTo(prevItems, 0);

            ltvGraphs.SuspendLayout();
            ltvGraphs.Items.Clear();

            // Load graphs.
            string[] allFiles = Directory.GetFiles(fullDir);

            for (int i = 0, j = 0; i < allFiles.Length; i++)
            {
                string fullPath = allFiles[i];
                string name = Path.GetFileName(fullPath);

                bool match = false;

                for (; j < prevItems.Length; j++)
                {
                    int comp = prevItems[j].Text.CompareTo(name);

                    if (comp < 0) continue;
                    if (comp == 0) match = true;
                    if (comp >= 0) break;
                }

                ListViewItem lvi = null;

                if (match)
                {
                    lvi = prevItems[j];
                }
                else
                {
                    StreamReader sr = new StreamReader(fullPath);
                    string firstLine = sr.ReadLine();
                    sr.Dispose();

                    int vSize = int.Parse(firstLine);

                    lvi = new ListViewItem();
                    lvi.Text = name;
                    lvi.SubItems.Add(vSize.ToString("#,##0"));
                }

                lvi.BackColor = GetBackColor(ltvGraphs.Items.Count);
                ltvGraphs.Items.Add(lvi);
            }

            ltvGraphs.ResumeLayout();
        }

        private void graphLoader_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void graphLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

    }
}
