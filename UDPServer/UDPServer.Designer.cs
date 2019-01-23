namespace UDPServer
{
    partial class UDPServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fmFile = new System.Windows.Forms.ToolStripMenuItem();
            this.fmHome = new System.Windows.Forms.ToolStripMenuItem();
            this.fmExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ipServices = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartServer = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartUDPServer = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartResponder = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartTCP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipTCPDiscon = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStopTCP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStopUDPServer = new System.Windows.Forms.ToolStripMenuItem();
            this.vmView = new System.Windows.Forms.ToolStripMenuItem();
            this.vmIPInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.hmHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.hmAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tStatus = new System.Windows.Forms.TextBox();
            this.tInfo = new System.Windows.Forms.TextBox();
            this.lStatus = new System.Windows.Forms.Label();
            this.cIPAddress = new System.Windows.Forms.ComboBox();
            this.tPort = new System.Windows.Forms.TextBox();
            this.lIPAddress = new System.Windows.Forms.Label();
            this.lPort = new System.Windows.Forms.Label();
            this.tAvailIPs = new System.Windows.Forms.TextBox();
            this.tClients = new System.Windows.Forms.TextBox();
            this.lAvailIPs = new System.Windows.Forms.Label();
            this.lClients = new System.Windows.Forms.Label();
            this.tUDPMonitor = new System.Windows.Forms.TextBox();
            this.tTCPMonitor = new System.Windows.Forms.TextBox();
            this.ipStopAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fmFile,
            this.ipServices,
            this.vmView,
            this.hmHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(605, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fmFile
            // 
            this.fmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fmHome,
            this.fmExit});
            this.fmFile.Name = "fmFile";
            this.fmFile.Size = new System.Drawing.Size(37, 20);
            this.fmFile.Text = "File";
            // 
            // fmHome
            // 
            this.fmHome.Name = "fmHome";
            this.fmHome.Size = new System.Drawing.Size(107, 22);
            this.fmHome.Text = "Home";
            this.fmHome.Click += new System.EventHandler(this.fmHome_Click);
            // 
            // fmExit
            // 
            this.fmExit.Name = "fmExit";
            this.fmExit.Size = new System.Drawing.Size(107, 22);
            this.fmExit.Text = "Exit";
            this.fmExit.Click += new System.EventHandler(this.fmExit_Click);
            // 
            // ipServices
            // 
            this.ipServices.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ipStartServer,
            this.ipStartUDPServer,
            this.ipStartResponder,
            this.ipStartTCP,
            this.ipTCPDiscon,
            this.ipStopTCP,
            this.ipStopUDPServer,
            this.ipStopAll});
            this.ipServices.Name = "ipServices";
            this.ipServices.Size = new System.Drawing.Size(74, 20);
            this.ipServices.Text = "IP Services";
            // 
            // ipStartServer
            // 
            this.ipStartServer.Name = "ipStartServer";
            this.ipStartServer.Size = new System.Drawing.Size(183, 22);
            this.ipStartServer.Text = "Start Server";
            this.ipStartServer.Click += new System.EventHandler(this.ipStartServer_Click);
            // 
            // ipStartUDPServer
            // 
            this.ipStartUDPServer.Name = "ipStartUDPServer";
            this.ipStartUDPServer.Size = new System.Drawing.Size(183, 22);
            this.ipStartUDPServer.Text = "Start UDP Server";
            this.ipStartUDPServer.Click += new System.EventHandler(this.ipStartUDPServer_Click);
            // 
            // ipStartResponder
            // 
            this.ipStartResponder.Name = "ipStartResponder";
            this.ipStartResponder.Size = new System.Drawing.Size(183, 22);
            this.ipStartResponder.Text = "Start UDP Responder";
            this.ipStartResponder.Click += new System.EventHandler(this.ipStartResponder_Click);
            // 
            // ipStartTCP
            // 
            this.ipStartTCP.Name = "ipStartTCP";
            this.ipStartTCP.Size = new System.Drawing.Size(183, 22);
            this.ipStartTCP.Text = "Start TCP Server";
            this.ipStartTCP.Click += new System.EventHandler(this.ipStartTCP_Click);
            // 
            // ipTCPDiscon
            // 
            this.ipTCPDiscon.Name = "ipTCPDiscon";
            this.ipTCPDiscon.Size = new System.Drawing.Size(183, 22);
            this.ipTCPDiscon.Text = "Disconnect TCP";
            this.ipTCPDiscon.Click += new System.EventHandler(this.ipTCPDiscon_Click);
            // 
            // ipStopTCP
            // 
            this.ipStopTCP.Name = "ipStopTCP";
            this.ipStopTCP.Size = new System.Drawing.Size(183, 22);
            this.ipStopTCP.Text = "Stop TCP Server";
            this.ipStopTCP.Click += new System.EventHandler(this.ipStopTCP_Click);
            // 
            // ipStopUDPServer
            // 
            this.ipStopUDPServer.Name = "ipStopUDPServer";
            this.ipStopUDPServer.Size = new System.Drawing.Size(183, 22);
            this.ipStopUDPServer.Text = "Stop UDP Server";
            this.ipStopUDPServer.Click += new System.EventHandler(this.ipStopUDPServer_Click);
            // 
            // vmView
            // 
            this.vmView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vmIPInfo});
            this.vmView.Name = "vmView";
            this.vmView.Size = new System.Drawing.Size(44, 20);
            this.vmView.Text = "View";
            // 
            // vmIPInfo
            // 
            this.vmIPInfo.Name = "vmIPInfo";
            this.vmIPInfo.Size = new System.Drawing.Size(108, 22);
            this.vmIPInfo.Text = "IP Info";
            this.vmIPInfo.Click += new System.EventHandler(this.vmIPInfo_Click);
            // 
            // hmHelp
            // 
            this.hmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hmAbout});
            this.hmHelp.Name = "hmHelp";
            this.hmHelp.Size = new System.Drawing.Size(44, 20);
            this.hmHelp.Text = "Help";
            // 
            // hmAbout
            // 
            this.hmAbout.Name = "hmAbout";
            this.hmAbout.Size = new System.Drawing.Size(107, 22);
            this.hmAbout.Text = "About";
            this.hmAbout.Click += new System.EventHandler(this.hmAbout_Click);
            // 
            // tStatus
            // 
            this.tStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tStatus.Location = new System.Drawing.Point(10, 359);
            this.tStatus.Multiline = true;
            this.tStatus.Name = "tStatus";
            this.tStatus.ReadOnly = true;
            this.tStatus.Size = new System.Drawing.Size(432, 84);
            this.tStatus.TabIndex = 1;
            // 
            // tInfo
            // 
            this.tInfo.Location = new System.Drawing.Point(10, 75);
            this.tInfo.Multiline = true;
            this.tInfo.Name = "tInfo";
            this.tInfo.ReadOnly = true;
            this.tInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tInfo.Size = new System.Drawing.Size(432, 207);
            this.tInfo.TabIndex = 2;
            // 
            // lStatus
            // 
            this.lStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(10, 347);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(37, 13);
            this.lStatus.TabIndex = 3;
            this.lStatus.Text = "Status";
            // 
            // cIPAddress
            // 
            this.cIPAddress.FormattingEnabled = true;
            this.cIPAddress.Location = new System.Drawing.Point(230, 52);
            this.cIPAddress.Name = "cIPAddress";
            this.cIPAddress.Size = new System.Drawing.Size(154, 21);
            this.cIPAddress.TabIndex = 4;
            this.cIPAddress.SelectedIndexChanged += new System.EventHandler(this.cIPAddress_SelectedIndexChanged);
            // 
            // tPort
            // 
            this.tPort.Location = new System.Drawing.Point(401, 52);
            this.tPort.Name = "tPort";
            this.tPort.Size = new System.Drawing.Size(41, 20);
            this.tPort.TabIndex = 5;
            // 
            // lIPAddress
            // 
            this.lIPAddress.AutoSize = true;
            this.lIPAddress.Location = new System.Drawing.Point(231, 35);
            this.lIPAddress.Name = "lIPAddress";
            this.lIPAddress.Size = new System.Drawing.Size(58, 13);
            this.lIPAddress.TabIndex = 7;
            this.lIPAddress.Text = "IP Address";
            // 
            // lPort
            // 
            this.lPort.AutoSize = true;
            this.lPort.Location = new System.Drawing.Point(401, 36);
            this.lPort.Name = "lPort";
            this.lPort.Size = new System.Drawing.Size(26, 13);
            this.lPort.TabIndex = 8;
            this.lPort.Text = "Port";
            // 
            // tAvailIPs
            // 
            this.tAvailIPs.BackColor = System.Drawing.SystemColors.Info;
            this.tAvailIPs.Location = new System.Drawing.Point(449, 52);
            this.tAvailIPs.Multiline = true;
            this.tAvailIPs.Name = "tAvailIPs";
            this.tAvailIPs.ReadOnly = true;
            this.tAvailIPs.Size = new System.Drawing.Size(144, 131);
            this.tAvailIPs.TabIndex = 9;
            // 
            // tClients
            // 
            this.tClients.BackColor = System.Drawing.SystemColors.Info;
            this.tClients.Location = new System.Drawing.Point(449, 216);
            this.tClients.Multiline = true;
            this.tClients.Name = "tClients";
            this.tClients.ReadOnly = true;
            this.tClients.Size = new System.Drawing.Size(144, 131);
            this.tClients.TabIndex = 10;
            // 
            // lAvailIPs
            // 
            this.lAvailIPs.AutoSize = true;
            this.lAvailIPs.Location = new System.Drawing.Point(448, 40);
            this.lAvailIPs.Name = "lAvailIPs";
            this.lAvailIPs.Size = new System.Drawing.Size(68, 13);
            this.lAvailIPs.TabIndex = 11;
            this.lAvailIPs.Text = "Available IPs";
            // 
            // lClients
            // 
            this.lClients.AutoSize = true;
            this.lClients.Location = new System.Drawing.Point(448, 204);
            this.lClients.Name = "lClients";
            this.lClients.Size = new System.Drawing.Size(38, 13);
            this.lClients.TabIndex = 12;
            this.lClients.Text = "Clients";
            // 
            // tUDPMonitor
            // 
            this.tUDPMonitor.BackColor = System.Drawing.SystemColors.Info;
            this.tUDPMonitor.Location = new System.Drawing.Point(279, 290);
            this.tUDPMonitor.Name = "tUDPMonitor";
            this.tUDPMonitor.ReadOnly = true;
            this.tUDPMonitor.Size = new System.Drawing.Size(163, 20);
            this.tUDPMonitor.TabIndex = 13;
            // 
            // tTCPMonitor
            // 
            this.tTCPMonitor.BackColor = System.Drawing.SystemColors.Info;
            this.tTCPMonitor.Location = new System.Drawing.Point(279, 316);
            this.tTCPMonitor.Name = "tTCPMonitor";
            this.tTCPMonitor.ReadOnly = true;
            this.tTCPMonitor.Size = new System.Drawing.Size(163, 20);
            this.tTCPMonitor.TabIndex = 14;
            // 
            // ipStopAll
            // 
            this.ipStopAll.Name = "ipStopAll";
            this.ipStopAll.Size = new System.Drawing.Size(183, 22);
            this.ipStopAll.Text = "Stop ALL Services";
            this.ipStopAll.Click += new System.EventHandler(this.ipStopAll_Click);
            // 
            // UDPServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 450);
            this.Controls.Add(this.tTCPMonitor);
            this.Controls.Add(this.tUDPMonitor);
            this.Controls.Add(this.lClients);
            this.Controls.Add(this.lAvailIPs);
            this.Controls.Add(this.tClients);
            this.Controls.Add(this.tAvailIPs);
            this.Controls.Add(this.lPort);
            this.Controls.Add(this.lIPAddress);
            this.Controls.Add(this.tPort);
            this.Controls.Add(this.cIPAddress);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.tInfo);
            this.Controls.Add(this.tStatus);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UDPServer";
            this.Text = "Server: Discover IP using UDP";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fmFile;
        private System.Windows.Forms.ToolStripMenuItem fmHome;
        private System.Windows.Forms.ToolStripMenuItem fmExit;
        private System.Windows.Forms.ToolStripMenuItem ipServices;
        private System.Windows.Forms.ToolStripMenuItem ipStartUDPServer;
        private System.Windows.Forms.ToolStripMenuItem ipStopUDPServer;
        private System.Windows.Forms.ToolStripMenuItem vmView;
        private System.Windows.Forms.ToolStripMenuItem vmIPInfo;
        private System.Windows.Forms.ToolStripMenuItem hmHelp;
        private System.Windows.Forms.ToolStripMenuItem hmAbout;
        private System.Windows.Forms.TextBox tStatus;
        private System.Windows.Forms.TextBox tInfo;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.ComboBox cIPAddress;
        private System.Windows.Forms.TextBox tPort;
        private System.Windows.Forms.Label lIPAddress;
        private System.Windows.Forms.Label lPort;
        private System.Windows.Forms.TextBox tAvailIPs;
        private System.Windows.Forms.TextBox tClients;
        private System.Windows.Forms.Label lAvailIPs;
        private System.Windows.Forms.Label lClients;
        private System.Windows.Forms.TextBox tUDPMonitor;
        private System.Windows.Forms.ToolStripMenuItem ipStartResponder;
        private System.Windows.Forms.ToolStripMenuItem ipStartTCP;
        private System.Windows.Forms.TextBox tTCPMonitor;
        private System.Windows.Forms.ToolStripMenuItem ipTCPDiscon;
        private System.Windows.Forms.ToolStripMenuItem ipStopTCP;
        private System.Windows.Forms.ToolStripMenuItem ipStartServer;
        private System.Windows.Forms.ToolStripMenuItem ipStopAll;
    }
}

