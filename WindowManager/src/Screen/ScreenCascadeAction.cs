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
	
	
	public class ScreenCascadeAction : ScreenActionAction
	{
		public override string Name {
			get { return Catalog.GetString ("Cascade Windows"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Cascade your Windows"); }
		}
		
		public override string Icon {
			get { return "preferences-system-windows"; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem item = items.First () as IScreenItem;
			IEnumerable<Window> windowList = item.VisibleWindows;
			
			//can't tile no windows
			if (windowList.Count () <= 1) return null;
			
			int width, height, offsetx, offsety;
			
			int xbuffer = 13;
			int ybuffer = 30;
			int winbuffer = 30;
			
			//we want to cascade here, so the idea is that for each window we
			//need to be able to create a buffer that is about the width of the
			//title bar.  We will approximate this to around 30px.  Therefor our
			//width and height need to be (30 * number of Windows) pixels
			//smaller than the screen.  We will also offset by 100 on both
			//the x and y axis, so we should end as such.  This needs to be
			//calculated for too.
			
			width = (Screen.Default.Width - (2 * xbuffer)) - 
				(winbuffer * (windowList.Count () - 1));
			
			height =  (Screen.Default.Height - (2 * ybuffer)) - 
				(winbuffer * (windowList.Count () - 1));
			
			offsetx = xbuffer;
			offsety = ybuffer;
			
			
			foreach (Window w in windowList) {
				DoModifyGeometry.SetWindowGeometry (w, offsetx, offsety, height,
				                                    width, true);
				
				offsetx += winbuffer;
				offsety += winbuffer;
			}
			
			return null;
		}
		
	}
}
