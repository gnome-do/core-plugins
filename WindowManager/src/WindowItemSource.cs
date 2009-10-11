// WindowItemSource.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
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

using Do.Universe;
using WindowManager.Wink;

using Wnck;
using Mono.Addins;

namespace WindowManager
{
	public class WindowItemSource : ItemSource
	{
		List<Item> items;
		
		public override string Name {
			get {
				return AddinManager.CurrentLocalizer.GetString ("Generic Window Items");
			}
		}

		public override string Description {
			get {
				return AddinManager.CurrentLocalizer.GetString ("Useful Generically Understood Window Items");
			}
		}

		public override string Icon {
			get {
				return "gnome-window-manager";
			}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (IApplicationItem),
				};
			}
		}

		public override IEnumerable<Item> Items {
			get {
				return items;
			}
		}

		public WindowItemSource ()
		{
			WindowUtils.Initialize ();
			items = new List<Item> ();
			items.Add (new CurrentApplicationItem ());
			items.Add (new CurrentWindowItem ());
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			List<Item> results = new List<Item> ();
			IApplicationItem app = item as IApplicationItem;
			if (app == null)
				return results;
			
			List<Wnck.Window> windows = WindowUtils.WindowListForCmd (app.Exec);
			if (!windows.Any ())
				return results;
			
			foreach (Wnck.Window window in windows)
				results.Add (new WindowItem (window, app.Icon));
			
			return results;
		}	
	}
}
