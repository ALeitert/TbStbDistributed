namespace TbStb.Server
{
    partial class ServerForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabGraphs = new System.Windows.Forms.TabPage();
            this.ltvGraphs = new System.Windows.Forms.ListView();
            this.clhGraphName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhGraphVSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhGraphESize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhGraphCC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhTb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhStb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuGraphs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mniGraphsLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGraphsLine1 = new System.Windows.Forms.ToolStripSeparator();
            this.mniGraphsCompTb = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGraphsCompStb = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGraphsLine2 = new System.Windows.Forms.ToolStripSeparator();
            this.mniGraphsRepair = new System.Windows.Forms.ToolStripMenuItem();
            this.tabClients = new System.Windows.Forms.TabPage();
            this.ltvClients = new System.Windows.Forms.ListView();
            this.clhClientIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhClientStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhClientInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabLog = new System.Windows.Forms.TabPage();
            this.ltvLog = new System.Windows.Forms.ListView();
            this.clhTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabMain.SuspendLayout();
            this.tabGraphs.SuspendLayout();
            this.mnuGraphs.SuspendLayout();
            this.tabClients.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabGraphs);
            this.tabMain.Controls.Add(this.tabClients);
            this.tabMain.Controls.Add(this.tabLog);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(604, 448);
            this.tabMain.TabIndex = 0;
            // 
            // tabGraphs
            // 
            this.tabGraphs.Controls.Add(this.ltvGraphs);
            this.tabGraphs.Location = new System.Drawing.Point(4, 22);
            this.tabGraphs.Name = "tabGraphs";
            this.tabGraphs.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraphs.Size = new System.Drawing.Size(596, 422);
            this.tabGraphs.TabIndex = 0;
            this.tabGraphs.Text = "Graphs";
            this.tabGraphs.UseVisualStyleBackColor = true;
            // 
            // ltvGraphs
            // 
            this.ltvGraphs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhGraphName,
            this.clhGraphVSize,
            this.clhGraphESize,
            this.clhGraphCC,
            this.clhTb,
            this.clhStb});
            this.ltvGraphs.ContextMenuStrip = this.mnuGraphs;
            this.ltvGraphs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ltvGraphs.FullRowSelect = true;
            this.ltvGraphs.GridLines = true;
            this.ltvGraphs.Location = new System.Drawing.Point(3, 3);
            this.ltvGraphs.MultiSelect = false;
            this.ltvGraphs.Name = "ltvGraphs";
            this.ltvGraphs.Size = new System.Drawing.Size(590, 416);
            this.ltvGraphs.TabIndex = 1;
            this.ltvGraphs.UseCompatibleStateImageBehavior = false;
            this.ltvGraphs.View = System.Windows.Forms.View.Details;
            this.ltvGraphs.SelectedIndexChanged += new System.EventHandler(this.ltvGraphs_SelectedIndexChanged);
            this.ltvGraphs.DoubleClick += new System.EventHandler(this.ltvGraphs_DoubleClick);
            // 
            // clhGraphName
            // 
            this.clhGraphName.Text = "Name";
            this.clhGraphName.Width = 100;
            // 
            // clhGraphVSize
            // 
            this.clhGraphVSize.Text = "Vertices";
            this.clhGraphVSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.clhGraphVSize.Width = 80;
            // 
            // clhGraphESize
            // 
            this.clhGraphESize.Text = "Edges";
            this.clhGraphESize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.clhGraphESize.Width = 80;
            // 
            // clhGraphCC
            // 
            this.clhGraphCC.Text = "Largest CC";
            this.clhGraphCC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.clhGraphCC.Width = 80;
            // 
            // clhTb
            // 
            this.clhTb.Text = "tb(G)";
            this.clhTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // clhStb
            // 
            this.clhStb.Text = "stb(G)";
            this.clhStb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mnuGraphs
            // 
            this.mnuGraphs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniGraphsLoad,
            this.mniGraphsLine1,
            this.mniGraphsCompTb,
            this.mniGraphsCompStb,
            this.mniGraphsLine2,
            this.mniGraphsRepair});
            this.mnuGraphs.Name = "mnuGraphs";
            this.mnuGraphs.Size = new System.Drawing.Size(234, 126);
            // 
            // mniGraphsLoad
            // 
            this.mniGraphsLoad.Name = "mniGraphsLoad";
            this.mniGraphsLoad.Size = new System.Drawing.Size(233, 22);
            this.mniGraphsLoad.Text = "Load";
            // 
            // mniGraphsLine1
            // 
            this.mniGraphsLine1.Name = "mniGraphsLine1";
            this.mniGraphsLine1.Size = new System.Drawing.Size(230, 6);
            // 
            // mniGraphsCompTb
            // 
            this.mniGraphsCompTb.Name = "mniGraphsCompTb";
            this.mniGraphsCompTb.Size = new System.Drawing.Size(233, 22);
            this.mniGraphsCompTb.Text = "Compute Tree-Breadth";
            this.mniGraphsCompTb.Click += new System.EventHandler(this.mniGraphsCompTb_Click);
            // 
            // mniGraphsCompStb
            // 
            this.mniGraphsCompStb.Name = "mniGraphsCompStb";
            this.mniGraphsCompStb.Size = new System.Drawing.Size(233, 22);
            this.mniGraphsCompStb.Text = "Compute Strong Tree-Breadth";
            this.mniGraphsCompStb.Click += new System.EventHandler(this.mniGraphsCompStb_Click);
            // 
            // mniGraphsLine2
            // 
            this.mniGraphsLine2.Name = "mniGraphsLine2";
            this.mniGraphsLine2.Size = new System.Drawing.Size(230, 6);
            // 
            // mniGraphsRepair
            // 
            this.mniGraphsRepair.Name = "mniGraphsRepair";
            this.mniGraphsRepair.Size = new System.Drawing.Size(233, 22);
            this.mniGraphsRepair.Text = "Repair";
            this.mniGraphsRepair.Click += new System.EventHandler(this.mniGraphsRepair_Click);
            // 
            // tabClients
            // 
            this.tabClients.Controls.Add(this.ltvClients);
            this.tabClients.Location = new System.Drawing.Point(4, 22);
            this.tabClients.Name = "tabClients";
            this.tabClients.Padding = new System.Windows.Forms.Padding(3);
            this.tabClients.Size = new System.Drawing.Size(596, 422);
            this.tabClients.TabIndex = 1;
            this.tabClients.Text = "Clients";
            this.tabClients.UseVisualStyleBackColor = true;
            // 
            // ltvClients
            // 
            this.ltvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhClientIP,
            this.clhClientStatus,
            this.clhClientInfo});
            this.ltvClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ltvClients.FullRowSelect = true;
            this.ltvClients.GridLines = true;
            this.ltvClients.Location = new System.Drawing.Point(3, 3);
            this.ltvClients.MultiSelect = false;
            this.ltvClients.Name = "ltvClients";
            this.ltvClients.Size = new System.Drawing.Size(590, 416);
            this.ltvClients.TabIndex = 0;
            this.ltvClients.UseCompatibleStateImageBehavior = false;
            this.ltvClients.View = System.Windows.Forms.View.Details;
            // 
            // clhClientIP
            // 
            this.clhClientIP.Text = "IP";
            this.clhClientIP.Width = 100;
            // 
            // clhClientStatus
            // 
            this.clhClientStatus.Text = "Status";
            this.clhClientStatus.Width = 120;
            // 
            // clhClientInfo
            // 
            this.clhClientInfo.Text = "Info";
            this.clhClientInfo.Width = 300;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.ltvLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(596, 422);
            this.tabLog.TabIndex = 2;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // ltvLog
            // 
            this.ltvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhTime,
            this.clhMessage});
            this.ltvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ltvLog.FullRowSelect = true;
            this.ltvLog.GridLines = true;
            this.ltvLog.Location = new System.Drawing.Point(3, 3);
            this.ltvLog.MultiSelect = false;
            this.ltvLog.Name = "ltvLog";
            this.ltvLog.Size = new System.Drawing.Size(590, 416);
            this.ltvLog.TabIndex = 0;
            this.ltvLog.UseCompatibleStateImageBehavior = false;
            this.ltvLog.View = System.Windows.Forms.View.Details;
            // 
            // clhTime
            // 
            this.clhTime.Text = "Time";
            // 
            // clhMessage
            // 
            this.clhMessage.Text = "Message";
            this.clhMessage.Width = 500;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 448);
            this.Controls.Add(this.tabMain);
            this.Name = "ServerForm";
            this.Text = "Tree-Breadth vs Strong Tree-Breadth (Server)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.Shown += new System.EventHandler(this.ServerForm_Shown);
            this.tabMain.ResumeLayout(false);
            this.tabGraphs.ResumeLayout(false);
            this.mnuGraphs.ResumeLayout(false);
            this.tabClients.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabGraphs;
        private System.Windows.Forms.TabPage tabClients;
        private System.Windows.Forms.ListView ltvClients;
        private System.Windows.Forms.ColumnHeader clhClientIP;
        private System.Windows.Forms.ColumnHeader clhClientStatus;
        private System.Windows.Forms.ColumnHeader clhClientInfo;
        private System.Windows.Forms.ListView ltvGraphs;
        private System.Windows.Forms.ColumnHeader clhGraphName;
        private System.Windows.Forms.ColumnHeader clhGraphVSize;
        private System.Windows.Forms.ColumnHeader clhGraphESize;
        private System.Windows.Forms.ColumnHeader clhGraphCC;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.ColumnHeader clhTb;
        private System.Windows.Forms.ColumnHeader clhStb;
        private System.Windows.Forms.ListView ltvLog;
        private System.Windows.Forms.ColumnHeader clhTime;
        private System.Windows.Forms.ColumnHeader clhMessage;
        private System.Windows.Forms.ContextMenuStrip mnuGraphs;
        private System.Windows.Forms.ToolStripMenuItem mniGraphsLoad;
        private System.Windows.Forms.ToolStripSeparator mniGraphsLine1;
        private System.Windows.Forms.ToolStripMenuItem mniGraphsCompTb;
        private System.Windows.Forms.ToolStripMenuItem mniGraphsCompStb;
        private System.Windows.Forms.ToolStripSeparator mniGraphsLine2;
        private System.Windows.Forms.ToolStripMenuItem mniGraphsRepair;
    }
}

