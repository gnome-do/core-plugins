
using System;
using System.Collections.Generic;
using Telepathy;
using Telepathy.Draft;
using Telepathy.MissionControl;
using GLib;
using Gtk;
using DBus;
using org.freedesktop.DBus;

namespace tests
{
    public class DTubeTest
    {
                
        const string DTUBETEST_IFACE = "org.freedesktop.Telepathy.Test.DTubeTest";
        const string DTUBETEST_PATH = "/org/freedesktop/Telepathy/Test/DTubeTest";
        
        const string DTUBE_HANDLER_IFACE = "org.gnome.Empathy.DTubeHandler";
        const string DTUBE_HANDLER_PATH = "/org/gnome/Empathy/DTubeHandler";
        
        const string MSG_PREFIX = "[DTubeTest] ";
        
        private string account;                     // account, such as jabber0, msn1, etc.
        private uint self_handle;                   // your contact's handle
        private string tube_partner;
        private uint tube_partner_handle = 0;       // will get set if partner is online
        private uint tube_initiator = 0;                // initiator is the client of remote interface
        
        private Bus bus = Bus.Session;
        private IConnection iconn = null;           // Telepathy Connection interface
        private IRequests irequests = null;          // New IRequests interface
        private IDBusTube itube = null;             // Telepathy DBusTube channel
        private string addr = null;                 // local address for tube communication
        
        private Connection dbus_conn = null;        // private, unshared connection for tube
        private bool running = true;                // run loop?
        private IDTubeTest obj;                     // D-bus exportable object
        private ITubeHandler tube_handler;          // tells Empathy we're handling the tube
        
        public DTubeTest()
        {
            account = null;
            tube_partner = null;
        }
        
        public DTubeTest (string account, string tube_partner)
        {
            // passing an account name, such as jabber0, msn1, etc.
            // passing a tube_partner, such as neil.loknath@jabber.org, etc.
            this.account = account;
            if (tube_partner != null)
                this.tube_partner = tube_partner;
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
            string[] online = mc.GetOnlineConnections();
            
            McStatus status = McStatus.Disconnected;
            
            if (online.Length == 0) {
                Console.WriteLine (MSG_PREFIX + "There are 0 online connections.");
                    
            }
            else {
                status = mc.GetConnectionStatus (account);
                
            }
            
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
                                
                    SetTubeCapability (bus_name, op);         // tell Telepathy about our special tube
                    
                    string service = DTUBE_HANDLER_IFACE + "." + DTUBETEST_IFACE;
                    string service_path = DTUBE_HANDLER_PATH + DTUBETEST_PATH;
                    if (tube_partner == null)
                        ClaimTubeName (service, service_path);
                    
                    ProcessContacts (bus_name, op);
                    
                    if (tube_partner_handle > 0) {
                        SetupTube ();
                    }
                    else if (tube_partner != null) {
                        Console.WriteLine (MSG_PREFIX + "Our tube partner is missing. Quitting.");
                        running = false;
                    }
                                                                       
                }
            }
            else {
                Console.WriteLine (MSG_PREFIX + account + " not connected.");
                Console.WriteLine (MSG_PREFIX + "will wait for a connection to test MC");
                Console.WriteLine (MSG_PREFIX + "Press CTRL+C to quit.");
                mc.AccountStatusChanged += OnAccountStatusChanged;
                //running = false;
            }
            /*
            while (running) {
                bus.Iterate ();
                if (dbus_conn != null)
                    dbus_conn.Iterate ();       // loop private connection for tube
            }
            */
            
            if (running) {
                Application.Run ();             // run Glib mainloop
            }
                       
            if (tube_initiator > 0) {
                Cleanup ();
            }
            
