/* AppendTextAction.cs
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
using System.Diagnostics;

using Do.Universe;

namespace GnomeDoVNC {
	public class VNCAction : AbstractAction {
		public override string Name { 
			get { 
				return "Connect with VNC"; 
			}
		}
		
		public override string Description {
			get {
				return "Connect with VNC"; 
			}
		}
		public override string Icon {
			get {
				return "vinagre";
			}
		}
		
		public override Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof(ITextItem),
					typeof(HostItem),
					typeof(VNCHostItem),
				};
			}
		}
		
		public override bool SupportsItem (IItem item) {
			return true;
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modItems) {
			
            string hostname;

            if (items [0] is ITextItem) {
                ITextItem textitem = items [0] as ITextItem;
                hostname = textitem.Text;
            }
            else {
                HostItem hostitem = items [0] as HostItem;
                hostname = hostitem.Text;
            }

			Process vinagre = new Process ();
			vinagre.StartInfo.FileName = "vinagre";
			vinagre.StartInfo.Arguments = hostname;
			vinagre.Start ();
			return null;
		}
	}
}
