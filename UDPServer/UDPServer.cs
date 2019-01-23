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

namespace UDPServer
{
    public partial class UDPServer : Form {
        /*
        NOTES:
            - Startup automatically starts the UDP Broadcast responder "udpBroadcastResponder()"
                - Has a 1 sec timeout on RX so this can be used to detect QUIT "quitServices"
            - Startup also starts a link monitor "StatusMonitor()"...
                - This updates the GUI UDP and TCP States (progress bar and state text)
            - Startup starts this Server side TCP service...
                - It must be started BEFORE the client side service
                - Can Stop and ReStart it  (Menu->IP Services->...)
            - Disconnect of client is detected by:
                - The Async Reciever "AcceptClient()" 
                    - Looks for 2 missing PING's (10 sec's)
                - And the monitor who sets the GUI Progress bar and state string
                    - Waits 5 seconds to be sure the link is down...
                - Both depend on the 5 second PING<->PONG TCP Msg...
        */
        // Local IP
        string ipAddrStr = "";
        int udpPort = 340; // The listening port...
        List<string> availIPAddresses; // Local WAN Ports
        public bool cnctd, initialized = false, quit;

        public UDPServer() {
            InitializeComponent();
            InitMyProgressBars();
            AddStatus("Server Started...");
            tPort.Text = udpPort.ToString();
            // Add events...
            tPort.MouseLeave += TPort_MouseLeave;
            cIPAddress.TextChanged +=CIPAddress_TextChanged;
            FindIPAddresses();
            AddStatus("IP Address set to: "+ipAddrStr);
            //StartServer(); // Start the 2 thread UDP Monitor
            //StartResponder(); // Start the single thread UDP monitor
            //StartTCPIPServer(); // Start the TCP Interface
            myMonitor = new Thread(new ThreadStart(StatusMonitor));
            myMonitor.Start(); // Track the UDP/TCP state
            tTCPMonitor.Text = "TCP/IP Link down...";
            cnctd=false;
            // May need a slight delay here...
            initialized = true;
        }

        // EVENTS:
        private void TPort_MouseLeave(object sender, EventArgs e) {
            //throw new NotImplementedException();
            try {
                int newport = Int32.Parse(tPort.Text);
                if (udpPort != newport) {
                    udpPort = newport;
                    AddStatus("UDP Port changed to " + udpPort.ToString()+" (IP="+ipAddrStr+')');
                }
            }
            catch (Exception) { }
        }
        private void cIPAddress_SelectedIndexChanged(object sender, EventArgs e) {
            ipAddrStr = cIPAddress.SelectedItem.ToString();
            AddStatus("IP Address set to: "+ipAddrStr);
        }
        private void CIPAddress_TextChanged(object sender, EventArgs e) {
            //throw new NotImplementedException();
            if (IsIPValid(cIPAddress.Text)) {
                ipAddrStr = cIPAddress.Text;
                AddStatus("IP Address changed to: "+ipAddrStr+" (port="+udpPort.ToString()+')');
            }
        }

        // MENUS:
        private void fmHome_Click(object sender, EventArgs e) {

        }
        private void fmExit_Click(object sender, EventArgs e) {
            StopUDPServer();
            quitMonitor = false;
            Environment.Exit(0);
        }
        private void ipStartServer_Click(object sender, EventArgs e) {
            StartServer();
        }
        private void ipStartUDPServer_Click(object sender, EventArgs e) {
            StartUDPServer();
        }
        private void ipStartResponder_Click(object sender, EventArgs e) {
            StartUDPResponder();
        }
        private void ipStartTCP_Click(object sender, EventArgs e) {
            StartTCPIPServer(); // Start the TCP Service on this thread...
            //StartTCPLink(); // Start the TCP Service on a separate thread...
        }
        private void ipTCPDiscon_Click(object sender, EventArgs e) {
            DisconnectClients();
        }
        private void ipStopTCP_Click(object sender, EventArgs e) {
            KillTCPIP();
        }
        private void ipStopUDPServer_Click(object sender, EventArgs e) {
            StopUDPServer();
        }
        private void ipStopAll_Click(object sender, EventArgs e) {
            StopServer();
        }
        private void vmIPInfo_Click(object sender, EventArgs e) {
            infotxt.Clear();
            FindIPAddresses();
            foreach (string ipAddr in availIPAddresses)
                infotxt.Add(ipAddr);
            tInfo.Lines = infotxt.ToArray();
        }
        private void hmAbout_Click(object sender, EventArgs e) {

        }

        // Hard conversion of a UINT to an IP Address string...
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