            Console.WriteLine (MSG_PREFIX + "Test complete.");
        }
        
        private void Cleanup ()
        {
            Console.WriteLine (MSG_PREFIX + "Doing cleanup");
            
            //string service = DTUBE_HANDLER_IFACE + "." + DTUBETEST_IFACE;
            string service_path = DTUBE_HANDLER_PATH + DTUBETEST_PATH;
            
            if (tube_initiator != self_handle) {
                bus.Unregister (new ObjectPath (service_path));
                if (dbus_conn != null) {
                    dbus_conn.Unregister (new ObjectPath (DTUBETEST_PATH));
                }
            }
            
        }
            
        private void SetTubeCapability (string bus_name, ObjectPath op)
        {
            Console.WriteLine (MSG_PREFIX + "Setting Contact capabilities");
            
            IContactCapabilities icaps = bus.GetObject<IContactCapabilities> (bus_name, op);
            
            IDictionary<string, object> [] caps = new Dictionary<string, object>[1];
            caps[0] = new Dictionary<string, object> ();
            caps[0].Add ("org.freedesktop.Telepathy.Channel.TargetHandleType", HandleType.Contact);
            caps[0].Add ("org.freedesktop.Telepathy.Channel.ChannelType", Constants.CHANNEL_TYPE_DBUSTUBE);
            caps[0].Add (Constants.CHANNEL_TYPE_DBUSTUBE + ".ServiceName", DTUBETEST_IFACE);
            
            icaps.SetSelfCapabilities (caps);
        }
        
        private bool IsContactCapable (string bus_name, ObjectPath op, uint handle)
        {
            Console.WriteLine (MSG_PREFIX + "Getting Contact capabilities");
            
            uint[] handles = new uint[1];
            handles[0] = handle;
            
            IDictionary<uint, RequestableChannelClass[]> caps;
            IContactCapabilities icaps = bus.GetObject<IContactCapabilities> (bus_name, op);
            caps = icaps.GetContactCapabilities (handles);
                                     
            if (caps.ContainsKey (handle)) {
                RequestableChannelClass[] rccs = caps[handle];
                for (int i = 0; i < rccs.Length; i++) {
                    if (rccs[i].FixedProperties.ContainsKey (Constants.CHANNEL_TYPE_DBUSTUBE + ".ServiceName")  &&
                        rccs[i].FixedProperties[Constants.CHANNEL_TYPE_DBUSTUBE + ".ServiceName"].Equals (DTUBETEST_IFACE)) {
                            Console.WriteLine (MSG_PREFIX + "Contact supports service name {0}", DTUBETEST_IFACE);
                            return true;
                    }
                }
            }

            return false;
            
        }
        
        private void ProcessContacts (string bus_name, ObjectPath op)
        {
            ISimplePresence ipresence = bus.GetObject<ISimplePresence> (bus_name, op);
            ipresence.PresencesChanged += OnPresencesChanged; // handle contacts changing status
            
            uint[] contacts = GetContacts (bus_name);
            
            if (tube_partner == null) {
                Console.WriteLine (MSG_PREFIX + "No tube_partner. Will wait for tube offer.");
                return;
            }
            
            // the hard way
            tube_partner_handle = GetTubePartnerHandle (contacts, ipresence);
            Console.WriteLine (MSG_PREFIX + "Handle for {0} the hard way (if online only): {1}", tube_partner, tube_partner_handle);
            
            // the easy way
            string[] tmp = {tube_partner};
            uint[] tmp_handles = iconn.RequestHandles (HandleType.Contact, tmp);
            Console.WriteLine (MSG_PREFIX + "Handle for {0} the easy way: {1}", tube_partner, tmp_handles[0]);
            
            
            // ensure contact can handle the type of tube 
            if (tube_partner_handle > 0 && !IsContactCapable (bus_name, op, tube_partner_handle)) {
                Console.WriteLine ("{0} is not capable of handling the test tube.", tube_partner);
                tube_partner_handle = 0;
            }
                        
        }
        
        private void SetupTube ()
        {
            
            // use new Requests interface to create channel.
            // Connection.RequestChannel is deprecated
            IDictionary<string, object> channel_specs = new Dictionary<string, object>();
            channel_specs.Add (Constants.CHANNEL_IFACE + ".ChannelType", Constants.CHANNEL_TYPE_DBUSTUBE);
            channel_specs.Add (Constants.CHANNEL_IFACE + ".TargetHandleType", HandleType.Contact);
            channel_specs.Add (Constants.CHANNEL_IFACE + ".TargetID", tube_partner);
            channel_specs.Add (Constants.CHANNEL_TYPE_DBUSTUBE + ".ServiceName", DTUBETEST_IFACE);
                        
            ObjectPath chann_op;
            IDictionary<string, object> chann_props;
            
            try {
                irequests.CreateChannel (channel_specs, out chann_op, out chann_props);
            }
            catch (Exception e) {
                Console.WriteLine (MSG_PREFIX + e);
            }
        }
        
        private void ClaimTubeName (string service, string service_path)
        {
            Console.WriteLine (MSG_PREFIX + "Claiming tube.");
            if (bus.RequestName (service) == RequestNameReply.PrimaryOwner) {
                tube_handler = new EmpathyHandler ();
                bus.Register (new ObjectPath (service_path), tube_handler);
                //bus.Register (new ObjectPath ("/org/gnome/Empathy/TubeHandler"), tube_handler);
            }
            
        }
        
        private void RegisterDBusObject () 
        {
            ObjectPath obj_op = new ObjectPath (DTUBETEST_PATH);
           
            //if (bus.RequestName (DTUBETEST_IFACE) == RequestNameReply.PrimaryOwner) {
               //create a new instance of the object to be exported
            obj = new DBusTestObject ();
            dbus_conn.Register (obj_op, obj);
            
            Console.WriteLine (MSG_PREFIX + "D-bus object registered.");
                       
            /*    
            }
            else {
                Console.WriteLine (MSG_PREFIX + "Unable to register D-Bus object.");
                running = false;
            }
        */
        }
        
        private uint[] GetContacts (string bus_name)
        {
            // use new Requests interface to create channel.
            // Connection.RequestChannel is deprecated
            IDictionary<string, object> channel_specs = new Dictionary<string, object>();
            channel_specs.Add (Constants.CHANNEL_IFACE + ".ChannelType", Constants.CHANNEL_TYPE_CONTACTLIST);
            channel_specs.Add (Constants.CHANNEL_IFACE + ".TargetHandleType", HandleType.List);
            channel_specs.Add (Constants.CHANNEL_IFACE + ".TargetID", "subscribe");
            
            bool yours;
            ObjectPath chann_op = null;
            IDictionary<string, object> chann_props;
            
            try {
                irequests.EnsureChannel (channel_specs, out yours, out chann_op, out chann_props);
            }
            catch (Exception e) {
                Console.WriteLine (MSG_PREFIX + e);
            }
            
            IGroup contact_list = bus.GetObject<IGroup> (bus_name, chann_op); // use Group interface with Channel

            // get contacts
            uint[] contacts; //, local_pending, remote_pending;
            contacts = contact_list.Members;
            Console.WriteLine(MSG_PREFIX + "Got {0} Contacts!", contacts.Length);
            
            return contacts;
        }
         
        private uint GetTubePartnerHandle (uint[] contacts, ISimplePresence ipresence) 
        {    
            
            string[] member_names;
            member_names = iconn.InspectHandles (HandleType.Contact, contacts);
            
            // get status information for contacts
            IDictionary<uint,SimplePresence> presence_info = new Dictionary<uint,SimplePresence>();
            presence_info = ipresence.GetPresences (contacts);

            uint tp_handle = 0;
            
            for (int i = 0; i < contacts.Length; i++) {
                if (presence_info.ContainsKey(contacts[i])) {
                    Console.WriteLine(MSG_PREFIX + "Member: " + member_names[i]);
                    Console.WriteLine(MSG_PREFIX + "Presences Key: " + contacts[i].ToString());
                    Console.WriteLine(MSG_PREFIX + "Presences Status: " + presence_info[contacts[i]].Status);

                    if (member_names[i].Equals(tube_partner) && !presence_info[contacts[i]].Status.Equals("offline")) {
                        tp_handle = contacts[i]; // remember hardcoded handle so we can message later
                        Console.WriteLine (MSG_PREFIX + "Saving handle for tube partner {0}", tube_partner);
                    }
                }
            }
            
            return tp_handle;
           
        }
        
        private void OnAccountStatusChanged (McStatus status, McPresence presence, 
                                             ConnectionStatusReason reason, string account_id)
        {
            Console.WriteLine (MSG_PREFIX + "OnAccountStatusChanged: status {0}, presence {1}, reason {2}, " +
                                                "account_id {3}", status, presence, reason, account_id);
        }
        
        private void OnChannelClosed (ObjectPath object_path)
        {
            Console.WriteLine (MSG_PREFIX + "OnChannelClosed: object_path {0}",
                                   object_path);
        }
        
        private void OnTubeClosed ()
        {
            Console.WriteLine (MSG_PREFIX + "OnTubeClosed: no args");
            Console.WriteLine (MSG_PREFIX + "Tube is being closed, so let's quit.");
            Application.Quit ();
        }
        
        private void OnNewChannels (ChannelDetails[] channels)
        {
            foreach (ChannelDetails c in channels) {
                
                ObjectPath object_path = c.Channel;
                string channel_type = c.Properties["org.freedesktop.Telepathy.Channel.ChannelType"] as string;
                HandleType handle_type = (HandleType) c.Properties["org.freedesktop.Telepathy.Channel.TargetHandleType"];
                uint handle = (uint) c.Properties["org.freedesktop.Telepathy.Channel.TargetHandle"];
                
                Console.WriteLine (MSG_PREFIX + "OnNewChannels: op {0}, channel_type {1}, handle_type {2}, handle {3}",
                                   object_path, channel_type, handle_type, handle);
                
                if (channel_type.Equals (Constants.CHANNEL_TYPE_DBUSTUBE)) {
                    itube = bus.GetObject<IDBusTube> (Constants.CONNMANAGER_GABBLE_IFACE, object_path);
                    //itube.NewTube += OnNewTube;
                    itube.TubeChannelStateChanged += OnTubeChannelStateChanged;
                    itube.Closed += OnTubeClosed;
                          
                    // get tube initiator handle
                    // should be = to self_handle on client
                    // should be != to self_handle on server
                    Properties p = bus.GetObject<Properties> (Constants.CONNMANAGER_GABBLE_IFACE, object_path);
                    tube_initiator = (uint) p.Get (Constants.CHANNEL_IFACE, "InitiatorHandle");
                    
                    if (tube_initiator == self_handle) {
                        Console.WriteLine (MSG_PREFIX + "Offering DTube");
                        addr = itube.Offer (new Dictionary<string, object>(), SocketAccessControl.Localhost);
                        Console.WriteLine (MSG_PREFIX + "Tube from {0} offered", addr);
                    }
                    else {
                        addr = itube.Accept (SocketAccessControl.Localhost);
                        Console.WriteLine (MSG_PREFIX + "Tube from {0} accepted", addr);
                    }
                    
                }
            }
            
        }
        /*
        private void OnNewChannel (ObjectPath object_path, string channel_type, HandleType handle_type, 
                                   uint handle, bool suppress_handler)
        {
            Console.WriteLine (MSG_PREFIX + "OnNewChannel: op {0}, type {1}, handle {2}",
                               object_path, channel_type, handle);
            
            if (channel_type.Equals (CHANNEL_TYPE_DBUSTUBE)) {
                itube = bus.GetObject<IDBusTube> (CONNMANAGER_GABBLE_IFACE, object_path);
                //itube.NewTube += OnNewTube;
                itube.TubeChannelStateChanged += OnTubeChannelStateChanged;
                      
                // get tube initiator handle
                // should be = to self_handle on client
                // should be != to self_handle on server
                Properties p = bus.GetObject<Properties> (CONNMANAGER_GABBLE_IFACE, object_path);
                tube_initiator = (uint) p.Get (CHANNEL_IFACE, "InitiatorHandle");
                
                if (tube_initiator == self_handle) {
                    Console.WriteLine (MSG_PREFIX + "Offering DTube");
                    addr = itube.OfferDBusTube (new Dictionary<string, object>());
                    Console.WriteLine (MSG_PREFIX + "Tube from {0} offered", addr);
                }
                else {
                    addr = itube.AcceptDBusTube ();
                    Console.WriteLine (MSG_PREFIX + "Tube from {0} accepted", addr);
                }
                
            }
        }
        */
        /*
        private void OnNewTube (uint id, uint initiator, TubeType type, string service, 
                                IDictionary<string,object> parameters, TubeState state)
        {
            Console.WriteLine (MSG_PREFIX + "OnNewTube: id {0), initiator {1}, service {2}", 
                               id, initiator, service);
            switch (state) {
                case TubeState.LocalPending:
                    if (type == TubeType.DBus && initiator != self_handle && service.Equals (DTUBETEST_IFACE)) {
                        Console.WriteLine (MSG_PREFIX + "Accepting DTube");
                        itube.AcceptDBusTube (id);
                    }
                    break;
            }
                    
        }
        */
        private void OnTubeChannelStateChanged (TubeChannelState state)
        {
            Console.WriteLine (MSG_PREFIX + "OnTubeStateChanged: state {0}", 
                               state);
            
            switch (state) {
                // this state is never reached, so accepting OnNewChannel
                // leaving here just in case, however
                case TubeChannelState.LocalPending:
                    addr = itube.Accept (SocketAccessControl.Localhost);
                    Console.WriteLine (MSG_PREFIX + "Tube from {0} accepted", addr);
                    break;
                
                // tube ready. set up connection and export/get object
                case TubeChannelState.Open:
                 
                    dbus_conn = Connection.Open (addr);
                    BusG.Init (dbus_conn);
                                  
                    if (tube_initiator != self_handle)
                        RegisterDBusObject ();
                    else {
                        obj = dbus_conn.GetObject<IDTubeTest> (DTUBETEST_IFACE, new ObjectPath (DTUBETEST_PATH));
                        obj.TestSignal += OnTestSignal;
                        obj.Hello ();
                        IDictionary<string, object>[] dic = obj.HelloDictionary ();
                        Console.WriteLine (MSG_PREFIX + "Got response on tube. Dictionary array has {0} items.", dic.Length);
                        TestStruct[] ts = obj.HelloStruct ();
                        Console.WriteLine (MSG_PREFIX + "Got response on tube. Struct array has {0} items", ts.Length);
                        Properties p = dbus_conn.GetObject<Properties> (DTUBETEST_IFACE, new ObjectPath (DTUBETEST_PATH));
                        string property = (string) p.Get (DTUBETEST_IFACE, "TestProperty");
                        Console.WriteLine (MSG_PREFIX + "Got response on tube. Property {0}", property);
                        itube.Close ();
                    } 
                    break;
                
                case TubeChannelState.NotOffered:
                    break;
               
            }
            
        }
        
        private void OnTestSignal (string response)
        {
            Console.WriteLine (MSG_PREFIX + "OnTestSignal: Got response on tube: {0}", response);    
        }
        
        private void OnPresencesChanged (IDictionary<uint,SimplePresence> presence)
        {
            Console.WriteLine (MSG_PREFIX + "Contact presence changed. Handling event");
        
            uint[] handles = new uint[presence.Keys.Count];
            presence.Keys.CopyTo(handles, 0);
            string[] member_names;
        
            try {
                member_names = iconn.InspectHandles (HandleType.Contact, handles);
            }
            catch (Exception e) {
                Console.WriteLine (MSG_PREFIX + e);
                return;
            }
    
            for (int i = 0; i < handles.Length; i++) {
    
                if (presence.ContainsKey(handles[i])) {
                    Console.WriteLine(MSG_PREFIX + "Member {0} is status {1}", member_names[i], presence[handles[i]].Status);
                } // end if
    
            } // end for
        }
        
    }
    
    // Empathy specific interface to tell Empathy we are handling the tube
    [Interface ("org.gnome.Empathy.TubeHandler")]
    public interface ITubeHandler
    {
        void HandleTube (string bus_name, ObjectPath conn, ObjectPath channel, uint handle_type,
                         uint handle);
    }
    
    // define a test object to export on D-Bus
    [Interface ("org.freedesktop.Telepathy.Test.DTubeTest")]
    public interface IDTubeTest
    {
        void Hello ();
        IDictionary<string, object>[] HelloDictionary ();
        TestStruct[] HelloStruct ();   
        string TestProperty {
            get;
        }
     
        event TestSignalHandler TestSignal;
    }
    
    public delegate void TestSignalHandler (string message);
    
    public struct TestStruct
    {
        public string name;
        public string value;
        public string field1;
        public string field2;
        public string field3;
        public string field4;
        public string field5;
        public string field6;
        public string field7;
        public string field8;
        public string field9;
        public string field10;
    }
    
    public class EmpathyHandler : ITubeHandler
    {
        public void HandleTube (string bus_name, ObjectPath conn, ObjectPath channel, uint handle_type,
                         uint handle)
        {
            return;
        }
    }
    
    public class DBusTestObject : IDTubeTest, Properties
    {
        public event TestSignalHandler TestSignal;
        
        public void Hello ()
        {
            if (TestSignal != null) {
                TestSignal ("Congrats! You're talking on a tube!");
            }
        }
        
        public IDictionary<string, object>[] HelloDictionary () 
        {
            IDictionary<string, object>[] dic = new Dictionary<string, object>[1000];
            
            for (int i = 0; i < 1000; i++) {
                dic[i] = new Dictionary<string, object> ();
                dic[i].Add ("Name", "Test");
                dic[i].Add ("Value", "Testing, Testing, 123");
                dic[i].Add ("Field1", "Testing, Testing, 123");
                dic[i].Add ("Field2", "Testing, Testing, 123");
                dic[i].Add ("Field3", "Testing, Testing, 123");
                dic[i].Add ("Field4", "Testing, Testing, 123");
                dic[i].Add ("Field5", "Testing, Testing, 123");
                dic[i].Add ("Field6", "Testing, Testing, 123");
                dic[i].Add ("Field7", "Testing, Testing, 123");
                dic[i].Add ("Field8", "Testing, Testing, 123");
                dic[i].Add ("Field9", "Testing, Testing, 123");
                dic[i].Add ("Field10", "Testing, Testing, 123");
            }
            
            return dic;
        }
        
        public TestStruct[] HelloStruct ()
        {
            TestStruct[] ts = new TestStruct[1000];
            
            for (int i = 0; i < 1000; i++) {
                ts[i].name = "Test";
                ts[i].value = "Testing, testing, 123";
                ts[i].field1 = "Testing, testing, 123";
                ts[i].field2 = "Testing, testing, 123";
                ts[i].field3 = "Testing, testing, 123";
                ts[i].field4 = "Testing, testing, 123";
                ts[i].field5 = "Testing, testing, 123";
                ts[i].field6 = "Testing, testing, 123";
                ts[i].field7 = "Testing, testing, 123";
                ts[i].field8 = "Testing, testing, 123";
                ts[i].field9 = "Testing, testing, 123";
                ts[i].field10 = "Testing, testing, 123";
            }
            
            return ts;
        }
        
        public string TestProperty {
            get { return "This is a test property."; }
        }
        
        public IDictionary <string, object> GetAll (string iface)
        {
            IDictionary <string, object> all = new Dictionary <string, object> ();
            all.Add ("TestProperty", TestProperty);
            
            return all;
        }
        
        public void Set (string iface, string property, object value)
        {
        }
        
        public object Get (string iface, string property)
        {
            if (property.Equals ("TestProperty")) {
                return TestProperty;
            }
            else {
                return null;
            }
        }
        
    }
    
        
}
