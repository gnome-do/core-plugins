using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Telepathy;
using Telepathy.Draft;
using Telepathy.MissionControl;

using GLib;
using Gtk;

using Mono.Unix;

using DBus;
using org.freedesktop.DBus;

namespace tests
{
    public class FileTransfer
    {
        private const string MSG_PREFIX = "[FileTransfer] ";
        
        private string account;                     // account, such as jabber0, msn1, etc.
        private uint self_handle;                   // your contact's handle
        private string transfer_partner;
        private uint transfer_partner_handle = 0;       // will get set if partner is online
        private uint transfer_initiator = 0;                // initiator is the client of remote interface
        
        private Bus bus = Bus.Session;
        private IConnection iconn = null;           // Telepathy Connection interface
        private IRequests irequests = null;          // New IRequests interface
        private IFileTransfer ift = null;             // Telepathy DBusTube channel
        private System.Net.Sockets.Socket s = null;    // socket for file transfer
        private ulong file_size;
        private string file_path = "/home/neil/commands.txt";
        private string socket_addr = null;
        
        private bool running = true;                // run loop?
        //private ITubeHandler tube_handler;          // tells Empathy we're handling the tube
        
        public FileTransfer ()
        {
            account = null;
            transfer_partner = null;
        }
        
        public FileTransfer (string account, string partner)
        {
            // passing an account name, such as jabber0, msn1, etc.
            // passing a transfer_partner, such as neil.loknath@jabber.org, etc.
            this.account = account;
            if (partner != null)
                this.transfer_partner = partner;

        }
        
        public void Initialize ()
        {
            BusG.Init ();
            Application.Init ();        // init GTK app for use of Glib mainloop
            
            IMissionControl mc = bus.GetObject<IMissionControl> (Constants.MISSIONCONTROL_IFACE,
                new ObjectPath (Constants.MISSIONCONTROL_PATH));
            if (mc == null ) {
                Console.WriteLine (MSG_PREFIX + "Unable to find MissonControl interface.");
                return;
            }
                     
            string bus_name = null;
            ObjectPath op = null;
            McStatus status = mc.GetConnectionStatus (account);
            
            if (status == McStatus.Connected) {
                mc.GetConnection (account, out bus_name, out op);
                iconn = bus.GetObject<IConnection> (Constants.CONNMANAGER_GABBLE_IFACE, op); // get connection
                if (iconn == null) {
                    running = false;
                }
                else {
                    Console.WriteLine (MSG_PREFIX + "Got connection using account {0}, bus {1}, path {2}"
                                       , account, bus_name, op);
                    //iconn.NewChannel += OnNewChannel;     // deprecated as of 0.17.23
                    irequests = bus.GetObject<IRequests> (bus_name, op);
                    irequests.NewChannels += OnNewChannels;
                    irequests.ChannelClosed += OnChannelClosed;     // don't really need this
                    self_handle = iconn.SelfHandle;
                    Console.WriteLine (MSG_PREFIX + "Your handle is {0}", self_handle);
                    /*            
                    SetTubeCapability (bus_name, op);         // tell Telepathy about our special tube
                    string service = DTUBE_HANDLER_IFACE + "." + DTUBETEST_IFACE;
                    string service_path = DTUBE_HANDLER_PATH + DTUBETEST_PATH;
                    if (tube_partner == null)
                        ClaimTubeName (service, service_path);
                    */
                    ProcessContacts (bus_name, op);
                    
                    if (transfer_partner_handle > 0) {
                        SetupFileTransfer ();
                    }
                    else if (transfer_partner != null) {
                        Console.WriteLine (MSG_PREFIX + "Our tube partner is missing. Quiting.");
                        running = false;
                    }
                                                                       
                }
            }
            
            if (running) {
                Application.Run ();             // run Glib mainloop
            }
                       
            Console.WriteLine (MSG_PREFIX + "Test complete.");
        }
        
