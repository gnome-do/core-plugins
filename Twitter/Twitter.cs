/* Twitter.cs
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
using System.Collections.Generic;
using System.IO;
using System.Net;

using Mono;
using Mono.Unix;

using GConf;

using NDesk.DBus;
using org.freedesktop;
using org.freedesktop.DBus;

using Do.Universe;

namespace Do.Twitter
{

        public class Twitter : IAction
        {
                
                public string Name {
                        get {
                                return "Tweet";
                        }
                }
                
                public string Description {
                        get {
                                return "Update Twitter Status";
                        }
                }
                
                public string Icon {
                        get {
                                return "twitter-icon.png@Twitter";
                        }
                }
                
                public Type[] SupportedItemTypes {
                        get {
                                return new Type[] {
                                        typeof (ITextItem),
                                };
                        }
                }

                public bool SupportsItem (IItem item) 
                {

                        return true;

                }




                public IItem[] Perform (IItem[] items, IItem[] modItems)
                {

                        String tweet = EscapeTweet ((items [0] as ITextItem).Text);

                        String url = "http://twitter.com/statuses/update.json?status=" + tweet;
                        HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;

                        request.Method = "POST";

                        GConf.Client gconf = new GConf.Client ();

                        if (!SetRequestCredentials (request, gconf)) return null;
                        if (!SetRequestProxy (request, gconf)) return null;
                        
                        try {

                                request.GetResponse ();
                                SendNotification ("Tweet Successful", "Successfully posted tweet '" 
                                                + (items [0] as ITextItem).Text
                                                + "' to Twitter.");


                        } catch (Exception e) {

                                Console.WriteLine (e.ToString ());
                                SendNotification ("Tweet Failed", "Unable to post tweet. Check your login "
                                                + "settings (/apps/gnome-do/plugins/twitter). If you are "
                                                + "behind a proxy, also make sure that the settings in "
                                                + "/system/http_proxy are correct.\n\nDetails:\n" 
                                                + e.ToString ());
                        }
                        
                        return null;
                }
                
                public Type[] SupportedModifierItemTypes {
                        get { return new Type[] {}; }
                }

                public bool ModifierItemsOptional {
                        get { return true; }
                }
                                
                public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
                {
                        return true;
                }
                
                public IItem[] DynamicModifierItemsForItem (IItem item)
                {
                        return null;
                }
              





                private string EscapeTweet (string tweet) {

                        String retstring = tweet.Replace ("%", "%25")
                                                .Replace ("#", "%23")
                                                .Replace ("{", "%7B")
                                                .Replace ("}", "%7D")
                                                .Replace ("|", "%7C")
                                                .Replace ("\\", "%5C")
                                                .Replace ("^", "%5E")
                                                .Replace ("~", "%7E")
                                                .Replace ("[", "%5B")
                                                .Replace ("]", "%5D")
                                                .Replace ("`", "%60")
                                                .Replace (";", "%3B")
                                                .Replace (")", "%29")
                                                .Replace ("/", "%2F");

                               retstring = tweet.Replace ("(", "%28")
                                                .Replace ("?", "%3F")
                                                .Replace (":", "%3A")
                                                .Replace ("@", "%40")
                                                .Replace ("=", "%3D")
                                                .Replace ("&", "%26")
                                                .Replace ("$", "%24")
                                                .Replace ("\"", "%22")
                                                .Replace ("'", "%27")
                                                .Replace ("*", "%2A")
                                                .Replace ("+", "%2B")
                                                .Replace ("!", "%21")
                                                .Replace (" ", "+");

                        return retstring;

                }

                private bool SetRequestCredentials (HttpWebRequest request, GConf.Client gconf)
                {

                        String username;
                        String password;

                        try {
                                username = gconf.Get ("/apps/gnome-do/plugins/twitter/username") as String;
                                password = gconf.Get ("/apps/gnome-do/plugins/twitter/password") as String;
                        }
                        catch (GConf.NoSuchKeyException) {
                                gconf.Set ("/apps/gnome-do/plugins/twitter/username", "");
                                gconf.Set ("/apps/gnome-do/plugins/twitter/password", "");
                                SendNotification ("GConf keys created", "GConf keys for storing your Twitter "
                                                                      + "login information have been created "
                                                                      + "in /apps/gnome-do/plugins/twitter/\n"
                                                                      + "Please set your username and password "
                                                                      + "in order to post tweets");
                                return false;
                        }

                        request.Credentials = new NetworkCredential (username, password);

                        return true;

                }

                private bool SetRequestProxy (HttpWebRequest request, GConf.Client gconf)
                {

                        bool use_proxy;

                        try {
                                use_proxy = (bool) gconf.Get ("/system/http_proxy/use_http_proxy");
                        }
                        catch (GConf.NoSuchKeyException) {
                                use_proxy = false;
                        }

                        if (use_proxy) {

                                String phost;
                                int pport;
                                String[] pignore;
                                bool pauth;
                                
                                try {
                                
                                        phost = gconf.Get ("/system/http_proxy/host") as String;
                                        pport = (int) gconf.Get ("/system/http_proxy/port");
                                        pignore = gconf.Get ("/system/http_proxy/ignore_hosts") as String[];
                                        pauth = (bool) gconf.Get ("/system/http_proxy/use_authentication");
                                
                                }
                                catch (GConf.NoSuchKeyException) {

                                        SendNotification ("Unable to load proxy settings", 
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
                                                
                                                SendNotification ("Unable to load proxy settings",
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
                
                private void SendNotification (string title, string message)
                {

                        Bus bus = Bus.Session;

                        Notifications nf = bus.GetObject<Notifications> ("org.freedesktop.Notifications", new ObjectPath ("/org/freedesktop/Notifications"));

                        Dictionary <string,object> hints = new Dictionary <string,object> ();
                        
                        nf.Notify (title, 0, "", title, message, new string[0], hints, -1);
                }
        }
}
