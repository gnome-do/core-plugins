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
using System.Text.RegularExpressions;

using Do.Platform;
using Do.Universe;

using Mono.Unix;

namespace GnomeDoSSH {

	public class HostItem : Item, IOpenableItem {
		string name;

		public HostItem (string hostname)
		{
			name = hostname;
		}

		public override string Name { get { return name; } }
		public override string Description { get { return Catalog.GetString ("SSH Host"); } }
		public override string Icon { get { return "gnome-globe"; } }

		public string Text { get { return name; } }

		public void Open ()
		{
			Services.Environment.OpenUrl ("ssh://" + name);
		}
	}

	public class SSHHostItemSource : ItemSource {
		List<Item> items;

		public SSHHostItemSource ()
		{
			items = new List<Item> ();
			UpdateItems ();
		}

		public override string Name { get { return Catalog.GetString ("SSH Hosts"); } }
		public override string Description { get { return Catalog.GetString ("Parses ssh-config"); } }
		public override string Icon { get { return "network-server"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (HostItem) };
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
			try {
				string hostsFile = Environment.GetEnvironmentVariable ("HOME") + "/.ssh/config";
				FileStream fs = new FileStream (hostsFile, FileMode.Open, FileAccess.Read);
				StreamReader reader = new StreamReader (fs);

				Regex r = new Regex ("^\\s*Host\\s+([^ ]+)\\s*$");

				string s;
				while ((s = reader.ReadLine ()) != null) {
					Match m = r.Match (s);
					if (m.Groups.Count == 2) {
						items.Add (new HostItem (m.Groups [1].ToString ()));
					}
				}
			} catch { }
		}
	}
}

