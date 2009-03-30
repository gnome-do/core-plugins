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
	
	
	public class ScreenTileAction : ScreenActionAction
	{
		public override string Name {
			get { return Catalog.GetString ("Tile Windows"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Tile All Windows in Current Viewport"); }
		}

		public override string Icon {
			get { return "preferences-system-windows"; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IScreenItem item = items.First () as IScreenItem;
			IEnumerable<Window> windowList = item.VisibleWindows;
			Gdk.Rectangle screenGeo = DoModifyGeometry.GetScreenMinusPanelGeometry;
			
			//can't tile no windows
			if (windowList.Count () <= 1) return null;
			
			int square, height, width;
			
			//We are going to tile to a square, so what we want is to find
			//the smallest perfect square all our windows will fit into
			square = (int) Math.Ceiling (Math.Sqrt (windowList.Count ()));
			
			//Our width will always be our perfect square
			width = square;
			
			//Our height is at least one (e.g. a 2x1)
			height = 1;
			while (width * height < windowList.Count ()) {
				height++;
			}
			
			int windowWidth, windowHeight;
			windowWidth = screenGeo.Width / width;
			
			windowHeight = screenGeo.Height / height;
			
			int row = 0, column = 0;
			int x, y;
			
			foreach (Wnck.Window window in windowList) {
				x = (column * windowWidth) + screenGeo.X;
				y = (row * windowHeight) + screenGeo.Y;
				
				if (window == windowList.Last ()) {
					DoModifyGeometry.SetWindowGeometry (window, x, y,
					                                    windowHeight, screenGeo.Width - x, true);
				} else {
					DoModifyGeometry.SetWindowGeometry (window, x, y, windowHeight, 
					                                    windowWidth, true);
				}
				
				column++;
				if (column == width) {
					column = 0;
					row++;
				}
			}
			
			return null;
		}

	}
}
