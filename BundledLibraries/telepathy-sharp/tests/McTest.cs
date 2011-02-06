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
using Telepathy.MissionControl;

namespace tests
{
    
    
    public class McTest
    {
        Bus bus = Bus.Session;
        bool running = true;
                
        const string MSG_PREFIX = "[McTest] ";
        
        public McTest()
        {
        }
    /*    
        private string TranslatePresence (McPresence p)
        {
            string translated = null;
            switch (p) {
                case McPresence.Available:
                    translated = "Available";
                    break;
                case McPresence.Away:
                    translated = "Away";
                    break;
                case McPresence.DoNotDisturb:
                    translated = "Do Not Disturb";
                    break;
                case McPresence.ExtendedAway:
                    translated = "Extended Away";
                    break;
                case McPresence.Hidden:
                    translated = "Hidden";
                    break;
                case McPresence.Offline:
                    translated = "Offline";
                    break;
                case McPresence.Unset:
                    translated = "Unset";
                    break;
            }
            
            return translated;
        }
        
        private string TranslateStatus (McStatus s)
        {
            string translated = null;
            
            switch (s) {
                case McStatus.Connected:
                    translated = "Connected";
                    break;
                case McStatus.Connecting:
                    translated = "Connecting";
                    break;
                case McStatus.Disconnected:
                    translated = "Disconnected";
                    break;
            }
            
            return translated;
        }
  */      
        public void Initialize ()
        {
            IMissionControl mc = bus.GetObject<IMissionControl> (Constants.MISSIONCONTROL_IFACE,
                new ObjectPath (Constants.MISSIONCONTROL_PATH));
            if (mc == null ) {
                Console.WriteLine (MSG_PREFIX + "Unable to find MissonControl interface.");
                return;
            }
         
            string[] conn;
            conn = mc.GetOnlineConnections ();
            
            for (int i = 0; i < conn.Length; i++) {
                McStatus conn_status = mc.GetConnectionStatus (conn[i]);
                                
                string bus_name;
                ObjectPath op;
                mc.GetConnection (conn[i], out bus_name, out op);
                
                string account = mc.GetAccountForConnection (op.ToString ());
                
                Console.WriteLine (MSG_PREFIX + "Account Name: {0}", conn[i]);
                Console.WriteLine (MSG_PREFIX + "Connection status: {0}", conn_status.ToString ());
                Console.WriteLine (MSG_PREFIX + "Object Path: {0}", op.ToString ());
                Console.WriteLine (MSG_PREFIX + "GetAccountForConnection: {0}", account);
            }
            
            McPresence presence = mc.GetPresence ();
            McPresence presence_actual = mc.GetPresenceActual ();
            string message = mc.GetPresenceMessage ();
            string message_actual = mc.GetPresenceMessageActual ();
            
            Console.WriteLine (MSG_PREFIX + "Presence Information");
            Console.WriteLine (MSG_PREFIX + "Presence: {0}", presence.ToString ());
            Console.WriteLine (MSG_PREFIX + "Actual Presence: {0}", presence_actual.ToString ());    
            Console.WriteLine (MSG_PREFIX + "Presence Message: {0}", message);
            Console.WriteLine (MSG_PREFIX + "Actual Presence Message: {0}", message_actual);
            
            mc.PresenceChanged += OnPresenceChanged;
            mc.AccountStatusChanged += OnAccountStatusChanged;
            
            Timer timer = new Timer ();
            timer.Interval = 30000;          // hang around for 10 seconds to test signals
            timer.Elapsed += new ElapsedEventHandler (Disconnect);
            timer.Start ();
            
            while (running) {
                bus.Iterate ();
            }
            
            Console.WriteLine (MSG_PREFIX + "Testing complete.");
        }
        
        private void OnPresenceChanged (McPresence p, string msg)
        {
            Console.WriteLine (MSG_PREFIX + "Presence Changed to {0}", p.ToString ());
            Console.WriteLine (MSG_PREFIX + "Presence Message Changed to {0}", msg);
        }
        
        private void Disconnect (object source, ElapsedEventArgs e)
        {
            running = false;
        }
        
        private void OnAccountStatusChanged (McStatus status, McPresence presence, 
                                             ConnectionStatusReason reason, string account_id)
        {
            Console.WriteLine (MSG_PREFIX + "OnAccountStatusChanged: status {0}, presence {1}, account {2}",
                               status.ToString (), presence.ToString (), account_id);
            //System.Threading.Thread.Sleep (5000);
            for (long i = 0; i < 1000000000; i++) { }
            Console.WriteLine (MSG_PREFIX + "Done looping");
        }
    }
}
