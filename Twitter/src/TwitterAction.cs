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
using System.Net;

using Do.Universe;

namespace Twitter
{
    public class TweetAction : IAction
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
            string reply = "";
            if (modItems.Length > 0)
                reply = (modItems[0] as ContactItem)["twitter.screename"] + " ";
            string tweet = EscapeTweet ((items [0] as ITextItem).Text);
            string url = "http://twitter.com/statuses/update.json?status=" + reply + tweet;
            HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
            request.Credentials = new NetworkCredential (Twitter.Username, Twitter.Password);
            request.Method = "POST";

            GConf.Client gconf = new GConf.Client ();

            if (!Twitter.SetRequestProxy (request, gconf)) return null;
            try {
                request.GetResponse ();
                Twitter.SendNotification ("Tweet Successful", "Successfully posted tweet '" 
                                + reply + " " + (items [0] as ITextItem).Text
                                + "' to Twitter.");
            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
                Twitter.SendNotification ("Tweet Failed", "Unable to post tweet. Check your login "
                                + "settings (/apps/gnome-do/plugins/twitter). If you are "
                                + "behind a proxy, also make sure that the settings in "
                                + "/system/http_proxy are correct.\n\nDetails:\n" 
                                + e.ToString ());
            }
            return null;
        }
        
        public Type[] SupportedModifierItemTypes {
            get { 
                return new Type[] {
                    typeof (ContactItem),
                };
            }
        }

        public bool ModifierItemsOptional {
            get { return true; }
        }
                        
        public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
        {
            return (modItem as ContactItem)["twitter.screenname"] != null;
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
    }
}
