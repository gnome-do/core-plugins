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
			get { return true; }
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
				
				if (!procList.ContainsKey (application)) return null;
				
				List<Window> winList;
				procList.TryGetValue(application, out winList);
				
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
			if (modItems.Length > 0) {
				WindowItem w = (modItems[0] as WindowItem);
				
				if (w.Window.IsMaximized) {
					w.Window.Unmaximize ();
				} else {
					w.Window.Maximize ();
				}
			} else {
				if (items[0] is ApplicationItem) {
					string application = (items[0] as ApplicationItem).Exec;
					application = application.Split(new char [] {' '})[0];
					
					List<Window> windows = WindowListItems.GetApplication(application);
					
					bool maximize = true;
					foreach (Window w in windows) {
						if (w.IsMaximized) maximize = false;
					}
					
					foreach (Window w in windows) {
						if (maximize)
							w.Maximize ();
						else
							w.Unmaximize ();
					}
				} else if (items[0] is GenericWindowItem) {
					if ((items[0] as GenericWindowItem).WindowType == GenericWindowType.CurrentWindow) {
						Window w = WindowListItems.CurrentWindow;
						
						if (w.IsMaximized) 
							w.Unmaximize ();
						else
							w.Maximize ();
					}
				}
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
			if (modItems.Length > 0) { //user selected a mod item
				Window w = (modItems[0] as WindowItem).Window;
				
				if (w.IsMinimized)
					w.Unminimize (Gtk.Global.CurrentEventTime);
				else
					w.Minimize ();
				
			} else { //no mod item
				if (items[0] is ApplicationItem) { //it was an application item
					string application;
					ApplicationItem app = (items[0] as ApplicationItem);
					
					application = app.Exec;
					application = application.Split (new char[] {' '})[0];
					
					List<Window> windowList = WindowListItems.GetApplication (application);
					
					bool minimize = false;
					foreach (Window w in windowList) {
						if (!w.IsMinimized) minimize = true;
					}
					
					foreach (Window w in windowList) {
						if (minimize)
							w.Minimize ();
						else
							w.Unminimize (Gtk.Global.CurrentEventTime);
					}
				} else if (items[0] is GenericWindowItem) {
					if ((items[0] as GenericWindowItem).WindowType == GenericWindowType.CurrentWindow) {
						Window w = WindowListItems.CurrentWindow;
						
						if (w.IsMinimized) 
							w.Unminimize (Gtk.Global.CurrentEventTime);
						else
							w.Minimize ();
					}
				}
			}
			
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
			if (modItems.Length > 0)
			{
				Wnck.Window w = (modItems[0] as WindowItem).Window;
				if (w.IsShaded)
					w.Unshade ();
				else
					w.Shade ();
			} else {
				if (items[0] is ApplicationItem) {
					string application = (items[0] as ApplicationItem).Exec;
					application = application.Split(new char [] {' '})[0];
					
					List<Window> windows = WindowListItems.GetApplication (application);
					
					bool shade = true;
					foreach (Window w in windows) {
						if (w.IsShaded) shade = false;
					}
					
					foreach (Window w in windows) {
						if (shade) 
							w.Shade();
						else
							w.Unshade ();
					}
				} else if (items[0] is GenericWindowItem) {
					if ((items[0] as GenericWindowItem).WindowType == GenericWindowType.CurrentWindow) {
						Window w = WindowListItems.CurrentWindow;
						
						if (w.IsShaded)
							w.Unshade ();
						else
							w.Shade ();
					}
				}
			}
			
			
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
		
		private List<Window> SortWindowsToStack (List<Window> windows)
		{
			Window[] stack = Screen.Default.WindowsStacked;
			List<Window> outList = new List<Window> ();
			
			for (int i = 0; i < stack.Length; i++) {
				if (windows.Contains (stack[i]))
					outList.Add (stack[i]);
			}
			
			return outList;
		}
		
		private void FocusWindowViewport (Window w)
		{
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
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			if (modItems.Length > 0) {
				Wnck.Window w = (modItems[0] as WindowItem).Window;
				
				FocusWindowViewport (w);
			} else {
				if (items[0] is ApplicationItem) {
					string application = (items[0] as ApplicationItem).Exec;
					application = application.Split (new char[] {' '})[0];
					
					List<Window> windows = WindowListItems.GetApplication (application);
					
					windows = SortWindowsToStack (windows);
					
					for (int i = 0; i < windows.Count; i++) {
						if ( i == windows.Count - 1) {
							Window w = windows[i];
							//The WM is ultimately going to determine what order this is done it.
							//It seems to like doing it in reverse order (stacks are fun)
							//So we delay to make sure this one is done last...
							FocusWindowViewport (w);
						}
						windows[i].Activate (Gtk.Global.CurrentEventTime);
					}
				} else if (items[0] is GenericWindowItem) {
					return null;
				}
			}
			return null;
		}

	}
}
