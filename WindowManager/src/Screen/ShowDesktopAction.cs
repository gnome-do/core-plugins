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

namespace WindowManager
{
	
	
	public class ShowDesktopAction : ScreenActionAction
	{
		
		public override string Name {
			get {
				return "Show Desktop";
			}
		}

		public override string Description {
			get {
				return "Minimize all windows on the desktop.";
			}
		}

		public override string Icon {
			get {
				return "desktop";
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem item = items.First () as IScreenItem;
			item.Viewport.ShowDesktop ();

			return null;
		}

	}
}
