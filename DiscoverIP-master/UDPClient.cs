using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace DiscoverIP
{
    // This app is set up to Discover a Server via UDP and connect via TCP
    /*  It allows the user to select which IP Port to use for this action...
            And will only accept clients on that network...
        Process:
            1) Find an Ethernet port to use and monitor the services status
                - Select the last available Ethernet address
                - Start the StateMonitor on a separate thread to track service status
            2) Start a UDP Broadcaster and listener to discover the Server's IP Address
                - The Braodcaster to sends out the message "Client IP=<ip addr>"
                    ON the selected UDP Port (udpPort) to the broadcast address (255.255.255.255)
                - AND starts the listener looking for a responce "Server IP=<ip addr>"
                    ON the selected UDP port -1
            3) Wait for a server to respond
                - The UDP Listener will update the Server list with detected server's
                - Sets the Connect flag to create a TCP connection to the server
      */
    public partial class DiscoverPort : Form
    {
        /*
        NOTES:
            - Startup automatically starts the UDP Broadcaster "UDPBroadcaster()" and Listener "UDPListener()"
                - Has a 1 sec timeout on RX so this can be used to detect QUIT "broadcasting"
            - Startup also starts a link monitor "StatusMonitor()"...
                - This updates the GUI UDP and TCP States (progress bar and state text)
            - Start the Server side TCP service before this client side service (Menu->IP Services->Start TCP Client)...
                - Currently is not automated...
            - Disconnect of this client is detected by the monitor
                - Use the Menu->IP Services to select the service to stop...
                    - NOTE: Stopping the TCP doesn't stop the UDP
                            Stopping the UDP doesn't stop the TCP
                - Waits 5 seconds to be sure the link is down...
                    - Depends on the 5 second PING<->PONG TCP Msg... AND the ServerIP<->ClientIP UDP msgs...
        */
        // Local IP params
        string ipAddrStr = "";
        int udpPort = 340;
        List<string> availIPAddresses = new List<string>(); // This PC's WAN ports
        static List<string> serverList = new List<string>(); // Detected Servers
        bool initialized = false;

        // The main app...
        public DiscoverPort() {
            InitializeComponent();
            InitMyProgressBars();
            AddStatus(DateTime.Now.ToString(),"App Started...");
            myMonitor = new Thread(new ThreadStart(StatusMonitor));
            myMonitor.Start();
            FindIPAddress();
            tPort.Text = udpPort.ToString();
            anounceClientInfo = "Client IP=";
            //StartUDPBroadcast();

            // Add events for text boxes...
            tPort.MouseLeave += TPort_MouseLeave;
            cIPAddrs.TextChanged += CIPAddrs_TextChanged;

            // May need a slight delay here...
            Thread.Sleep(100); // Allow time for threads to start...
            initialized = true;
        }

        // EVENTS...
        private void TPort_MouseLeave(object sender, EventArgs e) {
            //throw new NotImplementedException();
            try {
                int newport = Int32.Parse(tPort.Text);
                if (udpPort != newport) {
                    udpPort = newport;
                    AddStatus("UDP Port changed to " + udpPort.ToString()+" (IP="+ipAddrStr+')');
                }
            }
            catch (Exception) {
                AddInfo("Port Chg Exception...");
                udpPort = 8888;
            }
        }
        private void cIPAddrs_SelectedIndexChanged(object sender, EventArgs e) {
            if (!initialized) return;
            ipAddrStr = cIPAddrs.SelectedItem.ToString();
            AddStatus("TCP Address set to " + ipAddrStr);
        }
        private void CIPAddrs_TextChanged(object sender, EventArgs e) {
            //throw new NotImplementedException();
            if (IsIPValid(cIPAddrs.Text)) {
                ipAddrStr = cIPAddrs.Text;
                AddStatus("IP Address changed to: "+ipAddrStr+" on port "+udpPort.ToString());
            }
        }

        // MENU Items
        private void fmHome_Click(object sender, EventArgs e) {

        }
        private void fmExit_Click(object sender, EventArgs e) {
            broadcasting = false;
            if (myBroadcaster != null) {
                myBroadcaster.Abort();
                myBroadcaster = null;
            }
            if (myListener != null) {
                myListener.Abort();
                myListener = null;
            }
            quitUDPMonitor = true;
            Environment.Exit(0);
        }
        private void ipStartUDP_Click(object sender, EventArgs e) {
            StartUDPAutoConnect();
        }
        private void ipStartTCP_Click(object sender, EventArgs e) {
            if (serverList.Count>0) StartTCPLink(serverList[0]);
        }
        private void ipStartGVCP_Click(object sender, EventArgs e) {
            StartGVCPListener();
        }
        private void ipTCPDSiscon_Click(object sender, EventArgs e) {
            DisconnectTCPLink();
        }
        private void ipStopTCP_Click(object sender, EventArgs e) {
            StopTCPLink();
        }
        private void ipStopUDP_Click(object sender, EventArgs e) {
            StopUDP();
        }
        private void ipStopAll_Click(object sender, EventArgs e) {
            StopTCPLink();
            StopUDP();
        }
        private void vmIPInfo_Click(object sender, EventArgs e) {
            infotxt.Clear();
            FindIPAddress();
            foreach (string ipAddr in availIPAddresses)
                infotxt.Add(ipAddr);
            tInfo.Lines = infotxt.ToArray();
        }
        private void vmClient_Click(object sender, EventArgs e) {

        }
        private void vmServer_Click(object sender, EventArgs e) {

        }
        private void hmClient_Click(object sender, EventArgs e) {
            infotxt.Clear();
            infotxt.Add("UDP Client Info:");
            infotxt.Add("  - In a second instan=ce of Visusal Studio...");
            infotxt.Add("      - Startup a UDP Server (TCPServices->UDPServer->Start)");
            infotxt.Add("      - Note the default port is 8888...");
            infotxt.Add("  - Get the IP Information and select the IP params.");
            infotxt.Add("  - Type in the announcment string.");
            infotxt.Add("  - Select IP Services->Start UDP Client.");
        }
        private void hmServer_Click(object sender, EventArgs e) {

        }
        private void hmAbout_Click(object sender, EventArgs e) {

        }

        private void bHello_Click(object sender, EventArgs e) {
            //sendHello = !sendHello;
            SendPkt("Hello You...");
        }

        // Hard conversion of a UINT to an IP Address string...
        private bool IsIPValid(string ipAddr) {
            if ((ipAddr.Length>6)&&(ipAddr.Length<16)) {
                // IP Addresses format: x.x.x.x
                try {
                    IPAddress myIP = IPAddress.Parse(ipAddr);
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        } // Check for parsing exceptions...
        public string ConvertIPAddrBytesToStr(byte[] ipAddr) {
            uint[] ipparts = new uint[4];
            if (BitConverter.IsLittleEndian) Array.Reverse(ipAddr);
            uint ipAsInt = BitConverter.ToUInt32(ipAddr, 0);
            // Convert.ToString(ipAsInt, 2); Show the IP Address in Binary...
            // ipAsInt.ToString(); Shows it as the Decimal number
            ipparts[0] = ipAsInt / 16777216; // divide by 256^3
            ipparts[1] = (ipAsInt - ipparts[0] * 16777216) / 65536; // divide by 256^2
            ipparts[2] = (ipAsInt - ipparts[0] * 16777216 - ipparts[1] * 65536) / 256; // divide by 256^1
            ipparts[3] = ipAsInt - ipparts[0] * 16777216 - ipparts[1] * 65536 - ipparts[2] * 256;
            return ipparts[0].ToString() + "." + ipparts[1].ToString() + "." + ipparts[2].ToString() + "." + ipparts[3].ToString();
        }

        // UDP
        static int udpRxCnt;
        string anounceClientInfo;
        bool broadcasting = false; // UDP Broadcaster Shutdown flag
        static bool udpBroadCasterRunning = false; // UDP Broadcaster state
        static bool udpListenerRunning = false; // UDP Listener state
        static bool quitUDPMonitor = false; // UDP Monitor Shutdown flag
        static bool sendUDPHello = false; // UDP Responder flag
        // UDP/TCP Threads
        Thread myMonitor = null;
        Thread myBroadcaster = null;
        Thread myListener = null;
        private void StartUDPAutoConnect() {
            tInfo.Clear();
            ClearInfo();
            if (ipAddrStr.Length>7) {
                tServers.Text = "Starting UDP";
                bool initBrdCaster = (myBroadcaster==null);
                broadcasting = true;
                if (!initBrdCaster)
                    if (myBroadcaster.ThreadState != ThreadState.Running)
                        initBrdCaster = true;
                if (initBrdCaster) {
                    myBroadcaster = new Thread(new ThreadStart(UDPBroadcaster));
                    myBroadcaster.Start();
                    AddInfo("Broadcaster started...");
                }
                bool initListener = (myListener==null);
                if (!initListener)
                    if (myListener.ThreadState != ThreadState.Running)
                        initListener = true;
                if (initListener) {
                    myListener = new Thread(new ThreadStart(UDPListener));
                    myListener.Start();
                    AddInfo("Listener started...");
                }
            } else
                AddInfo("First select an IP Address ["+ipAddrStr+"]...");
        } // Start up the UD Services
        private void StopUDP() {
            broadcasting = false;
            while (udpBroadCasterRunning) {
                AddInfo("Waiting for Broadcaster to stop...");
                Thread.Sleep(1000);
                udpBroadCasterRunning = false;
            }
            AddInfo("Broadcaster stopped...");
            while (udpListenerRunning) {
                AddInfo("Waiting for Listener to stop...");
                Thread.Sleep(1000);
            }
            AddInfo("Listener stopped...");
            tServers.Text = "UDP Stopped";
        } // Shut down the UDP services
        private void UDPBroadcaster() {
            // Send out a Broadcast with my IP in the msg
            // Seems to require a separate task (UDPListener) to get the responce...
            ClearInfoText();
            UpdateInfo("UDPBrdcast: Starting UDP Broadcaster...");
            UdpClient myClient = new UdpClient();
            byte[] txPkt = Encoding.ASCII.GetBytes(anounceClientInfo+ipAddrStr); // Append my IP to the msg
            int lastRxCnt = 0;

            UpdateStatusText("UDP Broadcasting \""+anounceClientInfo+ipAddrStr+"\" on port "+udpPort.ToString()+"...");
            myClient.EnableBroadcast = true;
            int i = 1;
            udpBroadCasterRunning = true;
            while (broadcasting) {
                // Can use rxCnt to detect connection state
                //if (lastRxCnt==rxCnt) UpdateInfoText("UDPBrdcast: (RX Discon="+rxCnt+") Sending "+i.ToString()+"...");
                //else UpdateInfoText("UDPBrdcast: (RX Cnt'd="+rxCnt+") Sending "+i.ToString()+"...");
                lastRxCnt = udpRxCnt;
                // Now broadcast the msg...
                myClient.Send(txPkt, txPkt.Length, new IPEndPoint(IPAddress.Broadcast, udpPort));
                Thread.Sleep(1000); // Wait 1 second then send another broadcast...
                i++;
            }
            UpdateInfo("UDPBrdcast: Closing...");
            myClient.Close();
            udpBroadCasterRunning = false;
        } // Broadcast a packet until the Server is found
        private void UDPListener() {
            // Attach to a port and look for msg's 
            byte[] rxData = new byte[1024];
            byte[] txPkt = Encoding.ASCII.GetBytes("Hello...");
            string srvrRspStr;
            int pktSize, loopCnt=0;
            bool tcpStarted = false;

            udpRxCnt = 0;
            serverList.Clear();
            IPEndPoint myClientEP = new IPEndPoint(IPAddress.Parse(ipAddrStr), udpPort - 1);
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Setup a UDP DataGram socket.
            newSocket.ReceiveTimeout = 100;
            newSocket.Bind(myClientEP);   // Bind the RX endpoint to the port

            UpdateStatusText("UDP Listening for a Server on port " + (udpPort - 1).ToString() + "...");
            udpListenerRunning = true;
            while (broadcasting) {
                try {
                    pktSize = newSocket.Receive(rxData);
                    if (pktSize>7) {
                        udpRxCnt++;
                        srvrRspStr = Encoding.ASCII.GetString(rxData, 0, pktSize);
                        if (String.Compare(srvrRspStr, 0, "Server IP=", 0, 10, true)==0) {
                            string[] rxParts = srvrRspStr.Split('=');
                            if (rxParts.Count()>1) {
                                //UpdateInfoText(loopCnt+") asyncClient RX ("+rxCnt+") Server Found at: " + rxParts[1]);
                                if (!serverList.Contains(rxParts[1])) serverList.Add(rxParts[1]);
                                if (!tcpStarted) {
                                    StartTCPLink(rxParts[1]);
                                    tcpStarted = true;
                                }
                                if (sendUDPHello) {
                                    UpdateInfo(loopCnt+") Sending \"Hello...\" ("+udpRxCnt+")");
                                    /* Cannot seem to send on this Socket?
                                    try { newSocket.Send(txPkt, txPkt.Length, SocketFlags.None); } // This fails to send anything...
                                    catch (Exception) { UpdateInfo("TX Hello failed"); }
                                    */
                                    sendUDPHello = false;
                                }
                            }
                        } else
                            UpdateInfo(loopCnt+") asyncClient RX ("+udpRxCnt+") : " + srvrRspStr);

                    }
                }
                catch (Exception) { }
                loopCnt++;
            }
            //
            udpListenerRunning = false;
            UpdateInfo("asyncClient: Closing...");
            newSocket.Close();
        } // Listen on UDP for server responces...
        public void UpdateServerText() {
            Func<int> serverdel = delegate () {
                tServers.Lines = serverList.ToArray();
                return 0;
            };
            Invoke(serverdel);
        } // Write the IP Address of found servers to the GUI
        private void StartGVCPListener() {
            tInfo.Clear();
            ClearInfo();
            if (ipAddrStr.Length>7) {
                tServers.Text = "GVCP Discovery";
                bool initListener = (myListener==null);
                if (!initListener)
                    if (myListener.ThreadState != ThreadState.Running)
                        initListener = true;
                if (initListener) {
                    myListener = new Thread(new ThreadStart(GVCPListener));
                    myListener.Start();
                    AddInfo("GVCP Listener started...");
                }
            } else
                AddInfo("First select an IP Address ["+ipAddrStr+"]...");
        } // Start up the UD Services
        private void GVCPListener() {
            byte[] rxData = new byte[1024];
            //string srvrRspStr;
            int pktSize, loopCnt = 0;
            udpRxCnt = 0;
            IPEndPoint myClientEP = new IPEndPoint(IPAddress.Parse(ipAddrStr), 3956);
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Setup a UDP DataGram socket.
            newSocket.ReceiveTimeout = 100;
            newSocket.Bind(myClientEP);   // Bind the RX endpoint to the port

            UpdateStatusText("GVCP Listening for a Discovery Msg on port 3956...");
            /* Msg Format: 
                <xFF...FF> - Broadcast Msg
                ...<hdr Checksum>
                <Source IP><Dest IP>
                <Source Port><Dest Prt>
                ... <Checksum>
                <Datagram Pkt> = Rx data
            */
            broadcasting = true;
            udpListenerRunning = true;
            while (broadcasting) {
                try {
                    pktSize = newSocket.Receive(rxData);
                    if (pktSize>7) {
                        udpRxCnt++;
                        /* RX Data: x42 x11 x00 x02 x00 x00 x00 x01
                            x42 - Msg Key code
                            x11 - Flags
                            x0002 - Discovery Cmd
                            x0000 - No data
                            x0001 - ID Request
                        */
                        //srvrRspStr = Encoding.ASCII.GetString(rxData, 0, pktSize);
                        UpdateInfo(loopCnt+") GVCP RX ("+udpRxCnt+") msg...");
                    }
                }
                catch (Exception) { }
                loopCnt++;
            }
            UpdateInfo("Closing GVCP listener...");
            udpListenerRunning = false;
            newSocket.Close();
        } // Listen on UDP for server responces...

        // TCP Ftns
        static int tcpMsgCnt = -1;
        static bool quitTCP = false; // TCP Shutdown flag
        static bool connected = false; // TCP Connect state
        static string cmdTxt = ""; // Data to send to the server
        static string rxMsg; // The responce from the server
        static bool rspReady = false; // Flag indicating a responce hes been RX'd
        string serverIP;
        Thread myTCPLink = null;
        private static Socket _clientSocket;
        public void StartTCPLink(string ipAddr) {
            UpdateServerText();
            quitTCP = false;
            bool startTCP = (myTCPLink==null);
            if (!startTCP)
                if (myTCPLink.ThreadState != ThreadState.Running) startTCP = true;
            if (startTCP) {
                serverIP = ipAddr;
                myTCPLink = new Thread(new ThreadStart(ConnectToServer));
                myTCPLink.Start();
            }
        } // Startup the TCP Link
        public void DisconnectTCPLink() {
            connected = false;
            _clientSocket.Disconnect(true);
        }
        public void StopTCPLink() {
            quitTCP = true;
            connected = false;
            if (_clientSocket != null) {
                _clientSocket.Close();
                _clientSocket.Dispose();
                _clientSocket = null;
            }
            if (myTCPLink!=null) {
                myTCPLink.Abort();
                myTCPLink = null;
            }
            UpdateTCPMonitor("Stopped...");
            IncTCPProgress(Color.Red);
        }
        private void ConnectToServer() {
            int connectionCnt = 0;
            UpdateInfo("Waiting for a server to accept a connection...");
            while (broadcasting && !quitTCP) { // !_clientSocket.Connected && 
                if (_clientSocket==null)
                    _clientSocket = new Socket   // Create a local socket to listen on
                        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try {
                    connectionCnt++;
                    _clientSocket.Connect(IPAddress.Parse(serverIP), udpPort);  // Connect to the server using TCP
                }
                catch (SocketException) {
                    if ((connectionCnt == 1) || (connectionCnt % 10 == 0)) {
                        UpdateInfo(connectionCnt.ToString()); // Display the # failed Cnct's
                        return;
                    }
                }
                if (_clientSocket.Connected && !quitTCP) {
                    UpdateInfo("Connected to the Server via TCP/IP...");
                    SendLoop();
                    _clientSocket.Close();
                    _clientSocket.Dispose();
                    _clientSocket = null;
                    Thread.Sleep(2000);
                }
            }
            _clientSocket.Close();
            _clientSocket.Dispose();
            _clientSocket = null;
            connected = false;
        }
        private void SendLoop() { // Static?
            byte[] rxData = new byte[1024];
            byte[] dataLen = new byte[4];   // The server first 4 bytes is the data length...
            byte[] cmdData;
            int pingDelay = 0;

            connected = true;
            tcpMsgCnt = 0;
            while (connected && _clientSocket.Connected) {
                if (cmdTxt.Length >= 2)
                    cmdData = Encoding.ASCII.GetBytes(cmdTxt); // Convert the command to a byte array
                else
                    cmdData = Encoding.ASCII.GetBytes("Ping"); // Send a Ping...
                tcpMsgCnt++;
                try {
                    _clientSocket.Send(cmdData);                // Send the command to the server and look for a responce...
                    cmdTxt = "";                                // Clear the TX string so it isn't sent twice...
                    int rxCnt = _clientSocket.Receive(rxData);  // Read from the server...
                    Array.Copy(rxData, dataLen, 4);             // Get the RX Data length (4 bytes)
                    byte[] rxTxt = new byte[rxCnt - 4];         // Make the RX Data buffer the length of the RX'd data
                    Array.Copy(rxData, 4, rxTxt, 0, rxCnt - 4); // Now get only the Data bytes
                    rxMsg = Encoding.ASCII.GetString(rxTxt);
                    UpdateInfo("Msg RX'd: "+rxMsg);
                    rspReady = true;
                }
                catch (Exception) {
                    UpdateInfo("Client Socket Exception => Closing TCP/IP Link...");
                    connected = false;
                }
                //UpdateTCPprogress(true);

                // Wait for something to send...
                pingDelay = 0;
                while (connected && (cmdTxt.Length < 2)&&(pingDelay<50)) {
                    if (quitTCP) {
                        _clientSocket.Close();
                        return;
                    }
                    Thread.Sleep(100);
                    pingDelay++; // Send a PING every 5 seconds...
                }
            } // While we are connected...

            connected = false;
            /* We are Quitting OR no longer Connected => Clean up the socket.
            _clientSocket.Close();
            _clientSocket.Dispose();
            _clientSocket = null;
            */
        }
        public string SendPkt(string txt) {
            if (!connected) {
                AddInfo("Connect and retry...");
                AddInfo("Not connected to the server...");
                return null;
            }
            //Send the request and wait for a responce...
            rspReady = false;
            cmdTxt = txt;
            int i = 0;
            while (!rspReady && (i < 200)) { // Wait 2 seconds for a responce...
                Thread.Sleep(10); i++;
            }
            // Note: The first 4 bytes are the string length..
            if (rspReady) {
                AddInfo(rxMsg+"->RX'd");
                return rxMsg;
            }
            //AddStatus("Failed to retrieve a responce for: " + txt);
            return null;
        }

        // Progress bar
        static MyProgBar2 myUDPPrgBar, myTCPPrgBar;
        static Color progColor = Color.Yellow;
        static int udpProgState = 0, tcpProgState = 0;
        public void InitMyProgressBars() {
            myUDPPrgBar = new MyProgBar2();
            myUDPPrgBar.Name = "udpPrgbar";
            myUDPPrgBar.Location = new Point(10, 300);
            myUDPPrgBar.Size = new Size(250, 25);
            myUDPPrgBar.ProgressColor = progColor;
            myUDPPrgBar.ProgressDirection = MyProgBar2.ProgressDir.Horizontal;
            myUDPPrgBar.RoundedCorners = true;
            myUDPPrgBar.Text = "UDP State";
            myUDPPrgBar.ShowText = true;
            Controls.Add(myUDPPrgBar);
            myUDPPrgBar.Click += MyUDPPrgBar_Click; // Normally comment this line out...
            myUDPPrgBar.Show(); // Don't show the bar until the TCP server is started...
            // TCP
            myTCPPrgBar = new MyProgBar2();
            myTCPPrgBar.Name = "tcpPrgBar";
            myTCPPrgBar.Location = new Point(10, 325);
            myTCPPrgBar.Size = new Size(250, 25);
            myTCPPrgBar.ProgressColor = progColor;
            myTCPPrgBar.ProgressDirection = MyProgBar2.ProgressDir.Horizontal;
            myTCPPrgBar.RoundedCorners = true;
            myTCPPrgBar.Text = "TCP/IP State";
            myTCPPrgBar.ShowText = true;
            Controls.Add(myTCPPrgBar);
            myTCPPrgBar.Show(); // Don't show the bar until the TCP server is started...
        } // Open and place the TCP/IP progress bar.
        private void MyUDPPrgBar_Click(object sender, EventArgs e) {
            myUDPPrgBar.ProgressColor = Color.Gray;
        } // Change colour when selected...
        void IncUDPProgress() {
            udpProgState++;
            if (udpProgState>=100) udpProgState = 0;
            myUDPPrgBar.Value = udpProgState;
        }
        void IncUDPProgress(Color newColor) {
            Func<int> udpPrgdel = delegate () {
                myUDPPrgBar.ProgressColor = newColor;
                udpProgState++;
                if (udpProgState>=100) udpProgState = 0;
                myUDPPrgBar.Value = udpProgState;
                return 0;
            };
            Invoke(udpPrgdel);
        }
        void IncTCPProgress(Color newColor) {
            Func<int> tcpPrgdel = delegate () {
                myTCPPrgBar.ProgressColor = newColor;
                tcpProgState++;
                if (tcpProgState>=100) tcpProgState = 0;
                myTCPPrgBar.Value = tcpProgState;
                return 0;
            };
            Invoke(tcpPrgdel);
        }

        // Monitor Ftn's
        private void FindIPAddress() {
            cIPAddrs.Items.Clear(); availIPAddresses.Clear();
            Socket mySkt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int i = 0;
            foreach (NetworkInterface thisIF in NetworkInterface.GetAllNetworkInterfaces()) {
                if ((thisIF.OperationalStatus == OperationalStatus.Up)
                        && ((String.Compare(thisIF.Name, "Ethernet", true) == 0) || (String.Compare(thisIF.Name, "Wi-Fi", true) == 0))) {
                    foreach (UnicastIPAddressInformation thisAddr in thisIF.GetIPProperties().UnicastAddresses) {
                        byte[] ipAddr = thisAddr.Address.GetAddressBytes();
                        string ipAddressstr = ConvertIPAddrBytesToStr(ipAddr);
                        cIPAddrs.Items.Add(ipAddressstr);
                        availIPAddresses.Add(ipAddressstr);
                        if (String.Compare("Ethernet", thisIF.Name, true) == 0) {
                            cIPAddrs.SelectedIndex = i; // Select the LAST Ethernet port
                            ipAddrStr = ipAddressstr;
                            AddStatus("TCP Address set to " + ipAddrStr);
                        } // IF this is an Ethernet port
                        i++;
                    } // Foreach defined IP address...
                } // IF this is a TCP/IP port that is UP
            } // For every known network interface
            if ((cIPAddrs.SelectedIndex<0)&&(availIPAddresses.Count>0)) {
                i=availIPAddresses.Count-1; // IFF there was no Ethernet WAN ports... use the last available address...
                ipAddrStr = availIPAddresses[i];
                cIPAddrs.SelectedIndex = i;
                AddStatus("TCP Address set to " + ipAddrStr);
            }
            tAvailIPs.Lines = availIPAddresses.ToArray();
        } // Look for Ethernet ports to use
        private void StatusMonitor() {
            int lastUDPCnt = -1, lastTCPCnt = -1;
            int loopCnt = 0, nextTxCnt = -1;
            quitUDPMonitor = false;
            Thread.Sleep(1000);
            UpdateUDPMonitor("UDP Stopped...");
            IncUDPProgress(Color.Red);
            lastUDPCnt = udpRxCnt;
            UpdateTCPMonitor("TCP Stopped...");
            IncTCPProgress(Color.Red);
            loopCnt=nextTxCnt;
            while (!quitUDPMonitor) {
                if (broadcasting) {
                    if (udpBroadCasterRunning && udpListenerRunning) {
                        if (lastUDPCnt < udpRxCnt) {
                            UpdateUDPMonitor("UDP Connected...");
                            IncUDPProgress(Color.Green);
                            lastUDPCnt = udpRxCnt;
                        } else {
                            UpdateUDPMonitor("Waiting for UDP msg...");
                            IncUDPProgress(Color.Yellow);
                        }
                    } else {
                        UpdateUDPMonitor("UDP Starting...");
                        IncUDPProgress(Color.Yellow);
                    }
                    if (lastUDPCnt>udpRxCnt) lastUDPCnt=udpRxCnt;
                    if (connected && (myTCPLink != null)) {
                        if (lastTCPCnt<tcpMsgCnt) {
                            UpdateTCPMonitor("TCP Connected...");
                            IncTCPProgress(Color.Green);
                            lastTCPCnt = tcpMsgCnt;
                            nextTxCnt = loopCnt+5; // Ping-Pong happens every 5 sec's
                        } else {
                            if (loopCnt < nextTxCnt)
                                IncTCPProgress(Color.Green);
                            else if (loopCnt>nextTxCnt+10) {
                                UpdateTCPMonitor("TCP Waiting...");
                                IncTCPProgress(Color.Red);
                                try { _clientSocket.Disconnect(true); }
                                catch (Exception) { }
                            } else { 
                                UpdateTCPMonitor("Waiting for TCP cnct...");
                                IncTCPProgress(Color.Yellow);
                            }
                        }
                        if (lastTCPCnt>tcpMsgCnt) lastTCPCnt=tcpMsgCnt;
                    } else {
                        if (!connected && (lastTCPCnt>=0)) {
                            UpdateTCPMonitor("TCP Stopped...");
                            IncTCPProgress(Color.Red);
                            lastTCPCnt = -1;
                        }
                    }
                } else { // NOT Broadcasting...
                    if (udpBroadCasterRunning||udpListenerRunning) {
                        UpdateUDPMonitor("Shutting Down UDP...");
                        IncUDPProgress(Color.Red);
                    } else {
                        UpdateUDPMonitor("UDP Stopped...");
                        if (lastUDPCnt>=0) {
                            IncUDPProgress(Color.Red);
                            lastUDPCnt = -1;
                        }
                    }
                }
                Thread.Sleep(1050);
                loopCnt++;
            }
        } // Report the state of the UDP/TCP services...
        public void UpdateUDPMonitor(string newline) {
            if (!initialized) return;
            Func<int> udpMondel = delegate () {
                tUDPMonitor.Text = newline;
                return 0;
            };
            Invoke(udpMondel);
        } // Write out the state to the GUI
        public void UpdateTCPMonitor(string newline) {
            if (!initialized) return;
            Func<int> tcpMondel = delegate () {
                tTCPMonitor.Text = newline;
                return 0;
            };
            Invoke(tcpMondel);
        } // Write out the state to the GUI

        // Info Text
        static List<string> infotxt = new List<string>();
        private void ClearInfo() { infotxt.Clear(); }
        private void AddInfo(string txt) {
            infotxt.Insert(0, DateTime.Now.ToShortTimeString()+" - "+txt);
            tInfo.Lines = infotxt.ToArray();
        }
        private void ClearInfoText() {
            Func<int> del = delegate () {
                tInfo.Clear();
                return 0;
            };
        }
        private void UpdateInfo(string txt) {
            if (!initialized) return;
            Func<int> del = delegate () {
                infotxt.Insert(0,DateTime.Now.ToShortTimeString()+" - "+txt);
                tInfo.Lines = infotxt.ToArray();
                return 0;
            };
            Invoke(del);
        }
        // Status
        static List<string> statustxt = new List<string>();
        private void AddStatus(string txt) {
            statustxt.Insert(0, DateTime.Now.ToLongTimeString() + "-" + txt);
            TStatus.Lines = statustxt.ToArray();
        }
        private void AddStatus(string date, string txt) {
            statustxt.Insert(0, date + "-" + txt);
            TStatus.Lines = statustxt.ToArray();
        }
        private void UpdateStatusText(string sttxt) {
            if (!initialized) return;
            Func<int> stdel = delegate () {
                statustxt.Insert(0, DateTime.Now.ToShortTimeString()+" - "+sttxt);
                TStatus.Lines = statustxt.ToArray();
                return 0;
            };
            Invoke(stdel);
        }
    } // Class (DiscoverPort)
} // namespace (DiscoverIP)
