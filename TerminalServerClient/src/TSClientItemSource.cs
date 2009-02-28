/* TSClientItemSource.cs
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
using System.IO;
using System.Xml;
using System.Collections.Generic;


using Do.Universe;
using Do.Platform;

namespace TSClient
{
	public class TSClientItemSource : ItemSource
	{
		List<Item> items;

		public TSClientItemSource () {
			items = new List<Item> ();
		}

		public override string Name { get { return "Terminal Server Connection Items"; } }
		public override string Description { get { return "Parses Connections in ~/.tsclient"; } }
		public override string Icon { get { return "network-server"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (TSClientItem) };
			}
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent) {
			return null;
		}

		public override void UpdateItems () {
			items.Clear ();
			try {
				string tsclientDir = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
				                                         ".tsclient");
				List<string> clients = GetFilesRecursive(tsclientDir);
				
				foreach (string file in clients) {
					string name = file.Replace (".rdp", "");
					items.Add (new TSClientItem (name, file));
					Log<TSClientItemSource>.Debug ("rdp file '{0}' indexed.", file);
				}
			} catch { }
		}
		
	    private static List<string> GetFilesRecursive (string src) {
	        List<string> result = new List<string> ();
	        Stack<string> stack = new Stack<string> ();

	        stack.Push (src);

	        while (stack.Count > 0) {
	            string dir = stack.Pop ();

	            try {
	                result.AddRange (Directory.GetFiles(dir, "*.rdp"));

	                foreach (string dn in Directory.GetDirectories (dir))
	                    stack.Push (dn);
	            }
	            catch {
					Log<TSClientItemSource>.Error ("Could not open directory '{0}'", dir);
	            }
	        }
	        return result;
	    }

	}

}
