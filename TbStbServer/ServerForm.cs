using System;
using System.Windows.Forms;

namespace TbStb.Server
{
    public partial class ServerForm : Form
    {
        private bool logActive = true;
        private delegate void LogDelegate(string text);

        public ServerForm()
        {
            InitializeComponent();
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

                ltvLog.Items.Add(lvi);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
        }
    }
}
