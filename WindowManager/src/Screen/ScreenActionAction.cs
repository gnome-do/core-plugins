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
	
	
	public abstract class ScreenActionAction : Act
	{
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (IScreenItem);
			}
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override bool SupportsItem (Item item)
		{
			return true;
		}
		
		protected void SetWindowGeometry (Window w, int x, int y, int width, int height)
		{
			w.SetGeometry (WindowGravity.Northwest, 
                           WindowMoveResizeMask.Width,
			               x, y, width, height);
			w.SetGeometry (WindowGravity.Northwest, 
                           WindowMoveResizeMask.Height,
			               x, y, width, height);
			w.SetGeometry (WindowGravity.Northwest, 
                           WindowMoveResizeMask.X,
			               x, y, width, height);
			w.SetGeometry (WindowGravity.Northwest, 
                           WindowMoveResizeMask.Y,
			               x, y, width, height);
		}
	}
}
