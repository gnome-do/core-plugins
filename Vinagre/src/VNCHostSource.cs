/* VNCHostSource.cs

 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;

namespace VinagreVNC {  
    public class VNCHostItem : ItemSource {
        List<Item> items;

        public VNCHostItem ()
        {
            items = new List<Item> ();
            //UpdateItems ();
        }

        public override string Name { 
            get { return Catalog.GetString ("Vinagre Bookmarks"); }
        }

        public override string Description { 
            get { return Catalog.GetString ("Indexes your Vinagre Bookmarks"); }
        }

        public override string Icon {
            get { return "gnome-globe"; }
        }

        public override IEnumerable<Type> SupportedItemTypes {
            get {
                return new Type[] {
                    typeof (VNCHostItem), 
                    typeof (HostItem) 
                };
            }
        }

        public override IEnumerable<Item> Items {
            get { return items; }
        }

        public override IEnumerable<Item> ChildrenOfItem (Item parent)
        {
            return null;  
        }

        public override void UpdateItems ()
        {
            items.Clear ();
            string bookmarks_file = Environment.GetEnvironmentVariable ("HOME") + "/.gnome2/vinagre.bookmarks";
            string s, host, port;
            try {
                StreamReader reader = File.OpenText(bookmarks_file);
                while ((s = reader.ReadLine ()) != null) {
                    if (s.Length > 1) {
                        if ((s.Substring (0,1).Equals ("[")) && (s.Substring (s.Length - 1,1).Equals ("]"))) {
                            s = s.Substring (1, s.Length - 2);
                            host = reader.ReadLine ();
                            port = reader.ReadLine ();
                            host = host.Substring (5, host.Length - 5);
                            port = port.Substring (5, port.Length - 5);
                            items.Add (new HostItem(s, host, port));
                        }
                    }
                }
            } catch { 
            } 
        }
    }
}