        // Link State Monitor
        static bool quitMonitor = false; // Flag to Shutdown the UDP Monitor
        Thread myMonitor = null;
        private void StatusMonitor() {
            int loopCnt = 0;
            int lastUDPCnt = -1;
            int lastTCPCnt = -1,nextPongLoopCnt=-1;
            quitMonitor = false;
            Thread.Sleep(1000);
            while (!quitMonitor) {
                UpdateSocketStates();
                if (quitServices) {
                    if (udpBroadcasterRunning||udpResponderRunning) {
                        UpdateUDPMonitor("UDP Shutting Down...");
                        IncUDPProgress(Color.Yellow);
                    } else {
                        if (lastUDPCnt > 0) {
                            UpdateUDPMonitor("UDP Stopped...");
                            IncUDPProgress(Color.Red);
                            lastUDPCnt = -1;
                        }
                    }
                    if ((lastTCPCnt>=0)||(tcpMsgCnt>=0)) {
                        UpdateTCPMonitor("TCP Stopped...");
                        IncTCPProgress(Color.Red);
                        lastTCPCnt = -1; tcpMsgCnt = -1;
                    }
                } else {
                    if (udpBroadcasterRunning && udpResponderRunning) {
                        if (lastUDPCnt < udpRxCnt) {
                            UpdateUDPMonitor("UDP Connected...");
                            lastUDPCnt = udpRxCnt;
                            IncUDPProgress(Color.Green);
                        } else {
                            UpdateUDPMonitor("UDP Waiting for client...");
                            IncUDPProgress(Color.Yellow);
                        }
                        if (lastUDPCnt>udpRxCnt) lastUDPCnt = udpRxCnt;
                    } else if (lastUDPCnt>=0) {
                            UpdateUDPMonitor("UDP Stopped...");
                            IncUDPProgress(Color.Red);
                            lastUDPCnt = -1;
                    }
                    if (tcpRunning) { 
                        if (lastTCPCnt<tcpMsgCnt) {
                            UpdateTCPMonitor("TCP Connected...");
                            lastTCPCnt = tcpMsgCnt;
                            nextPongLoopCnt = loopCnt+5; // Allow at least 5 seconds for the next Ping-Pong event...
                            IncTCPProgress(Color.Green);
                        } else {
                            if (_serverTCPSocket==null) {
                                if (lastTCPCnt >=0) {
                                    IncTCPProgress(Color.Red);
                                    lastTCPCnt = -1; tcpMsgCnt = -1;
                                    UpdateTCPMonitor("TCP Stopped...");
                                }
                            } else if (loopCnt > nextPongLoopCnt) {
                                if (tcpConnected && (loopCnt < nextPongLoopCnt+10)) {
                                    UpdateTCPMonitor("TCP Waiting...");
                                    IncTCPProgress(Color.Yellow);
                                }
                            } else IncTCPProgress(Color.Green);
                        }
                    } else if (lastTCPCnt >=0) {
                        IncTCPProgress(Color.Red);
                        lastTCPCnt = -1; tcpMsgCnt = -1;
                        UpdateTCPMonitor("TCP Stopped...");
                    }
                } // not quiting all IP traffic
                Thread.Sleep(1050);
                loopCnt++;
            }
        } // Report the state of the UDP services...
        public void UpdateSocketStates() {
            if (!initialized) return;
            Func<int> scktdel = delegate () {
                string newline = "No socket checks yet...";
                //infotxt.Insert(0, DateTime.Now.ToShortTimeString() + "-" + newline);
                //tInfo.Lines = infotxt.ToArray();
                return 0;
            };
            Invoke(scktdel);
        }
        private void FindIPAddresses() {
            cIPAddress.Items.Clear();
            availIPAddresses = new List<string>();
            Socket mySkt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int i = 0;
            foreach (NetworkInterface thisIF in NetworkInterface.GetAllNetworkInterfaces()) {
                if ((thisIF.OperationalStatus == OperationalStatus.Up)
                        && ((String.Compare(thisIF.Name, "Ethernet", true) == 0) || (String.Compare(thisIF.Name, "Wi-Fi", true) == 0))) {
                    foreach (UnicastIPAddressInformation thisAddr in thisIF.GetIPProperties().UnicastAddresses) {
                        byte[] ipAddr = thisAddr.Address.GetAddressBytes();
                        string ipAddressstr = ConvertIPAddrBytesToStr(ipAddr);
                        cIPAddress.Items.Add(ipAddressstr);
                        availIPAddresses.Add(ipAddressstr);
                        if ((cIPAddress.SelectedIndex<0)&&(String.Compare("Ethernet", thisIF.Name, true) == 0)) {
                            cIPAddress.SelectedIndex = i; // Select the FIRST Ethernet port...
                            ipAddrStr = ipAddressstr;
                            AddStatus("TCP Address set to " + ipAddrStr);
                        } // IF this is an Ethernet port
                        i++;
                    } // Foreach defined IP address...
                } // IF this is a TCP/IP port that is UP
            } // For every known network interface
            if ((cIPAddress.SelectedIndex<0)&&(availIPAddresses.Count>0)) {
                i=availIPAddresses.Count-1; // IFF there was no Ethernet WAN ports... use the last available address...
                ipAddrStr = availIPAddresses[i];
                cIPAddress.SelectedIndex = i;
                AddStatus("TCP Address set to " + ipAddrStr);
            }
            tAvailIPs.Lines = availIPAddresses.ToArray();
        } // Get local IP ports
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

