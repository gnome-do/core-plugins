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
		protected Dictionary<string, string> matchList;
		protected Screen scrn;
		
		public WindowActionAction()
		{
			Gtk.Application.Init ();
			
			scrn = Screen.Default;
			
			WindowListItems.GetList (out procList);
			WindowListItems.GetList (out procListDyn);
			matchList = new Dictionary<string,string> ();
			
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
			
			if (item is ApplicationItem) {
				string application = (item as ApplicationItem).Exec;
				application = application.Split (new char[] {' '})[0];
				
				if (!procListDyn.ContainsKey (application)) return null;
				
				List<Window> winList;
				procListDyn.TryGetValue(application, out winList);
				
				items = new IItem[winList.Count];
				for (int i = 0; i < winList.Count; i++) {
					items[i] = new WindowItem (winList[i], item.Icon);
				}
			} else if (item.GetType () == typeof (GenericWindowItem)) {
				items = new IItem [1];
				
				items[0] = new WindowItem (WindowListItems.CurrentWindow, 
				                           "gnome-window-manager");
			} else {
				return null;
			}
			return items;
		}

		public override Type[] SupportedItemTypes {
			get { return new Type[] {
					typeof (ApplicationItem),
					typeof (GenericWindowItem) };
			}
		}

		public override bool SupportsItem (IItem item)
		{
			if (item is GenericWindowItem) return true;
			
			string application = (item as ApplicationItem).Exec;
			application = application.Split (new char[] {' '})[0];
			
			if (matchList.ContainsKey (application)) {
				string s;
				matchList.TryGetValue (application, out s);
				if (s != item.Name) return false;
			} else {
				matchList.Add (application, item.Name);
			}
			
			if (!procList.ContainsKey (application)) return false;
			
			//procList.Remove (application); //fixme potential crasher!
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
		private static Window currentWindow;
		
		public static Window CurrentWindow {
			get {
				return currentWindow;
			}
		}
		
		static WindowListItems ()
		{
			Gtk.Application.Init (); //why do i HAVE to do this?
			
			windowList = new Dictionary<string,List<Window>> ();
			
			scrn = Screen.Default;
			scrn.WindowOpened        += OnWindowOpened;
			scrn.WindowClosed        += OnWindowClosed;
			scrn.ActiveWindowChanged += OnActiveWindowChanged;
		}
			
		public static void GetList (out Dictionary<string, List<Window>> winList)
		{
			winList = new Dictionary<string,List<Window>> ();
			
			foreach (KeyValuePair<string, List<Window>> kvp in windowList) {
				winList.Add (kvp.Key, kvp.Value);
			}
		}
		
		private static void OnActiveWindowChanged (object o, ActiveWindowChangedArgs args)
		{
			try {
				if (!scrn.ActiveWindow.IsSkipTasklist)
					currentWindow = scrn.ActiveWindow;
			}
			catch {
				
			}
		}
		
		private static void OnWindowOpened (object o, WindowOpenedArgs args)
		{
			if (args.Window.IsSkipTasklist) return;
			
			Thread updateRunner = new Thread (new ThreadStart (UpdateList));
			
			updateRunner.Start ();
		}
		
		private static void OnWindowClosed (object o, WindowClosedArgs args)
		{
			if (args.Window.IsSkipTasklist) return;
			
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
			Wnck.Window w = (modItems[0] as WindowItem);
			
			if (w.IsMaximized) {
				w.Unmaximize ();
			} else {
				w.Maximize ();
			}
			
			return null;
		}

	}
	
	public class WindowMinimizeAction : WindowActionAction
	{
		public override string Name {
			get { return "Minimize Window"; }
		}
		
		public override string Description {
			get { return "Minimize/Restore a Window"; }
		}

		public override string Icon {
			get { return "down"; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			Window w = (modItems[0] as WindowItem).Window;
			
			if (w.IsMinimized)
				w.Unminimize (Gtk.Global.CurrentEventTime);
			else
				w.Minimize ();
			
			return null;
		}

	}
	
	public class WindowShadeAction : WindowActionAction 
	{
		public override string Name {
			get { return "Shade Window"; }
		}
		
		public override string Description {
			get { return "Shade/Unshade a Window into its Titlebar"; }
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
}
