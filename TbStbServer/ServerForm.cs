using System;
using System.Drawing;
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

                lvi.BackColor = ltvLog.Items.Count % 2 == 0 ? DefaultItemBackColor : ShadedItemBackColor;

                ltvLog.Items.Add(lvi);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
        }
    }
}
