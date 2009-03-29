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
using Do.Interface.Wink;

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
			
			if (!(item is IApplicationItem)) return false;
			
			string application = (item as IApplicationItem).Exec;
			application = application.Split (new char[] {' '})[0];
			
			return WindowUtils.WindowListForCmd (application).Any ();
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			return null;
		}
	}
}
