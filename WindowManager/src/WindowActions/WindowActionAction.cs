// WindowActionAction.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this
// source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

using Do.Universe;
using Do.Interface.Wink;

using Wnck;
using Mono.Unix;

namespace WindowManager
{
	
	public abstract class WindowActionAction : Act
	{
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {
					typeof (IApplicationItem),
					typeof (IWindowItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is IApplicationItem) {
				string application = (item as IApplicationItem).Exec;
				application = application.Split (new char[] {' '})[0];
			
				return WindowUtils.WindowListForCmd (application).Any ();
			} else if (item is IWindowItem) {
				return true;
			}
			return false;
		}
		
		public abstract void Action (IEnumerable<Window> windows);
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IEnumerable<Wnck.Window> windows = null;
			if (items.First () is IWindowItem)
				windows = items.Cast<IWindowItem> ().SelectMany (wi => wi.Windows);
			else if (items.First () is IApplicationItem)
				windows = items.Cast<IApplicationItem> ().SelectMany (a => WindowUtils.WindowListForCmd (a.Exec));
			
			if (windows != null)
				Action (windows);
			return null;
		}
	}
}
