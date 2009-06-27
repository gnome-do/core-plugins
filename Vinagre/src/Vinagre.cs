/* Vinagre.cs
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
using System.Diagnostics;
using System.Linq;

using Mono.Addins;

using Do.Universe;

namespace VinagreVNC 
{
	public class VNCAction : Act 
	{
       
		public override string Name { 
			get { return AddinManager.CurrentLocalizer.GetString ("Connect with VNC"); }
		}

		public override string Description {
	            get { return AddinManager.CurrentLocalizer.GetString ("Connect with VNC"); }
	        }

		public override string Icon {
			get { return "vinagre"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (HostItem);
				yield return typeof (ITextItem);
				yield return typeof (VNCHostItem);
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {         
			string hostname;
            
			if (items.First () is ITextItem) {
				hostname = (items.First () as ITextItem).Text;
			} else {
				HostItem hostitem = items.First () as HostItem;
				hostname = hostitem.Hostname + ":" + hostitem.Port;
			}
            
			Process vinagre = new Process ();
			vinagre.StartInfo.FileName = "vinagre";
			vinagre.StartInfo.Arguments = hostname;
			vinagre.Start ();
            
			yield break;
		}
	}
}
