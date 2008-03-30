// WindowSwitchAction.cs
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
	//This code can't go without acknowledging the woman who made it all
	//possible.  I know this file in itself is not much but it seemed
	//like a good place to put this.  You have always supported me Kristin.
	//You have put up with the late nights of me hacking away at this and many
	//many other parts of Do.  You showed amazing patience and you
	//have even showed interest from time to time.  I love you baby girl.
	//Without you I never would have gotten this far.
	
	public class WindowActionAction : AbstractAction
	{
		protected Dictionary<string, List<Window>> procList;
		protected Dictionary<string, List<Window>> procListDyn;
		protected Screen scrn;
		
		public WindowActionAction()
		{
			Gtk.Application.Init ();
			
			scrn = Screen.Default;
			
			WindowListItems.GetList (out procList);
			WindowListItems.GetList (out procListDyn);
			
			WindowListItems.ListUpdated += UpdateList;
		}
		
		public override string Name {
			get { return "Action Window"; }
		}
		
		public override string Description {
			get { return "Action a Window."; }
		}

		public override string Icon {
			get { return "eog"; } //fixme
		}
		
		public override bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return true;
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}
		
		protected void UpdateList ()
		{
			WindowListItems.GetList (out procList);
			WindowListItems.GetList (out procListDyn);
		}

		public override Type[] SupportedModifierItemTypes {
			get { return new Type [] {
				typeof (IWindowItem)};
			}
		}

		public override IItem[] DynamicModifierItemsForItem (IItem item)
		{
			IItem[] items;
			string application = (item as ApplicationItem).Exec;
			application = application.Split (new char[] {' '})[0];
			
			if (!procListDyn.ContainsKey (application)) return null;
			
			List<Window> winList;
			procListDyn.TryGetValue(application, out winList);
			
			items = new IItem[winList.Count];
			for (int i = 0; i < winList.Count; i++) {
				items[i] = new WindowItem (winList[i], item.Icon);
			}
			return items;
		}

		public override Type[] SupportedItemTypes {
			get { return new Type[] {
					typeof (ApplicationItem) };
			}
		}

		public override bool SupportsItem (IItem item)
		{
			string application = (item as ApplicationItem).Exec;
			application = application.Split (new char[] {' '})[0];
			
			if (!procList.ContainsKey (application)) return false;
			
			procList.Remove (application); //fixme potential crasher!
			return true;
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			return null;
		}


	}
	
	public class WindowListItems
	{
		private static Dictionary<string, List<Window>> windowList;
		private static Wnck.Screen scrn;
		private static bool listLock;
		
		static WindowListItems ()
		{
			Gtk.Application.Init (); //why do i HAVE to do this?
			
			windowList = new Dictionary<string,List<Window>> ();
			
			scrn = Screen.Default;
			scrn.WindowOpened += OnWindowOpened;
			scrn.WindowClosed += OnWindowClosed;
		}
			
		public static void GetList (out Dictionary<string, List<Window>> winList)
		{
			winList = new Dictionary<string,List<Window>> ();
			
			foreach (KeyValuePair<string, List<Window>> kvp in windowList) {
				winList.Add (kvp.Key, kvp.Value);
			}
		}
		
		private static void OnWindowOpened (object o, WindowOpenedArgs args)
		{
			Thread updateRunner = new Thread (new ThreadStart (UpdateList));
			
			updateRunner.Start ();
		}
		
		private static void OnWindowClosed (object o, WindowClosedArgs args)
		{
			Thread updateRunner = new Thread (new ThreadStart (UpdateList));
			
			updateRunner.Start ();
		}
		
		private static void UpdateList ()
		{
			if (listLock) return;
			
			listLock = true;
			windowList.Clear ();
			
			ProcessStartInfo st = new ProcessStartInfo ("ps");
			st.RedirectStandardOutput = true;
			st.UseShellExecute = false;
			
			Process process;
			string processName;
			
			foreach (Window w in scrn.Windows) {
				if (w.Pid == 0) continue;
				
				if (w.IsSkipTasklist) continue;
				
				st.Arguments = "c -o cmd --no-headers " + w.Pid;
				process = Process.Start (st);
				
				process.WaitForExit ();
				if (process.ExitCode != 0) continue;
				
				processName = process.StandardOutput.ReadLine ();
				if (windowList.ContainsKey (processName)) {
					List<Window> winList;
					windowList.TryGetValue (processName, out winList);
					winList.Add (w);
				} else {
					List<Window> winList = new List<Window> ();
					winList.Add (w);
					windowList.Add (processName, winList);
				}
			}
			
			listLock = false;
			
			ListUpdated ();
		}
		
		public static event ListUpdatedDelegate ListUpdated;
		
		public delegate void ListUpdatedDelegate ();
	}
	
	public class WindowMaximizeAction : WindowActionAction 
	{
		public override string Name {
			get { return "Maximize Window"; }
		}
		
		public override string Description {
			get { return "Make a window consume the whole screen"; }
		}

		public override string Icon {
			get { return "up"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			(modItems[0] as WindowItem).Window.Maximize ();
			return null;
		}

	}
	
	public class WindowMinimizeAction : WindowActionAction 
	{
		public override string Name {
			get { return "Minimize Window"; }
		}
		
		public override string Description {
			get { return "Minimize a Window to the Taskbar"; }
		}

		public override string Icon {
			get { return "down"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			(modItems[0] as WindowItem).Window.Minimize ();
			return null;
		}

	}
	
	public class WindowRestoreAction : WindowActionAction 
	{
		public override string Name {
			get { return "Restore Window"; }
		}
		
		public override string Description {
			get { return "Restore a Minimized Window"; }
		}

		public override string Icon {
			get { return "redo"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			Wnck.Window w = (modItems[0] as WindowItem).Window;
			if (w.IsMinimized)
				w.Unminimize (Gtk.Global.CurrentEventTime);
			return null;
		}

	}
	
	public class WindowShadeAction : WindowActionAction 
	{
		public override string Name {
			get { return "Shade/UnShade"; }
		}
		
		public override string Description {
			get { return "Shade a Window into its Titlebar"; }
		}

		public override string Icon {
			get { return "top"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			Wnck.Window w = (modItems[0] as WindowItem).Window;
			if (w.IsShaded)
				w.Unshade ();
			else
				w.Shade ();
			return null;
		}

	}
	
	public class WindowFocusAction : WindowActionAction 
	{
		public override string Name {
			get { return "Focus Window"; }
		}
		
		public override string Description {
			get { return "Bring a window into Focus"; }
		}

		public override string Icon {
			get { return "window-new"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			Wnck.Window w = (modItems[0] as WindowItem).Window;
			if (!w.IsInViewport (Wnck.Screen.Default.ActiveWorkspace)) {
				int viewX, viewY, viewW, viewH;
				int midX, midY;
				Screen scrn = Screen.Default;
				Workspace wsp = scrn.ActiveWorkspace;
				
				//get our windows geometry
				w.GetGeometry (out viewX, out viewY, out viewW, out viewH);
				
				//we want to focus on where the middle of the window is
				midX = viewX + (viewW / 2);
				midY = viewY + (viewH / 2);
				
				//The positions given above are relative to the current viewport
				//This makes them absolute
				midX += wsp.ViewportX;
				midY += wsp.ViewportY;
				
				//Check to make sure our middle didn't wrap
				if (midX > wsp.Width) {
					midX %= wsp.Width;
				}
				
				if (midY > wsp.Height) {
					midY %= wsp.Height;
				}
				
				//take care of negative numbers (happens?)
				while (midX < 0)
					midX += wsp.Width;
			
				while (midY < 0)
					midX += wsp.Height;
				
				Wnck.Screen.Default.MoveViewport (midX, midY);

			}
			w.Activate (Gtk.Global.CurrentEventTime);
			return null;
		}

	}
	
	public class WindowTileAction : WindowActionAction
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
		
		private void SetWindowGeometry (Window w, int x, int y, int width, int height)
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
			foreach (Window w in windowList)
			{
				x = column * windowWidth;
				y = (row * windowHeight) + 24;
				
				SetWindowGeometry (w, x, y, windowWidth, windowHeight);
				
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
