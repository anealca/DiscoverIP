namespace DiscoverIP
{
    partial class DiscoverPort
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
            this.tInfo = new System.Windows.Forms.TextBox();
            this.TStatus = new System.Windows.Forms.TextBox();
            this.LStatus = new System.Windows.Forms.Label();
            this.cIPAddrs = new System.Windows.Forms.ComboBox();
            this.lIPAddrs = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fmFile = new System.Windows.Forms.ToolStripMenuItem();
            this.fmHome = new System.Windows.Forms.ToolStripMenuItem();
            this.fmExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ipServices = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartUDP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartGVCP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStartTCP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipTCPDSiscon = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStopTCP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStopUDP = new System.Windows.Forms.ToolStripMenuItem();
            this.ipStopAll = new System.Windows.Forms.ToolStripMenuItem();
            this.vmView = new System.Windows.Forms.ToolStripMenuItem();
            this.vmIPInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.vmClient = new System.Windows.Forms.ToolStripMenuItem();
            this.vmServer = new System.Windows.Forms.ToolStripMenuItem();
            this.vmHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.hmClient = new System.Windows.Forms.ToolStripMenuItem();
            this.hmServer = new System.Windows.Forms.ToolStripMenuItem();
            this.hmAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tPort = new System.Windows.Forms.TextBox();
            this.lPort = new System.Windows.Forms.Label();
            this.tAvailIPs = new System.Windows.Forms.TextBox();
            this.tServers = new System.Windows.Forms.TextBox();
            this.lAvailIPs = new System.Windows.Forms.Label();
            this.lServers = new System.Windows.Forms.Label();
            this.tUDPMonitor = new System.Windows.Forms.TextBox();
            this.bHello = new System.Windows.Forms.Button();
            this.tTCPMonitor = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tInfo
            // 
            this.tInfo.Location = new System.Drawing.Point(10, 76);
            this.tInfo.Multiline = true;
            this.tInfo.Name = "tInfo";
            this.tInfo.ReadOnly = true;
            this.tInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tInfo.Size = new System.Drawing.Size(455, 217);
            this.tInfo.TabIndex = 2;
            // 
            // TStatus
            // 
            this.TStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TStatus.Location = new System.Drawing.Point(10, 369);
            this.TStatus.Multiline = true;
            this.TStatus.Name = "TStatus";
            this.TStatus.ReadOnly = true;
            this.TStatus.Size = new System.Drawing.Size(453, 86);
            this.TStatus.TabIndex = 3;
            // 
            // LStatus
            // 
            this.LStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LStatus.AutoSize = true;
            this.LStatus.Location = new System.Drawing.Point(10, 358);
            this.LStatus.Name = "LStatus";
            this.LStatus.Size = new System.Drawing.Size(37, 13);
            this.LStatus.TabIndex = 4;
            this.LStatus.Text = "Status";
            // 
            // cIPAddrs
            // 
            this.cIPAddrs.FormattingEnabled = true;
            this.cIPAddrs.Location = new System.Drawing.Point(247, 49);
            this.cIPAddrs.Name = "cIPAddrs";
            this.cIPAddrs.Size = new System.Drawing.Size(159, 21);
            this.cIPAddrs.TabIndex = 5;
            this.cIPAddrs.SelectedIndexChanged += new System.EventHandler(this.cIPAddrs_SelectedIndexChanged);
            // 
            // lIPAddrs
            // 
            this.lIPAddrs.AutoSize = true;
            this.lIPAddrs.Location = new System.Drawing.Point(253, 33);
            this.lIPAddrs.Name = "lIPAddrs";
            this.lIPAddrs.Size = new System.Drawing.Size(69, 13);
            this.lIPAddrs.TabIndex = 6;
            this.lIPAddrs.Text = "IP Addresses";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fmFile,
            this.ipServices,
            this.vmView,
            this.vmHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(604, 24);
            this.menuStrip1.TabIndex = 7;
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
            this.ipStartUDP,
            this.ipStartGVCP,
            this.ipStartTCP,
            this.ipTCPDSiscon,
            this.ipStopTCP,
            this.ipStopUDP,
            this.ipStopAll});
            this.ipServices.Name = "ipServices";
            this.ipServices.Size = new System.Drawing.Size(74, 20);
            this.ipServices.Text = "IP Services";
            // 
            // ipStartUDP
            // 
            this.ipStartUDP.Name = "ipStartUDP";
            this.ipStartUDP.Size = new System.Drawing.Size(185, 22);
            this.ipStartUDP.Text = "Start UDP Discovery";
            this.ipStartUDP.Click += new System.EventHandler(this.ipStartUDP_Click);
            // 
            // ipStartGVCP
            // 
            this.ipStartGVCP.Name = "ipStartGVCP";
            this.ipStartGVCP.Size = new System.Drawing.Size(185, 22);
            this.ipStartGVCP.Text = "Start GVCP Discovery";
            this.ipStartGVCP.Click += new System.EventHandler(this.ipStartGVCP_Click);
            // 
            // ipStartTCP
            // 
            this.ipStartTCP.Name = "ipStartTCP";
            this.ipStartTCP.Size = new System.Drawing.Size(185, 22);
            this.ipStartTCP.Text = "Start TCP Client";
            this.ipStartTCP.Click += new System.EventHandler(this.ipStartTCP_Click);
            // 
            // ipTCPDSiscon
            // 
            this.ipTCPDSiscon.Name = "ipTCPDSiscon";
            this.ipTCPDSiscon.Size = new System.Drawing.Size(185, 22);
            this.ipTCPDSiscon.Text = "TCP Disconnect";
            this.ipTCPDSiscon.Click += new System.EventHandler(this.ipTCPDSiscon_Click);
            // 
            // ipStopTCP
            // 
            this.ipStopTCP.Name = "ipStopTCP";
            this.ipStopTCP.Size = new System.Drawing.Size(185, 22);
            this.ipStopTCP.Text = "Stop TCP Client";
            this.ipStopTCP.Click += new System.EventHandler(this.ipStopTCP_Click);
            // 
            // ipStopUDP
            // 
            this.ipStopUDP.Name = "ipStopUDP";
            this.ipStopUDP.Size = new System.Drawing.Size(185, 22);
            this.ipStopUDP.Text = "Stop UDP Client";
            this.ipStopUDP.Click += new System.EventHandler(this.ipStopUDP_Click);
            // 
            // ipStopAll
            // 
            this.ipStopAll.Name = "ipStopAll";
            this.ipStopAll.Size = new System.Drawing.Size(185, 22);
            this.ipStopAll.Text = "Stop Both Services";
            this.ipStopAll.Click += new System.EventHandler(this.ipStopAll_Click);
            // 
            // vmView
            // 
            this.vmView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vmIPInfo,
            this.vmClient,
            this.vmServer});
            this.vmView.Name = "vmView";
            this.vmView.Size = new System.Drawing.Size(44, 20);
            this.vmView.Text = "View";
            // 
            // vmIPInfo
            // 
            this.vmIPInfo.Name = "vmIPInfo";
            this.vmIPInfo.Size = new System.Drawing.Size(135, 22);
            this.vmIPInfo.Text = "IP Info";
            this.vmIPInfo.Click += new System.EventHandler(this.vmIPInfo_Click);
            // 
            // vmClient
            // 
            this.vmClient.Name = "vmClient";
            this.vmClient.Size = new System.Drawing.Size(135, 22);
            this.vmClient.Text = "Client State";
            this.vmClient.Click += new System.EventHandler(this.vmClient_Click);
            // 
            // vmServer
            // 
            this.vmServer.Name = "vmServer";
            this.vmServer.Size = new System.Drawing.Size(135, 22);
            this.vmServer.Text = "Server State";
            this.vmServer.Click += new System.EventHandler(this.vmServer_Click);
            // 
            // vmHelp
            // 
            this.vmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hmClient,
            this.hmServer,
            this.hmAbout});
            this.vmHelp.Name = "vmHelp";
            this.vmHelp.Size = new System.Drawing.Size(44, 20);
            this.vmHelp.Text = "Help";
            // 
            // hmClient
            // 
            this.hmClient.Name = "hmClient";
            this.hmClient.Size = new System.Drawing.Size(130, 22);
            this.hmClient.Text = "Client Ops";
            this.hmClient.Click += new System.EventHandler(this.hmClient_Click);
            // 
            // hmServer
            // 
            this.hmServer.Name = "hmServer";
            this.hmServer.Size = new System.Drawing.Size(130, 22);
            this.hmServer.Text = "Server Ops";
            this.hmServer.Click += new System.EventHandler(this.hmServer_Click);
            // 
            // hmAbout
            // 
            this.hmAbout.Name = "hmAbout";
            this.hmAbout.Size = new System.Drawing.Size(130, 22);
            this.hmAbout.Text = "About";
            this.hmAbout.Click += new System.EventHandler(this.hmAbout_Click);
            // 
            // tPort
            // 
            this.tPort.Location = new System.Drawing.Point(412, 50);
            this.tPort.Name = "tPort";
            this.tPort.Size = new System.Drawing.Size(51, 20);
            this.tPort.TabIndex = 8;
            // 
            // lPort
            // 
            this.lPort.AutoSize = true;
            this.lPort.Location = new System.Drawing.Point(422, 33);
            this.lPort.Name = "lPort";
            this.lPort.Size = new System.Drawing.Size(26, 13);
            this.lPort.TabIndex = 9;
            this.lPort.Text = "Port";
            // 
            // tAvailIPs
            // 
            this.tAvailIPs.BackColor = System.Drawing.SystemColors.Info;
            this.tAvailIPs.Location = new System.Drawing.Point(471, 50);
            this.tAvailIPs.Multiline = true;
            this.tAvailIPs.Name = "tAvailIPs";
            this.tAvailIPs.ReadOnly = true;
            this.tAvailIPs.Size = new System.Drawing.Size(127, 136);
            this.tAvailIPs.TabIndex = 12;
            // 
            // tServers
            // 
            this.tServers.BackColor = System.Drawing.SystemColors.Info;
            this.tServers.Location = new System.Drawing.Point(471, 224);
            this.tServers.Multiline = true;
            this.tServers.Name = "tServers";
            this.tServers.ReadOnly = true;
            this.tServers.Size = new System.Drawing.Size(127, 69);
            this.tServers.TabIndex = 13;
            // 
            // lAvailIPs
            // 
            this.lAvailIPs.AutoSize = true;
            this.lAvailIPs.Location = new System.Drawing.Point(470, 38);
            this.lAvailIPs.Name = "lAvailIPs";
            this.lAvailIPs.Size = new System.Drawing.Size(68, 13);
            this.lAvailIPs.TabIndex = 14;
            this.lAvailIPs.Text = "Available IPs";
            // 
            // lServers
            // 
            this.lServers.AutoSize = true;
            this.lServers.Location = new System.Drawing.Point(468, 212);
            this.lServers.Name = "lServers";
            this.lServers.Size = new System.Drawing.Size(43, 13);
            this.lServers.TabIndex = 15;
            this.lServers.Text = "Servers";
            // 
            // tUDPMonitor
            // 
            this.tUDPMonitor.BackColor = System.Drawing.SystemColors.Info;
            this.tUDPMonitor.Location = new System.Drawing.Point(283, 303);
            this.tUDPMonitor.Name = "tUDPMonitor";
            this.tUDPMonitor.ReadOnly = true;
            this.tUDPMonitor.Size = new System.Drawing.Size(182, 20);
            this.tUDPMonitor.TabIndex = 16;
            // 
            // bHello
            // 
            this.bHello.Location = new System.Drawing.Point(181, 49);
            this.bHello.Name = "bHello";
            this.bHello.Size = new System.Drawing.Size(57, 20);
            this.bHello.TabIndex = 17;
            this.bHello.Text = "Hello?";
            this.bHello.UseVisualStyleBackColor = true;
            this.bHello.Click += new System.EventHandler(this.bHello_Click);
            // 
            // tTCPMonitor
            // 
            this.tTCPMonitor.BackColor = System.Drawing.SystemColors.Info;
            this.tTCPMonitor.Location = new System.Drawing.Point(283, 335);
            this.tTCPMonitor.Name = "tTCPMonitor";
            this.tTCPMonitor.ReadOnly = true;
            this.tTCPMonitor.Size = new System.Drawing.Size(182, 20);
            this.tTCPMonitor.TabIndex = 18;
            // 
            // DiscoverPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 461);
            this.ControlBox = false;
            this.Controls.Add(this.tTCPMonitor);
            this.Controls.Add(this.bHello);
            this.Controls.Add(this.tUDPMonitor);
            this.Controls.Add(this.lServers);
            this.Controls.Add(this.lAvailIPs);
            this.Controls.Add(this.tServers);
            this.Controls.Add(this.tAvailIPs);
            this.Controls.Add(this.lPort);
            this.Controls.Add(this.tPort);
            this.Controls.Add(this.lIPAddrs);
            this.Controls.Add(this.cIPAddrs);
            this.Controls.Add(this.LStatus);
            this.Controls.Add(this.TStatus);
            this.Controls.Add(this.tInfo);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DiscoverPort";
            this.Text = "Client: Discover IP using UDP ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tInfo;
        private System.Windows.Forms.TextBox TStatus;
        private System.Windows.Forms.Label LStatus;
        private System.Windows.Forms.ComboBox cIPAddrs;
        private System.Windows.Forms.Label lIPAddrs;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fmFile;
        private System.Windows.Forms.ToolStripMenuItem fmHome;
        private System.Windows.Forms.ToolStripMenuItem fmExit;
        private System.Windows.Forms.ToolStripMenuItem ipServices;
        private System.Windows.Forms.ToolStripMenuItem ipStartUDP;
        private System.Windows.Forms.ToolStripMenuItem ipStartTCP;
        private System.Windows.Forms.ToolStripMenuItem vmView;
        private System.Windows.Forms.ToolStripMenuItem vmIPInfo;
        private System.Windows.Forms.ToolStripMenuItem vmClient;
        private System.Windows.Forms.ToolStripMenuItem vmServer;
        private System.Windows.Forms.ToolStripMenuItem vmHelp;
        private System.Windows.Forms.ToolStripMenuItem hmClient;
        private System.Windows.Forms.ToolStripMenuItem hmServer;
        private System.Windows.Forms.ToolStripMenuItem hmAbout;
        private System.Windows.Forms.TextBox tPort;
        private System.Windows.Forms.Label lPort;
        private System.Windows.Forms.ToolStripMenuItem ipStopUDP;
        private System.Windows.Forms.TextBox tAvailIPs;
        private System.Windows.Forms.TextBox tServers;
        private System.Windows.Forms.Label lAvailIPs;
        private System.Windows.Forms.Label lServers;
        private System.Windows.Forms.TextBox tUDPMonitor;
        private System.Windows.Forms.ToolStripMenuItem ipStartGVCP;
        private System.Windows.Forms.Button bHello;
        private System.Windows.Forms.TextBox tTCPMonitor;
        private System.Windows.Forms.ToolStripMenuItem ipStopTCP;
        private System.Windows.Forms.ToolStripMenuItem ipTCPDSiscon;
        private System.Windows.Forms.ToolStripMenuItem ipStopAll;
    }
}

