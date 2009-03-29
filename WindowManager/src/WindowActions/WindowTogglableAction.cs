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

using Do.Universe;
using Do.Interface.Wink;

using Wnck;
using Mono.Unix;

namespace WindowManager
{
		
	//Most actions that are toggleable have identical logic for what to do with differnt types
	//of groups.  So lets just put it all here.
	public abstract class WindowTogglableAction : WindowActionAction
	{
		public abstract void ToggleGroup (List<Window> windows);
		public abstract void ToggleWindow (Window window);
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (modItems.Any ()) {
				Window w = (modItems.First () as WindowItem).Window;
				
				ToggleWindow (w);
			} else {
				if (items.First () is IApplicationItem) {
					List<Window> windows = new List<Window> ();
					windows.AddRange (WindowUtils.WindowListForCmd ((items.First () as IApplicationItem).Exec));
					
					ToggleGroup (windows);
				} else if (items.First () is GenericWindowItem) {
					GenericWindowItem generic;
					generic = (items.First () as GenericWindowItem);
					
					if (generic.WindowType == GenericWindowType.CurrentWindow) {
						ToggleWindow (WindowListItems.CurrentWindow);
					} else if (generic.WindowType == GenericWindowType.CurrentApplication) {
						ToggleGroup (WindowListItems.CurrentApplication);
					} else if (generic.WindowType == GenericWindowType.PreviousWindow) {
						ToggleWindow (WindowListItems.PreviousWindow);
					} else if (generic.WindowType == GenericWindowType.PreviousApplication) {
						ToggleGroup (WindowListItems.PreviousApplication);
					}
				}
			}
			
			return null;
		}

	}
}