        // UDP Server...
        /*
            Notes:
                - Using threads so can time events...
                    - Seems to work well when using separated UDP ports for RX and TX
                    - Broadcast on port x, listen on port x-1 respond on port x-2
                - Cannot figure out how to use a Dgram socket for both RX=OK and TX=Exception?
                - This Socket shows as Bound but not connected...
                - Leaving this socket up after physical client disconnect recoveres correctly...
            TODO:
                - Resolve socket RX<->TX
                - Start TCP/IP on it's own thread for each client
                - Detect Link Loss
        */ // Notes
        static bool quitServices = false; // Flag to Shutdown the UDP Server
        static byte[] _rxBuffer = new byte[1024];
        static string udpTXmsg = "";
        static int udpRxCnt;
        List<string> clientList = new List<string>();
        static bool sendSrvrPkt = false; // Flag between threads to trigger a responce (udpBroadcastMonitor + lclUDPServer)
        static bool udpBroadcasterRunning = false; // Broadcast Listener state
        static bool udpResponderRunning = false; // Broadcast Responder state
        Thread myUDPBroadcastRXer = null;
        Socket brdcstSocket = null;
        Thread myUDPResponder = null;
        Thread myUDPListener = null;
        private void StartServer() {
            //StartUDPServer();
            StartUDPResponder();
            StartTCPIPServer();
        } // Start BOTH the UDP and TCP services
        private void StopServer() {
            KillTCPIP();
            StopUDPServer();
        } // Stop BOTH the UDP and TCP services
        private void StartUDPServer() {
            tInfo.Clear();
            infotxt.Clear();
            if (ipAddrStr.Length>7) {
                tClients.Text = "Starting Server";
                AddStatus("Starting the UPD Server ("+ipAddrStr.ToString()+")...");
                quitServices = false;
                bool initMyServer = (myUDPBroadcastRXer==null);
                if (!initMyServer)
                    if (myUDPBroadcastRXer.ThreadState != ThreadState.Running)
                        initMyServer = true;
                if (initMyServer) {
                    myUDPBroadcastRXer = new Thread(new ThreadStart(udpBroadcastMonitor));
                    myUDPBroadcastRXer.Start();
                    AddInfo("UDP Broadcast Listener started...");
                }
                bool initMyResponder = (myUDPResponder==null);
                if (!initMyResponder)
                    if (myUDPResponder.ThreadState != ThreadState.Running)
                        initMyResponder = true;
                if (initMyResponder) {
                    myUDPResponder = new Thread(new ThreadStart(lclUDPServer));
                    myUDPResponder.Start();
                    AddInfo("UDP Responder (local) started...");
                }
            } else
                AddInfo("First select an IP Address ["+ipAddrStr+"]...");
        } // Start the 2 thread UDP service (udpBroadcastMonitor + lclUDPServer)
        private void StopUDPServer() {
            quitServices = true;
            while (udpBroadcasterRunning) {
                AddInfo("Waiting for BroadCaster to stop...");
                Thread.Sleep(1000);
                //udpBroadcasterRunning = false;
            }
            AddInfo("BroadCaster stopped...");
            while (udpResponderRunning) {
                AddInfo("Waiting for Responder to stop...");
                Thread.Sleep(1000);
                //udpResponderRunning = false;
            }
            while (udpListenerRunning) {
                AddInfo("Waiting for Listener to stop...");
                Thread.Sleep(1000);
            }
            AddInfo("Responder stopped...");
            tClients.Text = "Server Stopped";
        } // Quit the UDP Service
        private void udpBroadcastMonitor() {
            // Listen for Broadcasted msg's and create a list of IP Address's
            byte[] rxData = new byte[1024];
            string rxDataStr;
            int pktSize, rxId = 1;
            udpRxCnt = 0;
            clientList.Clear();

            udpBroadcasterRunning = true;
            IPEndPoint myBoardCastEP = new IPEndPoint(IPAddress.Parse(ipAddrStr), udpPort);
            brdcstSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Setup a UDP DataGram socket.
            brdcstSocket.Bind(myBoardCastEP);   // Bind the RX endpoint to the port
            brdcstSocket.ReceiveTimeout = 1000; // mS? Not sure this works?

            // Just receive packets and set a flag (sendSrvrPkt) to respond via another task "lclUDPServer()"
            UpdateStatus("UDP Brdcst Monitor started on port " + udpPort.ToString() + "...");
            while (!quitServices) {
                rxId++;
                try {
                    // A Client will send "Client=<ip address>"
                    if (brdcstSocket.Available > 1) {
                        pktSize = brdcstSocket.Receive(rxData);
                        if (pktSize > 7) {
                            udpRxCnt++;
                            rxDataStr = Encoding.ASCII.GetString(rxData);
                            int id = rxDataStr.IndexOf('\0');
                            rxDataStr=rxDataStr.Substring(0, id);
                            if (String.Compare(rxDataStr, 0, "Client IP=", 0, 10, true)==0) {
                                string[] rxParts = rxDataStr.Split('=');
                                if (rxParts.Count()>1) {
                                    UpdateClientList(rxParts[1]);
                                    sendSrvrPkt = true; // Should we use a semaphor...
                                }
                            } else
                                UpdateInfo(udpRxCnt.ToString()+") RX'd [" + pktSize.ToString() + "] :" + rxDataStr);
                        } else UpdateInfo(rxId+") Nothing RX'd...");
                    }
                }
                catch (Exception) { }
                UpdateInfo(rxId+") Brdcst loop...");
                Thread.Sleep(1000);
            }
            brdcstSocket.Close();
            brdcstSocket.Dispose();
            udpBroadcasterRunning = false;
            UpdateInfo("UDP: Broadcast Monitor stopped...");
        } // Listen for UDP Broadcasts and request a UDP responce
        private void lclUDPServer() {
            udpResponderRunning = true;
            UdpClient myRspServer = new UdpClient(udpPort-1);
            //IPEndPoint rspEndPoint = new IPEndPoint(IPAddress.Parse(ipAddrStr), udpport-1); // On the clients Listening port...
            byte[] txPkt = Encoding.ASCII.GetBytes("Server="+ipAddrStr);
            UpdateStatus("UDP Server started to " + ipAddrStr + " on port " + (udpPort-1).ToString());
            sendSrvrPkt = false;
            // Wait for the request to send something (sendSrvrPkt) then send the fixed responce
            while (!quitServices) {
                if (sendSrvrPkt) {
                    //UpdateInfo("UDP Server responding to ("+clientList.Count+") clients ");
                    foreach (string clientIP in clientList) {
                        try { // Try to report to every known client...
                            myRspServer.Send(txPkt, txPkt.Length, new IPEndPoint(IPAddress.Parse(clientIP), udpPort-1));
                        }
                        catch (Exception) { }
                    }
                    sendSrvrPkt = false;
                }
                Thread.Sleep(10); // Wait a bit (semaphor,Mutex,AutoEvent,Lock?) then look for the trigger to send a pkt
            }
            myRspServer.Close();
            udpResponderRunning = false;
            UpdateInfo("UDP: Local Server stopped...");
        } // Send responce via UDP (port-1)
        private void StartUDPResponder() {
            tInfo.Clear();
            infotxt.Clear();
            if (ipAddrStr.Length>7) {
                if (myUDPBroadcastRXer!=null)
                    if (myUDPBroadcastRXer.ThreadState == ThreadState.Running)
                        StopUDPServer();
                tClients.Text = "Starting Responder";
                AddStatus("Starting the UPD Responder ("+ipAddrStr.ToString()+")...");
                quitServices = false;
                bool initMyResponder = (myUDPResponder==null);
                if (!initMyResponder)
                    if (myUDPResponder.ThreadState != ThreadState.Running)
                        initMyResponder = true;
                if (initMyResponder) {
                    myUDPResponder = new Thread(new ThreadStart(udpBroadcastResponder));
                    myUDPResponder.Start();
                    AddInfo("UDP Responder (Server) started...");
                }
                bool initMyListener = (myUDPListener==null);
                if (!initMyListener)
                    if (myUDPListener.ThreadState != ThreadState.Running)
                        initMyListener = true;
                if (initMyListener) {
                    myUDPListener = new Thread(new ThreadStart(udpHelloListener));
                    myUDPListener.Start();
                    AddInfo("UDP HELLO Listener started...");
                }
            } else
                AddInfo("First select an IP Address ["+ipAddrStr+"]...");
        } // Start the single thread UDP service (udpBroadcastResponder)
        private void udpBroadcastResponder() {
            byte[] rxData = new byte[1024];
            string rxDataStr;
            int pktSize, rxId = 1;
            udpRxCnt = 0;
            clientList.Clear();

            // Monitor
            udpBroadcasterRunning = true; udpResponderRunning=true;
            IPEndPoint myResponderEP = new IPEndPoint(IPAddress.Parse(ipAddrStr), udpPort);
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Setup a UDP DataGram socket.
            newSocket.Bind(myResponderEP);   // Bind the RX endpoint to the port
            newSocket.ReceiveTimeout = 1000; // mS?
            // Responder
            UdpClient myRspServer = new UdpClient(udpPort-1);
            byte[] txPkt = Encoding.ASCII.GetBytes("Server IP="+ipAddrStr);

            // Respond to a specific client pkt with a server msg on separate Ports...
            UpdateStatus("UDP Brdcst Monitor started on port " + udpPort.ToString() + "...");
            while (!quitServices) {
                rxId++;
                try {
                    // A Client will send "Client IP=<ip address>"
                    pktSize = newSocket.Receive(rxData);
                    if (pktSize > 7) {
                        udpRxCnt++;
                        rxDataStr = Encoding.ASCII.GetString(rxData);
                        //int id = rxDataStr.IndexOf('\0');
                        rxDataStr=rxDataStr.Substring(0, pktSize);
                        if (String.Compare(rxDataStr, 0, "Client IP=", 0, 10, true)==0) {
                            string[] rxParts = rxDataStr.Split('=');
                            if (rxParts.Count()>1)
                                if (IsIPValid(rxParts[1])) {
                                    UpdateClientList(rxParts[1]);
                                    try { // Try to report to every known client...
                                        //newSocket.Send(txPkt, txPkt.Length, SocketFlags.None); // Not Connected => Cannot RX and TX on same port?
                                        myRspServer.Send(txPkt, txPkt.Length, new IPEndPoint(IPAddress.Parse(rxParts[1]), udpPort-1));
                                        // TODO: How to start TCP service from inside this thread...
                                        //AsyncTcpServer();
                                        //startTCPIP = true;
                                    }
                                    catch (Exception) { }
                                }
                        } else
                            UpdateInfo(udpRxCnt.ToString()+") RX'd [" + pktSize.ToString() + "] :" + rxDataStr);
                    } else UpdateInfo(rxId+") Nothing RX'd...");
                }
                catch (Exception) { }
                // newSocket.Send(txPkt, SocketFlags.None); The socket is not connected => cannot send...
                /* Don't know how to detect H/W failure from this socket?
                if (!newSocket.Connected) {
                    UpdateInfo(rxId+") Brdcst loop failed...");
                    Thread.Sleep(1000);
                } */
            }
            newSocket.Close();
            newSocket.Dispose();
            myRspServer.Close();
            udpBroadcasterRunning = false; udpResponderRunning = false;
            UpdateInfo("UDP: Responder stopped...");
        } // Listen for broadcasts and respond with Server IP on separate ports...

