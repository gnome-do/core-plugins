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

namespace GnomeDoVNC 
{
	public class HostItem : IItem {
		string name;

		public HostItem (string hostname)
		{
			name = hostname;
		}

		public string Name { get { return name; } }
		public string Description { get { return "VNC Host"; } }
		public string Icon { get { return "gnome-globe"; } }

		public string Text { get { return name; } }
	}
	
	public class VNCHostItem : IItemSource {
		List<IItem> items;

		public VNCHostItem ()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}

		public string Name { get { return "Vinagre Bookmarks"; } }
		public string Description { get { return "Indexes your Vinagre Bookmarks"; } }
		public string Icon { get { return "gnome-globe"; } }

		public Type[] SupportedItemTypes {
			get {
				return new Type[] { typeof (VNCHostItem), typeof (HostItem) };
			}
		}

		public ICollection<IItem> Items {
			get { return items; }
		}

		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;  
		}

		public void UpdateItems ()
		{
			items.Clear ();
			string bookmarksFile = Environment.GetEnvironmentVariable ("HOME") + "/.gnome2/vinagre.bookmarks";
			try {
				StreamReader reader = File.OpenText(bookmarksFile);
				string s;
				while ((s = reader.ReadLine ()) != null) {
					if((s.Substring(0,1).Equals("[")) && (s.Substring(s.Length - 1,1).Equals("]"))) {
						s = s.Substring(1,s.Length - 2);
						items.Add (new HostItem(s));
					}
				}
			} catch { 
				Console.Error.WriteLine("[ERROR] " + bookmarksFile + " cannot be read!");
			} 
		}
	}
}

