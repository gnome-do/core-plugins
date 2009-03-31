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
	
	
	public class WindowMoveAction : WindowActionAction
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
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return true;
		}
		
		public override void Action (IEnumerable<Window> windows)
		{
			// not used
		}

		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem screen = modItems.First () as IScreenItem;
			
			IEnumerable<Window> windows = null;
			if (items.First () is IApplicationItem) {
				windows = items.Cast<IApplicationItem> ().SelectMany (app => WindowUtils.WindowListForCmd (app.Exec));
			} else if (items.First () is WindowItem) {
				windows = items.Cast<WindowItem> ().SelectMany (wi => wi.Windows);
			}
			
			if (windows != null)
				foreach (Wnck.Window window in windows)
					screen.Viewport.MoveWindowInto (window);
			
			return null;
		}
	}
}