        // Try to create a Bi-Directional UDP Link
        static int helloCnt;
        static bool udpListenerRunning = false; // Hello Responder state
        private void udpHelloListener() {
            byte[] rxData = new byte[1024];
            string rxDataStr;
            int pktSize, rxId = 0;
            helloCnt = 0;

            // Monitor
            udpListenerRunning=true;
            IPEndPoint myListenerEP = new IPEndPoint(IPAddress.Parse(ipAddrStr), udpPort-1);
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Setup a UDP DataGram socket.
            newSocket.Bind(myListenerEP);   // Bind the RX endpoint to the port
            newSocket.ReceiveTimeout = 1000; // mS?

            /* Responder cannot be on the same port/socket as the listener...  Why?
            UdpClient myRspServer = new UdpClient(udpport-1);
            */
            byte[] txPkt = Encoding.ASCII.GetBytes("Pong...");

            // Look for a "Hello..." and respond with a "Pong..."
            UpdateStatus("UDP HELLO Listener started on port " + (udpPort-1).ToString() + "...");
            while (!quitServices) {
                rxId++;
                try {
                    // A Client will send "Hello..."
                    pktSize = newSocket.Receive(rxData);
                    if (pktSize > 4) {
                        helloCnt++;
                        rxDataStr = Encoding.ASCII.GetString(rxData);
                        int id = rxDataStr.IndexOf('\0');
                        rxDataStr=rxDataStr.Substring(0, id);
                        if (String.Compare(rxDataStr, 0, "UDP Pong", 0, 5, true)==0) {
                            //
                            try { // Try to send a responce to Hello...
                                newSocket.Send(txPkt, txPkt.Length, SocketFlags.None);
                            }
                            catch (Exception) { UpdateInfo("UDP Pong failed..."); }
                            /*
                            try { myRspServer.Send(txPkt, txPkt.Length); } // Send the Pong...
                            catch (Exception) { UpdateInfo("Pong failed..."); }
                            */
                        } else
                            UpdateInfo(helloCnt.ToString()+") RX'd [" + pktSize.ToString() + "] :" + rxDataStr);
                    }
                }
                catch (Exception) { }
            }
            newSocket.Close();
            newSocket.Dispose();
            udpListenerRunning = false;
            UpdateInfo("UDP: HELLO Listener stopped...");
        } // Respond to Hello msgs with a Pong via UDP

