//  
//  Copyright (C) 2009 GNOME Do
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
	
	
	public class ScreenSwapAction : ScreenActionAction
	{
		
		public override string Name {
			get { return Catalog.GetString ("Swap With..."); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Swap all windows on desktops"); }
		}

		public override string Icon {
			get { return "rotate"; }
		}
		
		public override bool ModifierItemsOptional {
			get {
				return false;
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return SupportedItemTypes;
			}
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return items.First () != modItem;
		}


		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem screen1 = items.First () as IScreenItem;
			IScreenItem screen2 = modItems.First () as IScreenItem;
			
			IEnumerable<Window> screen2Windows = ScreenUtils.ViewportWindows (screen2.Viewport);
			
			// Move screen1 windows to screen2
			foreach (Window w in ScreenUtils.ViewportWindows (screen1.Viewport)) {
				screen2.Viewport.MoveWindowInto (w);
			}
			
			// Move screen2 windows to screen1
			foreach (Window w in screen2Windows) {
				screen1.Viewport.MoveWindowInto (w);
			}
			
			return null;
		}
	}
}
