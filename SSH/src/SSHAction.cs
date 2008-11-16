/* SSHAction.cs
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
using System.Collections.Generic;
using System.Linq;
using Do.Universe;
using System.Diagnostics;

using GConf;
using Mono.Unix;

namespace GnomeDoSSH {
	public class SSHAction : AbstractAction {
		
		public override string Name {
			get {
				return Catalog.GetString ("Connect with SSH");
			}
		}
		
		public override string Description {
			get {
				return Catalog.GetString ("Connect with SSH");
			}
		}
		
		public override string Icon {
			get {
				return "network-server";
			}
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
            get {
                return new Type[] {
                    typeof(ITextItem),
                    typeof(HostItem),
                };
            }
		}
		
		public override bool SupportsItem (IItem item) {
			return true;
		}
		
		public override IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems) {
			GConf.Client client = new GConf.Client ();

			string exec;
			try {
				exec = client.Get ("/desktop/gnome/applications/terminal/exec") as string;
			} 
			catch {
				exec = "gnome-terminal";
			}
			
            string hostname;

            if (items.First () is ITextItem) {
                ITextItem textitem = items.First () as ITextItem;
                hostname = textitem.Text;
            }
            else {
                HostItem hostitem = items.First () as HostItem;
                hostname = hostitem.Text;
            }

			Process term = new Process ();
			term.StartInfo.FileName = exec;
			term.StartInfo.Arguments = "-e 'ssh " + hostname + "'";
			term.Start ();
			return null;
		}
	}
}
