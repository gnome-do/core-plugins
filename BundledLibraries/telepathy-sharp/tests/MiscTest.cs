/*
 *   Copyright (C) 2009 Neil Loknath <neil.loknath@gmail.com>
 * 
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published 
 *   by the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Lesser General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU LesserGeneral Public License as published 
 *   by the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 */

using System;
using System.Timers;
using System.Collections.Generic;
using DBus;
using Telepathy;

namespace tests
{
    
    class MiscTest
    {
        private IConnection iconn;
        private IText itext;
        ObjectPath chann_op;
        uint myhandle = 0;
        ObjectPath op;
        Bus bus = Bus.Session;
        bool running = true;
        string BusName;
    
        // accounts
        const string ACCOUNT_BANSHEE_TEST1 = "banshee_test1@jabber.org";
        const string ACCOUNT_BANSHEE_TEST1_PASSWORD = "banshee";
        const string ACCOUNT_BANSHEE_TEST2 = "banshee_test2@jabber.org";
        const string ACCOUNT_SERVER = "jabber.org";
        const string ACCOUNT_SERVER_PORT = "5222";
    
        const string MSG_PREFIX = "[MiscTest] ";
    
        public void sendText (string message)
        {
            if (myhandle == 0)
                return;
    
            // let's try to send an IM
            try {
                
                chann_op = iconn.RequestChannel (Constants.CHANNEL_TYPE_TEXT, HandleType.Contact, myhandle, true);
                if (itext == null) {
                    itext = bus.GetObject<IText> (BusName, chann_op); // use Text interface with Channel
                    itext.Sent += OnSent;
                }
                itext.Send (ChannelTextMessageType.Normal, message);
                
            }
            catch (Exception e) {
                Console.WriteLine (MSG_PREFIX + e);
                return;
            }
    
        }
    
        public void OnSent (uint timestamp, ChannelTextMessageType type, string text)
        {
            Console.WriteLine (MSG_PREFIX + "Message sent at {0}", timestamp.ToString());
        }
    
        public void OnPresencesChanged (IDictionary<uint,SimplePresence> presence)
        {
            Console.WriteLine (MSG_PREFIX + "Some contact presence changed...handling event");
        
            uint[] handles = new uint[presence.Keys.Count];
            presence.Keys.CopyTo(handles, 0);
            string[] members_str;
        
            try {
                members_str = iconn.InspectHandles (HandleType.Contact, handles);
            }
            catch (Exception e) {
                Console.WriteLine (MSG_PREFIX + e);
                return;
            }
    
            for (int i = 0; i < handles.Length; i++) {
    
                if (presence.ContainsKey(handles[i])) {
                    Console.WriteLine(MSG_PREFIX + "Member {0} is status {1}", members_str[i], presence[handles[i]].Status);
                
                    if (members_str[i].Equals(ACCOUNT_BANSHEE_TEST2) && !presence[handles[i]].Status.Equals("offine")) {
                        myhandle = handles[i];
                        Console.WriteLine (MSG_PREFIX + "Trying to send a message to {0}", ACCOUNT_BANSHEE_TEST2);
                        
    
                        // let's try to send an IM
                        try {
                            chann_op = iconn.RequestChannel (Constants.CHANNEL_TYPE_TEXT, HandleType.Contact, handles[i], true);
                            if (itext == null) {
                                itext = bus.GetObject<IText> (BusName, chann_op); // use Text interface with Channel
                                itext.Sent += OnSent;
                            }
                            itext.Send(ChannelTextMessageType.Normal, "Hey Buddy!");
                         }
                         catch (Exception e) {
                             Console.WriteLine (MSG_PREFIX + e);
                             return;
                         }
          
                     } // end if
        
                 } // end if
    
             } // end for
        
    
        }
    
