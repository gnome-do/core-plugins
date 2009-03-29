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

using Wnck;

using Do.Platform;
using Do.Universe;
using Do.Interface.Wink;

namespace WindowManager
{
	
	
	public class WindowMoveAction : Act
	{
		
		public WindowMoveAction()
		{
		}
		
		public override string Name {
			get {
				return "Move Window To...";
			}
		}
		
		public override string Description {
			get {
				return "Move window to remote workspace";
			}
		}

		public override string Icon {
			get {
				return "forward";
			}
		}
		
		public override bool ModifierItemsOptional {
			get {
				return false;
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new [] { typeof (IScreenItem) };
			}
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new [] { typeof (IApplicationItem) };
			}
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return true;
		}
		
		public override bool SupportsItem (Item item)
		{
			if (!(item is IApplicationItem)) return false;
			
			string application = (item as IApplicationItem).Exec;
			application = application.Split (new char[] {' '})[0];
			
			return WindowUtils.WindowListForCmd (application).Any ();
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem screen = modItems.First () as IScreenItem;
			
			foreach (IApplicationItem app in items.Cast<IApplicationItem> ()) {
				List<Window> windows = WindowUtils.WindowListForCmd (app.Exec);
				foreach (Wnck.Window window in windows)
					screen.Viewport.MoveWindowInto (window);
			}
			
			return null;
		}
	}
}
