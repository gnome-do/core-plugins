/* SSHHosts.cs
 *
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

using Do.Universe;

using Mono.Unix;

namespace NX {

    public class NXHostItem : Item {
        string name;
        string path;
        
        public NXHostItem (string hostname, string configpath)
        {
            name = hostname;
            path = configpath;
        }
        
        public override string Name { 
        	get { return name; } 
        }
                
        public override string Description { 
        	get { return Catalog.GetString ("NX Host"); }
        }
        
        public override string Icon {
        	get { return "gnome-globe"; } 
        }

		public string Path { 
        	get { return path; } 
        }
    }
    
    public class NXHostItemSource : ItemSource {
        List<Item> items;
        
        public NXHostItemSource ()
        {
            items = new List<Item> ();
        }
        
        public override string Name { 
        	get { return Catalog.GetString ("NX Hosts"); } 
        }
        
        public override string Description { 
        	get { return Catalog.GetString ("Parses nx sessions"); } 
        }
        
        public override string Icon { 
        	get { return "network-server"; } 
        }
        
        public override IEnumerable<Type> SupportedItemTypes {
            get { yield return typeof (NXHostItem); }
        }
        
        public override IEnumerable<Item> Items {
            get { return items; }
        }
        
        public override IEnumerable<Item> ChildrenOfItem (Item parent)
        {
            yield break;  
        }
        
        public override void UpdateItems ()
        {
        	items.Clear ();
        	
            string nxDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".nx");
            nxDir = Path.Combine (nxDir, "config");
            DirectoryInfo dir = new DirectoryInfo (nxDir);
            foreach (FileInfo file in dir.GetFiles ("*.nxs"))
            {
                string name = file.Name.Replace (".nxs", "");
                items.Add (new NXHostItem (name, Path.Combine (nxDir, file.Name)));
            }
        }
    }
}

