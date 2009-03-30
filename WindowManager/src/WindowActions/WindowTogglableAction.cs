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
		public abstract void ToggleGroup (IEnumerable<Window> windows);
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IEnumerable<Window> windows = null;
			
			if (items.First () is IWindowItem)
				windows = items.Cast<IWindowItem> ().SelectMany (w => w.Windows);
			else if (items.First () is IApplicationItem)
				windows = items.Cast<IApplicationItem> ().SelectMany (a => WindowUtils.WindowListForCmd (a.Exec));
			
			if (windows != null)
				ToggleGroup (windows);
			
			return null;
		}

	}
}
