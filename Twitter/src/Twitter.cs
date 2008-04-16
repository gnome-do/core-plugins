/* DoTwitter.cs
 *
 * This is a simple action for posting to twitter using GNOME DO.
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net;
using System.Xml;
using System.Threading;
using System.Collections.Generic;

using NDesk.DBus;
using org.freedesktop;
using org.freedesktop.DBus;

using GConf;
using Do.Universe;

namespace Twitter
{
    public static class Twitter
    {
		static List<IItem> items;
		static string username, password;
		private static Timer ClearContactsTimer;
		const int CacheSeconds  = 350; //cache contacts
		
		static Twitter () 
		{
			items = new List<IItem> ();
			SetUsername ();
			SetPassword ();
			ClearContactsTimer = new Timer (ClearContacts);
		}
		
		public static List<IItem> Friends {
			get { return items; }
		}
		
        public static string Username {
            get { return username; }
		}
		
		public static string Password {
			get { return password; } 
		}
        
		private static void SetUsername ()
		{
			GConf.Client gconf = new GConf.Client ();
            try {
                username = gconf.Get ("/apps/gnome-do/plugins/twitter/username") as string;
            } catch (NoSuchKeyException) {
                gconf.Set ("/apps/gnome-do/plugins/twitter/username",username);
                Twitter.SendNotification ("GConf keys created", "GConf keys for storing your Twitter "
                          + "login information have been created "
                          + "in /apps/gnome-do/plugins/twitter/\n"
                          + "Please set your username and password "
                          + "in order to post tweets");
            }
        }
        
        private static void SetPassword ()
		{
            GConf.Client gconf = new GConf.Client ();
            try {
                password = gconf.Get ("/apps/gnome-do/plugins/twitter/password") as string;
            } catch (NoSuchKeyException) {
                gconf.Set ("/apps/gnome-do/plugins/twitter/username",password);
                Twitter.SendNotification ("GConf keys created", "GConf keys for storing your Twitter "
                          + "login information have been created "
                          + "in /apps/gnome-do/plugins/twitter/\n"
                          + "Please set your username and password "
                          + "in order to post tweets");
            }
        }
        
        public static void GetTwitterFriends () 
		{
			if (!Monitor.TryEnter (items)) return;
			ClearContactsTimer.Change (CacheSeconds*1000, Timeout.Infinite);
			if (items.Count > 0) return;
			
            XmlDocument friends = new XmlDocument ();
            string url = "http://twitter.com/statuses/friends.xml";

            HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
            request.Credentials = new NetworkCredential (Twitter.Username,Twitter.Password);
            if(!SetRequestProxy (request, new GConf.Client ())) return;
			try {
				HttpWebResponse response = (HttpWebResponse) request.GetResponse ();
				friends.Load (response.GetResponseStream ());
			} catch (WebException e) {
				Console.Error.WriteLine (e);
				return;
			}
			
            string screen_name, name;
            screen_name = name = "";
            foreach (XmlNode user_node in friends.GetElementsByTagName ("user")) {
                foreach (XmlNode attr in user_node.ChildNodes) {
                    switch(attr.Name) {
                    case("screen_name"):
                        screen_name = attr.InnerText; break;
                    case("name"):
                        name = attr.InnerText; break;
                    }
                }
                ContactItem twit_friend_by_name = ContactItem.Create(name);
                twit_friend_by_name["twitter.screenname"] = "@" + screen_name;
				//Console.Error.WriteLine (twit_friend_by_name["twitter.screenname"]);
                ContactItem twit_friend_by_screenname = ContactItem.Create(screen_name);
                twit_friend_by_screenname["twitter.screename"] = "@" + screen_name;
				//Console.Error.WriteLine (twit_friend_by_screenname["twitter.screenname"]);
                items.Add (twit_friend_by_name);
                items.Add (twit_friend_by_screenname);
            }
			Monitor.Exit (items);
        }
		
		private static void ClearContacts (object state)
		{
			lock (items) {
				items.Clear ();
				Console.Error.WriteLine ("Contacts Cleared " + items.Count);
			}		
		}
        
        public static bool SetRequestProxy (HttpWebRequest request, GConf.Client gconf)
        {
            bool use_proxy;
            try {
                    use_proxy = (bool) gconf.Get ("/system/http_proxy/use_http_proxy");
            }
            catch (GConf.NoSuchKeyException) {
                    use_proxy = false;
            }

            if (use_proxy) {
                string phost;
                int pport;
                string[] pignore;
                bool pauth;                
                try {
                    phost = gconf.Get ("/system/http_proxy/host") as String;
                    pport = (int) gconf.Get ("/system/http_proxy/port");
                    pignore = gconf.Get ("/system/http_proxy/ignore_hosts") as String[];
                    pauth = (bool) gconf.Get ("/system/http_proxy/use_authentication");                
                }
                catch (GConf.NoSuchKeyException) {
                    Twitter.SendNotification ("Unable to load proxy settings", 
                                  "You have specified in GConf that "
                                + "you are using a proxy server, but no proxy "
                                + "settings could be found. Please check "
                                + "/system/http_proxy/ to make sure the "
                                + "appropriate values are set.");
                    return false;
                }
                WebProxy proxy = new WebProxy (phost, pport);
                proxy.BypassList = pignore;
                if (pauth) {
                    String pauser, papass;
                    try {
                        pauser = gconf.Get ("/system/http_proxy/authentication_user") as String;
                        papass = gconf.Get ("/system/http_proxy/authentication_password") as String;
                    }
                    catch (GConf.NoSuchKeyException) {
                        Twitter.SendNotification ("Unable to load proxy settings",
                                      "You have specified in GConf that "
                                    + "you are using a proxy server, but no proxy "
                                    + "settings could be found. Please check "
                                    + "/system/http_proxy/ to make sure the "
                                    + "appropriate values are set.");
                        return false;
                    }
                    proxy.Credentials = new NetworkCredential (pauser, papass);
                }
                request.Proxy = proxy;
            }
            return true;
        }
        
        public static void SendNotification (string title, string message)
        {
            Bus bus = Bus.Session;
            Notifications nf = bus.GetObject<Notifications> ("org.freedesktop.Notifications", new ObjectPath ("/org/freedesktop/Notifications"));
            Dictionary <string,object> hints = new Dictionary <string,object> ();
            nf.Notify (title, 0, "", title, message, new string[0], hints, -1);
        }
    }
}