        // ASYNC UDP handlers...
        List<string> judges = new List<string>();
        static Socket _serverUDPSocket = null;
        private void StartAsyncUDPServer() {
            //if (myServer != null) return;
            AddStatus("FAILED to start the ASYNC UPD Server...");
            /* UDP Is not supported in this form...
            if (_serverSocket != null) return;
            txmsg = "FCServer=" + ipAddrStr + ":" + udpport.ToString();
            UpdateInfo("Listening for incoming Clients on net " + ipAddrStr + " ...");
            _serverSocket = new Socket   // Create a local socket to listen on
                        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddrStr), udpport));
            _serverSocket.Listen(5); // Enable up to 5 backlogged (pending) connections 

            _serverSocket.BeginAccept(new AsyncCallback(AcceptClient), null); // Start listening for a connection
            */
        }
        private void AcceptUDPClient(IAsyncResult AR) {
            Socket mySocket = _serverUDPSocket.EndAccept(AR);  // Accept this connection
            _clientSockets.Add(mySocket);   // Add it to the list
            //UpdateStatus("    Client connected...");
            quitServices = false;
            while (!quitServices) {
                try {
                    mySocket.BeginReceive(_rxBuffer, 0, _rxBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), mySocket); // Call the function to RX Data
                }
                catch (SocketException) { }
                Thread.Sleep(100); // Wait a bit before trying to get more data
            }
            //AddStatus("    --------------------\n");
            _serverUDPSocket.BeginAccept(new AsyncCallback(AcceptUDPClient), null); // Go back to listen for another connection
        } // MyCallBack()
        private void UDPReceiveCallback(IAsyncResult AR) {
            Socket lclSocket = (Socket)AR.AsyncState;    // Create a socket to RX the data

            try {
                int rxCnt = lclSocket.EndReceive(AR);     // RX data and close the socket
                if (rxCnt > 0) {
                    byte[] dataBuf = new byte[rxCnt];           // Create a temp buffer to hold the new RX data
                    Array.Copy(_rxBuffer, dataBuf, rxCnt);      // Crop the new data to a temp buffer
                    string lclText = Encoding.ASCII.GetString(dataBuf); // Convert it to a string
                    if (String.Compare(lclText, 0, "FCJudge=", 0, 9, true) == 0) {
                        // Verify the judge's name? and respond with my info...
                        lclText = lclText.Remove(0, 9);
                        if (!judges.Contains(lclText)) UpdateJudges(lclText);
                        byte[] myStr = Encoding.ASCII.GetBytes(udpTXmsg.Length.ToString("D4") + udpTXmsg); // Convert the responce to a byte array with a 4 byte length at the front
                        lclSocket.BeginSend(myStr, 0, myStr.Length, SocketFlags.None, new AsyncCallback(SendCallback), lclSocket); // TX the responce to the client
                    }
                } // if RX'd data
            } // try to RX data
            catch (SocketException) {

            } // catch (errors)
            _serverUDPSocket.BeginAccept(new AsyncCallback(AcceptUDPClient), null); // Go back to listen for another connection
            // lclSocket.Close(); // This seems to cause an exception...
        } // ReceiveCallback()
        private void UDPSendCallback(IAsyncResult AR) {
            Socket lclSocket = (Socket)AR.AsyncState; // Define a local socket ref
            lclSocket.EndSend(AR);   // End the TX
        } // SendCallback()

