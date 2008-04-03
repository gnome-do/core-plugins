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
	}
	
	public class ScreenTileAction : ScreenActionAction
	{
		public override string Name {
			get { return "Tile Windows"; }
		}
		
		public override string Description {
			get { return "Tile All Windows in Current Viewport"; }
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
			
			//can't tile no windows
			if (windowList.Count <= 1) return null;
			
			int square, height, width;
			
			square = (int) Math.Ceiling (Math.Sqrt (windowList.Count));
			
			width = square;
			
			height = 1;
			while (width * height < windowList.Count) {
				height++;
			}
			
			int windowWidth, windowHeight;
			windowWidth = Screen.Default.Width / width;
			windowHeight = (Screen.Default.Height - 48) / height;
			
			int row = 0, column = 0;
			int x, y;
			
			foreach (Window w in windowList) {
				x = column * windowWidth;
				y = (row * windowHeight) + 24;
				
				DoModifyGeometry.SetWindowGeometry (w, x, y, windowHeight, 
				                                    windowWidth, true);
				
				column++;
				if (column == width) {
					column = 0;
					row++;
				}
			}
			
			return null;
		}

	}
	
	public class ScreenRestoreAction : ScreenActionAction
	{
		public override string Name {
			get { return "Restore Windows"; }
		}
		
		public override string Description {
			get { return "Restore Windows to their Previous Positions"; }
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
