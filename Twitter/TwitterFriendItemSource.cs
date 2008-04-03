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
             string url, username, password;
             string screen_name, image, name;
             HttpWebResponse response;
             XmlDocument friends = new XmlDocument ();
             GConf.Client gconf = new GConf.Client ();
             ContactItem twit_friend_by_name, twit_friend_by_sn;
             screen_name = name = image = username = password = "";
             items.Clear ();
             
             try {
                 username = gconf.Get ("/apps/gnome-do/plugins/twitter/username") as string;
                 password = gconf.Get ("/apps/gnome-do/plugins/twitter/password") as string;
             } catch (NoSuchKeyException) {
                 gconf.Set ("/apps/gnome-do/plugins/twitter/username","");
                 gconf.Set ("/apps/gnome-do/plugins/twitter/password","");
             }
             
             url = "http://twitter.com/statuses/friends.xml";
             HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
             request.Credentials = new NetworkCredential (username,password);
             response = (HttpWebResponse) request.GetResponse ();
             friends.Load (response.GetResponseStream ());
             response.Close ();
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
                 //Here we make two contact items to make searching a little bit
                 //easier. One can search by a real name and get an assload of contact
                 //details, or by twitter screen name.
                 twit_friend_by_name =  ContactItem.Create (name);
                 twit_friend_by_name["twitter.screename"] = "@" + screen_name;
                 items.Add (twit_friend_by_name);
                 twit_friend_by_sn = ContactItem.Create (screen_name);
                 twit_friend_by_sn["twitter.screename"] = "@" + screen_name;
                 items.Add(twit_friend_by_sn);
             }
         }
     }
 }