        // TCP
        /*
            Notes:
                - Async RX<->TX work well IFF it is started before the Client wants to connect...
                    - Can only detect link loss by TX Exception so send first...
                    - Caused by state not changed until failed action
                        - RX waits without knowledge of link state changes...
                - Hangs on RX when link lost
            TODO:
                - Resolve link failure detection so teardown is clean
                - Make restart not require NEW...
                - Resolve re
        */ // Notes
        //static bool startTCPIP;
        static int tcpMsgCnt = 0; // The TCP Link keepalive Counter...
        static int tcpLoopCnt;
        static bool quitTCPServices = false; // Flag to Shutdown the TCP Server
        static bool tcpRunning = false; // Track the OVERALL state of the TCP Service
        static bool tcpConnected = false; // Track when we are talking to a client...
        private static byte[] _tcpBuffer = new byte[1024]; // Create a buffer to RX data
        static Socket _serverTCPSocket = null;
        static List<Socket> _clientSockets = new List<Socket>();
        private static List<bool> _clientCnctState = new List<bool>(); // Store the TCP Connect state for each client
        private void StartTCPIPServer() {
            //ChangeServerState("Starting async server...");
            tcpRunning = true;
            if (_serverTCPSocket == null) {
                AddStatus("TCP Server started on port "+ ipAddrStr);
                quitTCPServices = false;
                // Start up a TCP listener for client connections
                _serverTCPSocket = new Socket   // Create a local socket to listen on
                            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverTCPSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddrStr), udpPort));
                _serverTCPSocket.Listen(5); // Enable up to 5 backlogged (pending) connections 
                _serverTCPSocket.ReceiveTimeout = 1000; // 
                _serverTCPSocket.BeginAccept(new AsyncCallback(AcceptClient), null); // Start listening for a connection
            }
        } // Create the TCP Socket on the WAN port
        private void KillTCPIP() {
            quitTCPServices = true;
            KillClientSockets();
            _serverTCPSocket.Close();
            _serverTCPSocket.Dispose();
            _serverTCPSocket = null;
            tcpRunning = false;
        }
        private void KillClientSockets() {
            //quitTCPServices = true;
            if (_clientSockets.Count>0) {
                UpdateInfo("Killing all "+_clientSockets.Count+" Clients...");
                foreach (Socket mySkt in _clientSockets) {
                    mySkt.Close(); // Should we Disconnect?
                    //mySkt.Dispose();
                }
                _clientSockets.Clear();
                _clientCnctState.Clear();
            }
        } // Leave the TCP Server running... Just kill all clients...
        private void DisconnectClients() {
            if (_clientSockets.Count>0) {
                UpdateInfo("Disconnecting all "+_clientSockets.Count+" Clients...");
                for (int i = 0; i<_clientSockets.Count; i++) {
                    _clientSockets[i].Disconnect(true); // Allow them to be restarted...
                    _clientCnctState[i] = false;
                }
            }
        } // Leave the TCP Server running... Just kill all clients...
        private void AcceptClient(IAsyncResult AR) { // Static?
            int clientId = _clientSockets.Count;
            if (_serverTCPSocket != null) {
                Socket mySocket = _serverTCPSocket.EndAccept(AR);  // Accept this connection
                _clientSockets.Add(mySocket);   // Add it to the list
                _clientCnctState.Add(true);

                AddTCPText("Client connected via TCP/IP.");
                bool quitLoop = false;
                int msgLoopCnt = 0;
                tcpLoopCnt=0;
                while (!quitTCPServices && !quitLoop) {
                    if (mySocket.Available > 0) {
                        msgLoopCnt = tcpLoopCnt;
                        try {
                            mySocket.BeginReceive(_tcpBuffer, 0, _tcpBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), mySocket); // Call the function to RX Data
                            tcpConnected = true;
                        }
                        catch (SocketException) {
                            UpdateInfo("Socket exception... Quiting the ASYNC TCP/IP client Loop");
                            //if (!mySocket.Connected) _clientCnctState[clientId] = false;
                            // This restarts the listener accepting clients
                            quitLoop = true; // Should we quit TCP Services?
                        }
                        Thread.Sleep(100); // Wait a bit before trying to get more data
                    } else {
                        if (!mySocket.Connected) {
                            UpdateInfo("AcceptClient Socket Disconnected...");
                            quitLoop = true;
                        } else {
                            //UpdateInfo("TCP waiting...");
                            Thread.Sleep(1000); // Wait a bit before trying to get more data
                            if (tcpLoopCnt>msgLoopCnt+10) { // Should get a PING every 5 seconds...
                                // Quit this loop so the client can re-connect...
                                UpdateInfo("TCP link lost => closing it down...");
                                quitLoop = true; // We must have lost the link...
                            }
                        }
                    }
                    /* None of these seem to work as expected...
                    if ((clientId <0) ||(clientId >= _clientCnctState.Count)) quitLoop = true;
                    else quitLoop = !_clientCnctState[clientId];
                    */
                    /* if (!mySocket.Connected) {
                        tcpLoopCnt++;
                    } else tcpLoopCnt=0; */
                    /*
                    if (!_clientCnctState[clientId]||!mySocket.Connected) {
                        quitLoop = true; // Quit the loop when the Client requests a link close...
                    } */
                    tcpLoopCnt++;
                }
            }
            tcpConnected = false;
            //AddStatus("    --------------------\n");
            if (quitTCPServices) { // Quiting the server... Close all connections...
                AddTCPText("Closing down ALL connected clients...");
                foreach (Socket client in _clientSockets) {
                    client.Close();
                    client.Dispose();
                }
                _clientSockets.Clear();
                _clientCnctState.Clear();
            } else { // This client requested a quit... Close it down and go look for other clients
                if (_clientSockets.Count > clientId) {
                    _clientSockets[clientId].Close();
                    _clientSockets.RemoveAt(clientId);
                    _clientCnctState.RemoveAt(clientId);
                }
                AddTCPText("Closing down this socket... => Go Wait for another client... ");
                _serverTCPSocket.BeginAccept(new AsyncCallback(AcceptClient), null); // Go and listen for another connection
            }
        } // Try to reteive a pkt from the TCP link
        public void ReceiveCallback(IAsyncResult AR) { // Static?
            Socket lclSocket = (Socket)AR.AsyncState;    // Get the socket to RX the data
            int rxCnt = 0;
            string lclText, rspText;
            byte[] rxBuffer = new byte[200];
            int clientId = _clientSockets.IndexOf(lclSocket); // Find the index of the client...

            if (!lclSocket.Connected) {
                UpdateInfo("RcvCallBk socket Disconnected...");
                return;
            }
            try {
                rxCnt = lclSocket.EndReceive(AR);     // RX data and close the socket
                if (rxCnt > 0) {
                    //UpdateServerState("TCP-Client connected...");
                    byte[] dataBuf = new byte[rxCnt];               // Create a temp buffer to hold the new RX data
                    Array.Copy(_tcpBuffer, dataBuf, rxCnt);            // Copy the new data to the temp buffer
                    lclText = Encoding.ASCII.GetString(dataBuf);    // Convert it to a string
                    //AddTCPText("  RX'd data (" + lclText.Length.ToString() + ") : " + lclText);    // Update the server console
                    rspText = "Okay"; // The default responce...
                    tcpMsgCnt++; // Count ALL messages RX'd...
                    if ((String.Compare(lclText, "get time", true) == 0) || (String.Compare(lclText, "time", true) == 0)) {
                        // Send Server Timestamp
                        rspText = DateTime.Now.ToLongTimeString();
                    }
                    // Look for a Hello msg...
                    else if (String.Compare(lclText, 0, "Hello", 0, 5, true) == 0) {
                        // Verify the judge's name? and respond Welcome Msg
                        rspText = "Hello => Server is OK...";
                        AddTCPText("\"Hello\" msg detected.");
                    } else if (String.Compare(lclText, "Ping", true) == 0) {
                        AddTCPText("\"Ping\" msg detected.");
                        rspText = "Pong"; // Let the client know we are here...
                        //UpdateTCPProgress(true);
                    } else if (String.Compare(lclText, "quit", true) == 0) {
                        // Cleanup any unfinished business
                        AddTCPText("Client requested we \"QUIT\".");
                        rspText = "Quitting..."; // Let the client know we are closing down the connection...
                        _clientCnctState[clientId] = false; // Cause this port to stop listening...
                    } else {
                        rspText = "Invalid Request: [" + lclText + "]";    // Tell the client what request was ignored
                    }
                    byte[] myStr = Encoding.ASCII.GetBytes(rspText.Length.ToString("D4") + rspText); // Convert the responce to a byte array with a 4 byte length at the front
                    lclSocket.BeginSend(myStr, 0, myStr.Length, SocketFlags.None, new AsyncCallback(SendCallback), lclSocket); // TX the responce to the client
                } // if RX'd data
            } // try to RX data
            catch (SocketException) {

            } // catch (errors)
            //_serverSocket.BeginAccept(new AsyncCallback(AcceptClient), null); // Go back to listen for another connection
            // lclSocket.Close(); // This seems to cause an exception...
        } // Decode and respond to the Pkt
        private void SendCallback(IAsyncResult AR) { // Static?
            Socket lclSocket = (Socket)AR.AsyncState; // Define a local socket ref
            lclSocket.EndSend(AR);   // End the TX
        } // Create a thread to send a pkt to the Client
        // Threaded TCP Service
        /* This FAILS in the Callback Ftn "AcceptClient()" because socket = null...
            - Probably because the Callback Ftn's are not on the same thread as the Service? 
        static bool startTCP;
        Thread myTCPLink = null;
        public void StartTCPLink() {
            bool startTCP = (myTCPLink == null);
            if (!startTCP)
                startTCP = (myTCPLink.ThreadState != ThreadState.Running);
            if (startTCP) {
                myTCPLink = new Thread(new ThreadStart(TCPThread));
                myTCPLink.Start();
            }
        }
        private void TCPThread() {
            StartTCPIPServer();
            tcpRunning = true;
            while (tcpRunning) {
                if (quitTCPServices) KillTCPIP();
                Thread.Sleep(10);
            }
        }
        */

        // Progress bar
        static MyProgBar myUDPPrgBar,myTCPPrgBar;
        static Color progColor = Color.Yellow;
        static int udpPrgState = 0,tcpPrgState=0;
        public void InitMyProgressBars() {
            // UDP Status
            myUDPPrgBar = new MyProgBar();
            myUDPPrgBar.Name = "udpProgress";
            myUDPPrgBar.Location = new Point(10,290);
            myUDPPrgBar.Size = new Size(250, 25);
            myUDPPrgBar.ProgressColor = progColor;
            myUDPPrgBar.ProgressDirection = MyProgBar.ProgressDir.Horizontal;
            myUDPPrgBar.RoundedCorners = true;
            myUDPPrgBar.Text = "UDP State";
            myUDPPrgBar.ShowText = true;
            Controls.Add(myUDPPrgBar);
            myUDPPrgBar.Click += MyPrgBar_Click; // Normally comment this line out...
            myUDPPrgBar.Show(); // Don't show the bar until the UDP server is started...

            // TCP Status
            myTCPPrgBar = new MyProgBar();
            myTCPPrgBar.Name = "tcpProgress";
            myTCPPrgBar.Location = new Point(10, 315);
            myTCPPrgBar.Size = new Size(250, 25);
            myTCPPrgBar.ProgressColor = progColor;
            myTCPPrgBar.ProgressDirection = MyProgBar.ProgressDir.Horizontal;
            myTCPPrgBar.RoundedCorners = true;
            myTCPPrgBar.Text = "TCP/IP State";
            myTCPPrgBar.ShowText = true;
            Controls.Add(myTCPPrgBar);
            myTCPPrgBar.Show(); // Don't show the bar until the TCP server is started...
        } // Open and place the TCP/IP progress bar.
        private void MyPrgBar_Click(object sender, EventArgs e) {
            myUDPPrgBar.ProgressColor = Color.Gray;
        } // Change colour when selected...
        void IncUDPProgress() {
            udpPrgState++;
            if (udpPrgState>=100) udpPrgState = 0;
            myUDPPrgBar.Value = udpPrgState;
        }
        void IncUDPProgress(Color newColor) {
            Func<int> udpbardel = delegate () {
                myUDPPrgBar.ProgressColor = newColor;
                udpPrgState++;
                if (udpPrgState>=100) udpPrgState = 0;
                myUDPPrgBar.Value = udpPrgState;
                return 0;
            };
            Invoke(udpbardel);
        }
        void IncTCPProgress(Color newColor) {
            Func<int> tcpbardel = delegate () {
                myTCPPrgBar.ProgressColor = newColor;
                tcpPrgState++;
                if (tcpPrgState>=100) tcpPrgState = 0;
                myTCPPrgBar.Value = tcpPrgState;
                return 0;
            };
            Invoke(tcpbardel);
        }

        // Misc Ftn's
        public string TrimString(string txt) {
            return txt.Trim(' ', '\r', '\n');
        }
        public void UpdateJudges(string newjudge) {
            if (!initialized) return;
            /*
                Func<int> judgedel = delegate () {
                    if (judges.Contains(newjugde)) return 0;
                    AddStatus("new judge found at " + newjugde);
                    judges.Add(newjugde);
                    cJudges.Items.Add(newjugde);
                    return 0;
                };
                Invoke(judgedel);
            */
        }
        public void UpdateClientList(string clientIP) {
            if (!clientList.Contains(clientIP)) {
                UpdateStatus("New Client found at: " + clientIP);
                clientList.Add(clientIP);
                if (!initialized) return;
                Func<int> clientdel = delegate () {
                    tClients.Lines = clientList.ToArray();
                    return 0;
                };
                Invoke(clientdel);
            }
        }
        public void UpdateUDPMonitor(string newline) {
            if (!initialized) return;
            Func<int> monUDPdel = delegate () {
                tUDPMonitor.Text = newline;
                return 0;
            };
            Invoke(monUDPdel);
        }
        public void UpdateTCPMonitor(string newline) {
            if (!initialized) return;
            Func<int> monTCPdel = delegate () {
                tTCPMonitor.Text = newline;
                return 0;
            };
            Invoke(monTCPdel);
        }

        // Info Window Ftn's
        static List<string> infotxt = new List<string>();
        private void AddInfo(string newline) {
            infotxt.Insert(0, DateTime.Now.ToShortTimeString() + "-" + newline);
            tInfo.Lines = infotxt.ToArray<string>();
        }
        public void UpdateInfo(string newline) {
            if (!initialized) return;
            Func<int> infodel = delegate () {
                infotxt.Insert(0, DateTime.Now.ToShortTimeString() + "-" + newline);
                tInfo.Lines = infotxt.ToArray();
                return 0;
            };
            Invoke(infodel);
        }
        public void AddTCPText(string newline) {
            if (!initialized) return;
            Func<int> tcpTxtdel = delegate () {
                infotxt.Insert(0, DateTime.Now.ToShortTimeString() + "-" + newline);
                tInfo.Lines = infotxt.ToArray();
                return 0;
            };
            Invoke(tcpTxtdel);
        }
        // Log events in the staus window...
        List<string> statustext = new List<string>();
        private void AddStatus(string newline) {
            statustext.Insert(0, DateTime.Now.ToShortTimeString() + "-" + newline);
            tStatus.Lines = statustext.ToArray<string>();
        }
        private void AddStatus(string date, string newline) {
            statustext.Insert(0, date + "-" + newline);
            tStatus.Lines = statustext.ToArray<string>();
        }
        public void UpdateStatus(string newline) {
            if (!initialized) return;
            Func<int> stdel = delegate () {
                statustext.Insert(0, DateTime.Now.ToShortTimeString() + "-" + newline);
                tStatus.Lines = statustext.ToArray();
                return 0;
            };
            Invoke(stdel);
        }
    } // Class
} // Namespace