        private void ProcessContacts (string bus_name, ObjectPath op)
        {
            
            if (transfer_partner == null) {
                Console.WriteLine (MSG_PREFIX + "No transfer_partner. Will wait for file.");
                return;
            }
                                    
            // the easy way
            string[] tmp = {transfer_partner};
            uint[] tmp_handles = iconn.RequestHandles (HandleType.Contact, tmp);
            Console.WriteLine (MSG_PREFIX + "Handle for {0} the easy way: {1}", transfer_partner, tmp_handles[0]);
            
            ISimplePresence ipresence = bus.GetObject<ISimplePresence> (bus_name, op);
            IDictionary <uint, SimplePresence> presences = ipresence.GetPresences (tmp_handles);
            Console.WriteLine (MSG_PREFIX + "{0} is {1}", transfer_partner, presences[tmp_handles[0]].Type.ToString ());
            
            if (presences[tmp_handles[0]].Type == ConnectionPresenceType.Available) {
                transfer_partner_handle = tmp_handles[0];
                Console.WriteLine (MSG_PREFIX + "{0} is {1}", transfer_partner, presences[tmp_handles[0]].Type.ToString ());
            }
            /*
            // ensure contact can handle the type of tube 
            if (tube_partner_handle > 0 && !IsContactCapable (bus_name, op, tube_partner_handle)) {
                Console.WriteLine ("{0} is not capable of handling the test tube.", tube_partner);
                tube_partner_handle = 0;
            }
            */          
        }
        
        private void SetupFileTransfer ()
        {
            FileInfo f = new FileInfo (file_path);
            ulong file_length = (ulong) f.Length;
            
            // use new Requests interface to create channel.
            // Connection.RequestChannel is deprecated
            IDictionary<string, object> channel_specs = new Dictionary<string, object>();
            channel_specs.Add (Constants.CHANNEL_IFACE + ".ChannelType", Constants.CHANNEL_TYPE_FILETRANSFER);
            channel_specs.Add (Constants.CHANNEL_IFACE + ".TargetHandleType", HandleType.Contact);
            channel_specs.Add (Constants.CHANNEL_IFACE + ".TargetID", transfer_partner);
            channel_specs.Add (Constants.CHANNEL_TYPE_FILETRANSFER + ".ContentType", "application/octet-stream");
            channel_specs.Add (Constants.CHANNEL_TYPE_FILETRANSFER + ".Filename", file_path);
            channel_specs.Add (Constants.CHANNEL_TYPE_FILETRANSFER + ".Size", file_length);
                        
            ObjectPath chann_op;
            IDictionary<string, object> chann_props;
            
            try {
                irequests.CreateChannel (channel_specs, out chann_op, out chann_props);
            }
            catch (Exception e) {
                Console.WriteLine (MSG_PREFIX + e);
            }
        }
        
        private void OnChannelClosed (ObjectPath object_path)
        {
            Console.WriteLine (MSG_PREFIX + "OnChannelClosed: object_path {0}",
                                   object_path);
        }
        
        private void OnNewChannels (ChannelDetails[] channels)
        {
            foreach (ChannelDetails c in channels) {
                
                ObjectPath object_path = c.Channel;
                string channel_type = c.Properties[Constants.CHANNEL_IFACE + ".ChannelType"] as string;
                HandleType handle_type = (HandleType) c.Properties[Constants.CHANNEL_IFACE + ".TargetHandleType"];
                uint handle = (uint) c.Properties[Constants.CHANNEL_IFACE + ".TargetHandle"];
                
                Console.WriteLine (MSG_PREFIX + "OnNewChannels: op {0}, channel_type {1}, handle_type {2}, handle {3}",
                                   object_path, channel_type, handle_type, handle);
                
                if (channel_type.Equals (Constants.CHANNEL_TYPE_FILETRANSFER)) {
                    ift = bus.GetObject<IFileTransfer> (Constants.CONNMANAGER_GABBLE_IFACE, object_path);
                    ift.FileTransferStateChanged += OnFileTransferStateChanged;
                    ift.Closed += OnFileTransferClosed;
                    ift.InitialOffsetDefined += OnInitialOffsetDefined;
                    ift.TransferredBytesChanged += OnTransferredBytesChanged;
                          
                    // get tube initiator handle
                    // should be = to self_handle on client
                    // should be != to self_handle on server
                    Properties p = bus.GetObject<Properties> (Constants.CONNMANAGER_GABBLE_IFACE, object_path);
                    transfer_initiator = (uint) p.Get (Constants.CHANNEL_IFACE, "InitiatorHandle");
                    IDictionary <uint, uint[]> supported_sockets = 
                        (IDictionary <uint, uint[]>) 
                            p.Get (Constants.CHANNEL_TYPE_FILETRANSFER, "AvailableSocketTypes");
                    file_size = (ulong) p.Get (Constants.CHANNEL_TYPE_FILETRANSFER, "Size");
                    
                    SocketAddressType socket_type = SocketAddressType.Unix;
                    SocketAccessControl socket_ac = SocketAccessControl.Localhost;
                    bool supported = false;
                    
                    if (supported_sockets.ContainsKey ((uint)SocketAddressType.Unix)) {
                       supported = true;
                    }
                    else if (supported_sockets.ContainsKey ((uint)SocketAddressType.IPv4)) {
                        socket_type = SocketAddressType.IPv4;
                        supported = true;
                    }
                    
                    if (supported && transfer_initiator == self_handle) {
                        Console.WriteLine (MSG_PREFIX + "Offering File");
                        object addr = ift.ProvideFile (socket_type, socket_ac, "");
                        if (socket_type == SocketAddressType.Unix) {
                            Console.WriteLine (MSG_PREFIX + "Boxed object is {0}", addr.ToString ());
                            //IPv4Address socket_addr = (IPv4Address)Convert.ChangeType (addr, typeof (IPv4Address));
                            socket_addr = Encoding.ASCII.GetString ((byte[]) addr);
                            Console.WriteLine (MSG_PREFIX + "File from {0} offered", socket_addr );
                            
                        }
                    
                    
                    }
                    else if (supported && transfer_initiator != self_handle) {
                        object addr = ift.AcceptFile (socket_type, socket_ac, "", 0);
                        if (socket_type == SocketAddressType.Unix) {
                            Console.WriteLine (MSG_PREFIX + "Boxed object is {0}", addr.ToString ());
                            //IPv4Address socket_addr = (IPv4Address)Convert.ChangeType (addr, typeof (IPv4Address));
                            socket_addr = Encoding.ASCII.GetString ((byte[]) addr);
                            Console.WriteLine (MSG_PREFIX + "File from {0} accepted", socket_addr);
                            
                        }
                    }
                    else {
                        Console.WriteLine (MSG_PREFIX + "Could not find supported socket type.");
                        ift.Close ();
                    }
                    //ift.Close ();    
                }
            }
            
        }
        