        public void OnConnectionStateChanged (ConnectionStatus status, ConnectionStatusReason reason)
        {
            Console.WriteLine (MSG_PREFIX + "Connection state changed, Status: {0}, Reason: {1}", status, reason);
    
            if (status == ConnectionStatus.Connected) {
    
                try {
                    // show supported interfaces
                    string[] interfaces;
                    interfaces = iconn.GetInterfaces ();
    
                    foreach (string s in interfaces)
                        Console.WriteLine(MSG_PREFIX + s);
        
                    string[] args = {"subscribe"};
                    uint[] handles;
                    handles = iconn.RequestHandles (HandleType.List, args);
    
                    ISimplePresence ipresence = bus.GetObject<ISimplePresence> (BusName, op);
                    ipresence.PresencesChanged += OnPresencesChanged; // handle contacts changing status
    
                    ObjectPath chann_op;
                    chann_op = iconn.RequestChannel (Constants.CHANNEL_TYPE_CONTACTLIST, HandleType.List, handles[0], true);
                    
                    IGroup contact_list = bus.GetObject<IGroup> (BusName, chann_op); // use Group interface with Channel
    
                    // get contacts
                    uint[] contacts; //, local_pending, remote_pending;
                    //contact_list.GetAllMembers(out contacts, out local_pending, out remote_pending);
                    contacts = contact_list.Members;
                    Console.WriteLine(MSG_PREFIX + "Got {0} Contacts!", contacts.Length);
                    string[] members_str;
                    members_str = iconn.InspectHandles (HandleType.Contact, contacts);
    
                    // get status information for contacts
                    IDictionary<uint,SimplePresence> dic = new Dictionary<uint,SimplePresence>();
                    dic = ipresence.GetPresences (contacts);
    
                    for (int i = 0; i < contacts.Length; i++) {
                        if (dic.ContainsKey(contacts[i])) {
                            Console.WriteLine(MSG_PREFIX + "Member: " + members_str[i]);
                            Console.WriteLine(MSG_PREFIX + "Presences Key: " + contacts[i].ToString());
                            Console.WriteLine(MSG_PREFIX + "Presences Status: " + dic[contacts[i]].Status);
    
                            if (members_str[i].Equals(ACCOUNT_BANSHEE_TEST2) && !dic[contacts[i]].Status.Equals("offine"))
                                myhandle = contacts[i]; // remember hardcoded handle so we can message later
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine (MSG_PREFIX + e);
                }
           
           Timer timer = new Timer ();
           timer.Interval = 10000;          // delay logoff by 10 seconds
           timer.Elapsed += new ElapsedEventHandler (Disconnect);
           timer.Start ();
           
           }
    
           if (status == ConnectionStatus.Disconnected)
               running = false;
        }
    
        private void Disconnect (object source, ElapsedEventArgs e)
        {
            iconn.Disconnect (); // try disconnect no matter so to avoid orphaned connection 
        }
        
        public void Initialize()
        {
            //get connection manager from dbus
            IConnectionManager conn_manager = bus.GetObject<IConnectionManager> (Constants.CONNMANAGER_GABBLE_IFACE,
                new ObjectPath (Constants.CONNMANAGER_GABBLE_PATH));
            
            if (conn_manager == null) {
                Console.WriteLine (MSG_PREFIX + "Error on get cm");
                return;
            }
    
            Console.WriteLine (MSG_PREFIX + "Account: {0}\nPassowrd: ?\nServer: {1}\nPort: {2}", ACCOUNT_BANSHEE_TEST1,
                ACCOUNT_SERVER,UInt32.Parse (ACCOUNT_SERVER_PORT));
    
            IDictionary<string, object> option_list = new Dictionary<string, object>();
            // required parms for establishing connection
            option_list.Add ("account", ACCOUNT_BANSHEE_TEST1);
            option_list.Add ("password", ACCOUNT_BANSHEE_TEST1_PASSWORD);
            option_list.Add ("server", ACCOUNT_SERVER);
            option_list.Add ("port", (uint) UInt32.Parse (ACCOUNT_SERVER_PORT));
            //option_list.Add ("old-ssl", true);
            option_list.Add ("ignore-ssl-errors", true);
    
            try {
                conn_manager.RequestConnection ("jabber", option_list, out BusName, out op);
                if (op == null) {
                    Console.WriteLine(MSG_PREFIX + "Unable to get connection???");
                    return;
                }
            
                Console.WriteLine (MSG_PREFIX + "Bus Name: {0}\n Object Path: {1}",BusName, op.ToString());
            
            
                iconn = bus.GetObject<IConnection> (BusName, op); // get connection
                iconn.StatusChanged += OnConnectionStateChanged; // handle connection to jabber server events
                iconn.Connect ();
                
                while (running)
                    bus.Iterate ();
    
                Console.WriteLine (MSG_PREFIX + "Testing complete.");  
            }
            catch (Exception e) {
                Console.WriteLine(MSG_PREFIX + e);
                if (iconn != null)
                    iconn.Disconnect();
            }
        }
    } // end class MiscTest
}
