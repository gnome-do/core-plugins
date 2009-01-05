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
using System.Linq;
using System.Diagnostics;
using System.Threading;

using Do.Universe;

using Wnck;
using Mono.Unix;

namespace WindowManager
{
	//This code can't go without acknowledging the woman who made it all
	//possible.  I know this file in itself is not much but it seemed
	//like a good place to put this.  You have always supported me Kristin.
	//You have put up with the late nights of me hacking away at this and many
	//many other parts of Do.  You showed amazing patience and you
	//have even showed interest from time to time.  I love you baby girl.
	//Without you I never would have gotten this far.
	
	public class WindowActionAction : Act
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
			get { return Catalog.GetString ("Action Window"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Action a Window."); }
		}

		public override string Icon {
			get { return "eog"; } //fixme
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
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
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { return new Type [] {
				typeof (IWindowItem)};
			}
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			Item[] items;
			
			if (item is IApplicationItem) {
				string application = (item as IApplicationItem).Exec;
				application = application.Split (new char[] {' '})[0];
				
				if (!procList.ContainsKey (application)) return null;
				
				List<Window> winList;
				procList.TryGetValue(application, out winList);
				
				items = new Item[winList.Count];
				for (int i = 0; i < winList.Count; i++) {
					items[i] = new WindowItem (winList[i], item.Icon);
				}
			} else if (item.GetType () == typeof (GenericWindowItem)) {
				items = new Item [1];
				
				items[0] = new WindowItem (WindowListItems.CurrentWindow, 
				                           "gnome-window-manager");
			} else {
				return null;
			}
			return items;
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {
					typeof (IApplicationItem),
					typeof (GenericWindowItem) };
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is GenericWindowItem) return true;
			string application = (item as IApplicationItem).Exec;
			application = application.Split (new char[] {' '})[0];
			
			return WindowManager.Util.GetApplicationList (application).Any ();
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			return null;
		}
	}
	
	//Most actions that are toggleable have identical logic for what to do with differnt types
	//of groups.  So lets just put it all here.
	public abstract class WindowTogglableAction : WindowActionAction
	{
		public abstract void ToggleGroup (List<Window> windows);
		public abstract void ToggleWindow (Window window);
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (modItems.Any ()) {
				Window w = (modItems.First () as WindowItem).Window;
				
				ToggleWindow (w);
			} else {
				if (items.First () is IApplicationItem) {
					List<Window> windows = new List<Window> ();
					foreach (Wnck.Application app in WindowManager.Util.GetApplicationList ((items.First () as IApplicationItem).Exec))
						windows.AddRange (app.Windows);
					ToggleGroup (windows);
				} else if (items.First () is GenericWindowItem) {
					GenericWindowItem generic;
					generic = (items.First () as GenericWindowItem);
					
					if (generic.WindowType == GenericWindowType.CurrentWindow) {
						ToggleWindow (WindowListItems.CurrentWindow);
					} else if (generic.WindowType == GenericWindowType.CurrentApplication) {
						ToggleGroup (WindowListItems.CurrentApplication);
					} else if (generic.WindowType == GenericWindowType.PreviousWindow) {
						ToggleWindow (WindowListItems.PreviousWindow);
					} else if (generic.WindowType == GenericWindowType.PreviousApplication) {
						ToggleGroup (WindowListItems.PreviousApplication);
					}
				}
			}
			
			return null;
		}

	}
	
	public class WindowMaximizeAction : WindowTogglableAction
	{
		public override string Name {
			get { return Catalog.GetString ("Maximize"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Make a window consume the whole screen"); }
		}

		public override string Icon {
			get { return "up"; }
		}

		public override void ToggleGroup (List<Window> windows)
		{
			if (!windows.Any ())
				return;
			WindowControl.MaximizeWindow (windows.First ());
		}

		public override void ToggleWindow (Window window)
		{
			WindowControl.MaximizeWindow (window);
		}
	}
	
	public class WindowMinimizeAction : WindowTogglableAction
	{
		public override string Name {
			get { return Catalog.GetString ("Minimize/Restore"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Minimize/Restore a Window"); }
		}

		public override string Icon {
			get { return "down"; }
		}
		
		public override void ToggleWindow (Window window)
		{
			WindowControl.MinimizeRestoreWindows (window);
		}
		
		public override void ToggleGroup (List<Window> windows)
		{
			WindowControl.MinimizeRestoreWindows (windows);
		}
	}
	
	public class WindowShadeAction : WindowTogglableAction
	{
		public override string Name {
			get { return Catalog.GetString ("Shade Window"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Shade/Unshade a Window into its Titlebar"); }
		}

		public override string Icon {
			get { return "top"; }
		}
		
		public override void ToggleWindow (Window window)
		{
			if (window.IsShaded)
				window.Unshade ();
			else
				window.Shade ();
		}
		
		public override void ToggleGroup (List<Window> windows)
		{
			bool shade = windows.All (w => !w.IsShaded);
			foreach (Window w in windows.Where (window => window.IsInViewport (window.Workspace))) {
				if (shade) 
					w.Shade();
				else
					w.Unshade ();
			}
		}
	}
	
	public class WindowCloseAction : WindowActionAction
	{
		public override string Name {
			get { return Catalog.GetString ("Close All"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Close your current window."); }
		}

		public override string Icon {
			get { return Gtk.Stock.Quit; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (modItems.Any ()) {
				Wnck.Window w = (modItems.First () as WindowItem).Window;
				
				w.Close (Gtk.Global.CurrentEventTime);
			} else {
				if (items.First () is IApplicationItem) {
					string application = (items.First () as IApplicationItem).Exec;
					List<Application> apps = WindowManager.Util.GetApplicationList (application);
					
					foreach (Application app in apps)
						foreach (Window w in app.Windows)
							w.Close (Gtk.Global.CurrentEventTime);
					
				}
			}
			return null;
		}

	}
}