        private void OnTransferredBytesChanged (ulong bytes)
        {
            Console.WriteLine (MSG_PREFIX + "OnTransferredBytesChanged: bytes {0}", bytes);    
        }
        
        private void OnInitialOffsetDefined (ulong offset)
        {
            Console.WriteLine (MSG_PREFIX + "OnInitialOffsetDefined: offset {0}", offset);
        }
        
        private void OnFileTransferClosed ()
        {
            Console.WriteLine (MSG_PREFIX + "OnFileTransferClosed: no args");
            Console.WriteLine (MSG_PREFIX + "File transfer is being closed, so let's quit.");
            Application.Quit ();
        }
        
        private void OnFileTransferStateChanged (FileTransferState state, FileTransferStateChangeReason reason)
        {
            Console.WriteLine (MSG_PREFIX + "OnFileTransferStateChanged: state {0}, reason {1}",
                               state.ToString (), reason.ToString ());
            
            switch (state) {
                case FileTransferState.Open:
                    s = new System.Net.Sockets.Socket (AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                    EndPoint ep = new UnixEndPoint (socket_addr);
                    s.Connect (ep);
                    if (transfer_initiator == self_handle) {
                        Console.WriteLine (MSG_PREFIX + "Sending file");
                        ThreadPool.QueueUserWorkItem ( delegate { SendFile (); } );
                    }
                    else {
                        Console.WriteLine (MSG_PREFIX + "Receiving file");
                        ThreadPool.QueueUserWorkItem ( delegate { ReceiveFile (); } );
                    }
                    break;
                
                case FileTransferState.Completed:
                    Console.WriteLine (MSG_PREFIX + "Transfer completed.");
                    ift.Close ();
                    break;
            }
            
        }
        
        private void SendFile ()
        {
            Console.WriteLine (MSG_PREFIX + "In sending thread...");
            FileStream fs = new FileStream (file_path, FileMode.Open, FileAccess.Read);
            byte [] data = new byte[1024];
            int read;
            int total = 0;
            while ( (read = fs.Read (data, 0, data.Length)) > 0) {
                s.Send (data, 0, read, SocketFlags.None );
                total += read;
            }
            Console.WriteLine (MSG_PREFIX + "Sent {0} bytes", total);
        }
        
        private void ReceiveFile ()
        {
            Console.WriteLine (MSG_PREFIX + "In receiving thread...");
            byte [] data = new byte[1024];
            int bytes_received;
            int total = 0;
            
            BinaryWriter bwriter = new BinaryWriter ( File.Open ("./downloaded.file", FileMode.Create));
            while ( (bytes_received = s.Receive (data, 0, data.Length, SocketFlags.None)) > 0) {
                bwriter.Write (data, 0, bytes_received);
                total += bytes_received;
            }
            bwriter.Close ();
            Console.WriteLine (MSG_PREFIX + "{0} bytes received", total);
        }
        
    }
    
    public struct IPv4Address
    {
        public string address;
        public uint port;
    }
    
}