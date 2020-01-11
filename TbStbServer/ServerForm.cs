using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;

namespace TbStb.Server
{
    public partial class ServerForm : Form
    {
        private Color DefaultItemBackColor = Color.White;
        private Color ShadedItemBackColor = Color.FromArgb(228, 232, 237);

        private bool logActive = true;
        private delegate void LogDelegate(string text);
        private delegate void AddClientsDelegate(ClientBase[] clients);
        private delegate void CloseClientDelegate(int clientId);

        private CryptoServer server;
        private List<ClientBase> clientList = new List<ClientBase>();

        public ServerForm()
        {
            InitializeComponent();

            PropertyInfo pi = typeof(ListView).GetProperty("DoubleBuffered", (BindingFlags)(-1));
            pi.SetValue(ltvGraphs, true, new object[] { });
            pi.SetValue(ltvClients, true, new object[] { });
            pi.SetValue(ltvLog, true, new object[] { });
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
                lvi.Text = DateTime.Now.ToString("HH:mm:ss");
                lvi.SubItems.Add(text);

                lvi.BackColor = ltvLog.Items.Count % 2 == 1 ? DefaultItemBackColor : ShadedItemBackColor;

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
                lvi.BackColor = ltvClients.Items.Count % 2 == 1 ? DefaultItemBackColor : ShadedItemBackColor;

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
    }
}
