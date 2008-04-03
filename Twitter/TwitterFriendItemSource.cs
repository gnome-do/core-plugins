/* TwitterFriendItem.cs
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
 using System.Diagnostics;
 using System.Collections.Generic;
 
 using GConf;
 using Do.Universe;
 
 namespace Do.Twitter
 {
     public class TwitterFriendItemSource : IItemSource {
         List<IItem> items;
         
         public TwitterFriendItemSource () {
             items = new List<IItem> ();
         }
         
         public string Name { get { return "Twitter friends"; } }
         public string Description { get { return "Indexes your Twitter friends"; } }
         public string Icon { get { return ""; } }
         
         public Type[] SupportedItemTypes {
             get {
                 return new Type[] { typeof (TwitterFriendItem) };
             }
         }
         
         public ICollection<IItem> Items {
             get { return items; }
         }
         
         public ICollection<IItem> ChildrenOfItem (IItem parent) {
             return null;
         }
         
         public void UpdateItems () {
             GConf.Client gconf = new GConf.Client ();
             string username = gconf.Get ("/apps/gnome-do/plugins/twitter/username") as string;
             string password = gconf.Get ("/apps/gnome-do/plugins/twitter/password") as string;
             XmlDocument friends = new XmlDocument ();
             items.Clear ();
             string uri = string.Format("http://{0}:{1}@twitter.com/statuses/friends.xml",username,password);
             string dest = Environment.GetEnvironmentVariable ("HOME") + "/.local/share/gnome-do/friends.xml";
             string screen_name, image, name;
             screen_name = name = image = "";
             System.Diagnostics.Process p = System.Diagnostics.Process.Start("wget", string.Format("{0} --output-document={1}", uri, dest) ); 
             p.WaitForExit ();
             friends.Load (dest);
             foreach (XmlNode user_node in friends.GetElementsByTagName ("user")) {
                 foreach (XmlNode attr in user_node.ChildNodes) {
                     switch(attr.Name) {
                     case("screen_name"):
                         screen_name = attr.InnerText; break;
                     case("name"):
                         name = attr.InnerText; break;
                     case("profile_image_url"):
                         image =  attr.InnerText; break;
                     }
                 }
                 items.Add (new TwitterFriendItem (screen_name,name,image));

             }
             System.IO.File.Delete (dest);
         }
     }
 }
