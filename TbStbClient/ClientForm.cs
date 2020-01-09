using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TbStb.Client
{
    public partial class ClientForm : Form
    {
        private bool logActive = true;
        private delegate void LogDelegate(string text);

        public ClientForm()
        {
            InitializeComponent();
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
    }
}
