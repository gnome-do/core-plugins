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
	
	
	public class DoModifyGeometry
	{
		private class WindowState {
			public int X, Y, Height, Width;
			public Wnck.WindowState State;
			
			public WindowState (int x, int y, int height, 
			                    int width, Wnck.WindowState state)
			{
				X = x;
				Y = y;
				Height = height;
				Width = width;
				state = State;
			}
		}
		
		static Dictionary<Window, WindowState> windowList;
		
		static DoModifyGeometry ()
		{
			windowList = new Dictionary<Window, WindowState> ();
			
			Screen.Default.WindowClosed += OnWindowClosed;
		}
		
		public static Gdk.Rectangle GetScreenMinusPanelGeometry {
			get {
				Gdk.Rectangle outRect = new Gdk.Rectangle ();

				Screen scrn = Screen.Default;
				outRect.Width = scrn.Width;
				outRect.Height = scrn.Height;
				
				foreach (Window w in scrn.Windows) {
					if (!w.IsSkipTasklist || w.Name == "Do") continue;
					if (w.WindowType == WindowType.Dock) {
						//ok this is likely a panel.  Lets make sure!
						Gdk.Rectangle windowGeo = new Gdk.Rectangle ();
						w.GetGeometry (out windowGeo.X, out windowGeo.Y, 
						               out windowGeo.Width, out windowGeo.Height);
							
						//we would like to know which edge it sits against
						if (windowGeo.Y == 0 && windowGeo.Width > windowGeo.Height) {
							//top edge
							outRect.Y += windowGeo.Height;
							outRect.Height -= windowGeo.Height;
						} else if (windowGeo.X == 0 && windowGeo.Height > windowGeo.Width) {
							//left edge
							outRect.X += windowGeo.Width;
							outRect.Width -= windowGeo.Width;
						} else if (windowGeo.Y == scrn.Height - windowGeo.Height &&
						           windowGeo.Width > windowGeo.Height) {
							//bottom edge
							outRect.Height -= windowGeo.Height;
						} else if (windowGeo.X == scrn.Width - windowGeo.Width &&
						           windowGeo.Height > windowGeo.Width) {
							//right edge
							outRect.Width -= windowGeo.Width;
						}
					}
				}
				
				
				return outRect;
			}
		}
		
		/// <summary>
		/// Removes unused windows in the window list that have been closed
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="WindowClosedArgs"/>
		/// </param>
		public static void OnWindowClosed (object o, WindowClosedArgs args) {
			if (windowList.ContainsKey (args.Window)) {
				windowList.Remove (args.Window);
			}
		}
		
		public static bool SetWindowGeometry (Window w, int x, int y,
		                                      int height, int width, bool save)
		{
			int oldx, oldy, oldheight, oldwidth;
			
			w.GetGeometry (out oldx, out oldy, out oldwidth, out oldheight);
			
			bool alreadyInList = windowList.ContainsKey (w);
			
			if (save && !alreadyInList) 
				windowList.Add (w, new WindowState (oldx, oldy, oldheight, 
				                                    oldwidth, w.State));
			
			if (w.IsMaximized)
				w.Unmaximize ();
			
			WindowMoveResizeMask mask = 0;
			
			if (width != oldwidth)
				mask |= WindowMoveResizeMask.Width;
			if (height != oldheight)
				mask |= WindowMoveResizeMask.Height;
			if (x != oldx)
				mask |= WindowMoveResizeMask.X;
			if (y != oldy)
				mask |= WindowMoveResizeMask.Y;
			
			w.SetGeometry (WindowGravity.Northwest, mask, x, y, width, height);
			
			return true;
		}
		
		public static bool RestoreWindowGeometry (Window w)
		{
			WindowState state;
			if (windowList.TryGetValue (w, out state)) {
				SetWindowGeometry (w, state.X, state.Y, state.Height, 
				                   state.Width, false);
				windowList.Remove (w);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Returns a list of the windows that are actually configured
		/// </summary>
		/// <param name="windows">
		/// A <see cref="List`1"/>
		/// </param>
		/// <returns>
		/// A <see cref="List`1"/>
		/// </returns>
		public static List<Window> IsStored (List<Window> windows)
		{
			List<Window> retList = new List<Window> ();
			foreach (Window w in windows) {
				if (windowList.ContainsKey (w)) {
					retList.Add (w);
				}
			}
			return retList;
		}
	}
}
