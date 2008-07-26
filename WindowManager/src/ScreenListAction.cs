// ScreenListAction.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Do.Universe;
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
					if (!w.IsSkipTasklist) continue;
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
				mask = mask | WindowMoveResizeMask.Width;
			if (height != oldheight)
				mask = mask | WindowMoveResizeMask.Height;
			if (x != oldx)
				mask = mask | WindowMoveResizeMask.X;
			if (y != oldy)
				mask = mask | WindowMoveResizeMask.Y;
			
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
	
	public class ScreenActionAction : AbstractAction
	{
		public override Type[] SupportedItemTypes {
			get { return new Type [] {
				typeof (IScreenItem), };
			}
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override Type[] SupportedModifierItemTypes {
			get { return new Type [] {}; }
		}
		
		public override bool SupportsItem (IItem item)
		{
			return true;
		}
		
		public override IItem[] DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}

		public override bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return false;
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			throw new NotImplementedException ();
		}

		public override string Description {
			get { return ""; }
		}

		public override string Icon {
			get { return ""; }
		}

		public override string Name {
			get { return ""; }
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
		
		protected List<Window> ViewportWindows {
			get {
				List<Window> windowList = new List<Window> ();
				
				Dictionary<string, List<Window>> processesList;
				
				WindowListItems.GetList (out processesList);
				
				//We need a list of every window in our current viewport only
				foreach (KeyValuePair<string, List<Window>> kvp in processesList) {
					foreach (Window w in kvp.Value) {
						if (w.IsInViewport (Screen.Default.ActiveWorkspace))
							windowList.Add (w);
					}
				}
				
				return windowList;
			}
		}
		
		protected List<Window> VisibleViewportWindows {
			get {
				return ViewportWindows.FindAll (delegate (Window w) { return !w.IsMinimized; });
			}
		}
	}
	
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

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			List<Window> windowList = VisibleViewportWindows;
			Gdk.Rectangle screenGeo = DoModifyGeometry.GetScreenMinusPanelGeometry;
			
			//can't tile no windows
			if (windowList.Count <= 1) return null;
			
			int square, height, width;
			
			//We are going to tile to a square, so what we want is to find
			//the smallest perfect square all our windows will fit into
			square = (int) Math.Ceiling (Math.Sqrt (windowList.Count));
			
			//Our width will always be our perfect square
			width = square;
			
			//Our height is at least one (e.g. a 2x1)
			height = 1;
			while (width * height < windowList.Count) {
				height++;
			}
			
			int windowWidth, windowHeight;
			windowWidth = screenGeo.Width / width;
			
			windowHeight = screenGeo.Height / height;
			
			int row = 0, column = 0;
			int x, y;
			
			for (int i = 0; i < windowList.Count; i++) {
				x = (column * windowWidth) + screenGeo.X;
				y = (row * windowHeight) + screenGeo.Y;
				
				if (i == windowList.Count - 1) {
					DoModifyGeometry.SetWindowGeometry (windowList[i], x, y,
					                                    windowHeight, screenGeo.Width - x, true);
				} else {
					DoModifyGeometry.SetWindowGeometry (windowList[i], x, y, windowHeight, 
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

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			List<Window> windowList = VisibleViewportWindows;
			
			//can't tile no windows
			if (windowList.Count <= 1) return null;
			
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
				(winbuffer * (windowList.Count - 1));
			
			height =  (Screen.Default.Height - (2 * ybuffer)) - 
				(winbuffer * (windowList.Count - 1));
			
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
	
	public class ScreenShelfAction : ScreenActionAction
	{
		public override string Name {
			get { return Catalog.GetString ("Shelf Windows"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Places your windows in a configurable shelf arrangement"); }
		}
		
		public override string Icon {
			get { return "preferences-system-windows"; }
		}

		public override IItem[] DynamicModifierItemsForItem (IItem item)
		{
			return base.DynamicModifierItemsForItem (item);
		}
		
		private Window LargestWindow (List<Window> windows)
		{
			Window largest = windows[0];
			int largest_size = 0;
			
			foreach (Window w in windows) {
				int x, y, height, width;
				w.GetGeometry (out x, out y, out width, out height);
				
				if (width * height > largest_size) {
					largest = w;
					largest_size = width * height;
				}
			}
			
			return largest;
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			//magic goes here (a.k.a the code that was here is too embarassing to release even into
			//a beta tree
			
			return null;
		}


	}
	
	public class ScreenRestoreAction : ScreenActionAction
	{
		public override string Name {
			get { return Catalog.GetString ("Restore Windows"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Restore Windows to their Previous Positions"); }
		}

		public override string Icon {
			get { return "preferences-system-windows"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			List<Window> windowList = new List<Window> ();
			
			Dictionary<string, List<Window>> processesList;
			
			WindowListItems.GetList (out processesList);
			
			foreach (KeyValuePair<string, List<Window>> kvp in processesList) {
				foreach (Window w in kvp.Value) {
					if (w.IsInViewport (Screen.Default.ActiveWorkspace))
						windowList.Add (w);
				}
			}
			
			foreach (Window w in windowList) {
				DoModifyGeometry.RestoreWindowGeometry (w);
			}
			
			return null;
		}

	}
}
