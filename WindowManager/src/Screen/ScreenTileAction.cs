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
using WindowManager.Wink;

using Wnck;
using Mono.Addins;

namespace WindowManager
{
	
	
	public class ScreenTileAction : ScreenActionAction
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Tile Windows"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Tile All Windows in Current Viewport"); }
		}

		public override string Icon {
			get { return "preferences-system-windows"; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem item = items.First () as IScreenItem;
			item.Viewport.Tile ();
			
			return null;
		}

	}
}
