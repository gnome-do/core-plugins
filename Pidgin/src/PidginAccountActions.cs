// PidginAccountActions.cs
// 
// Copyright (C) 2008 Alex Launi <alex.launi@gmail.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//


using System;
using System.Collections.Generic;
using System.Linq;
using Do.Universe;
using Mono.Unix;

namespace Do.Addins.Pidgin
{
	public class PidginEnableAccount : IAction
	{
		public string Name { get { return Catalog.GetString ("Sign on"); } }
		public string Description { get { return Catalog.GetString ("Enable pidgin account"); } }
		public string Icon { get { return "pidgin"; } }
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (PidginAccountItem),
				};
			}
		}
		
		public IEnumerable<Type> SupportedModifierItemTypes {
			get { return null; }
		}
		
		public bool SupportsItem (IItem item)
		{
			Pidgin.IPurpleObject prpl;
			 try {
				prpl = Pidgin.GetPurpleObject ();
				if (!prpl.PurpleAccountIsConnected ((item as PidginAccountItem).ID))
					return true;
			} catch { }
			
			return false;
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		
		public bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem)
		{
			return false;
		}
		
		public IEnumerable<IItem> DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}
		
		public IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems)
		{
			Pidgin.IPurpleObject prpl;
			try {
				prpl = Pidgin.GetPurpleObject ();
				prpl.PurpleAccountSetEnabled ((items.First () as PidginAccountItem).ID,
					"gtk-gaim", 1);
			} catch { }
			
			return null;
		}		
	}
	
	public class PidginDisableAccount : IAction
	{
		public string Name { get { return Catalog.GetString ("Sign off"); } }
		public string Description { get { return Catalog.GetString ("Disble pidgin account"); } }
		public string Icon { get { return "pidgin"; } }
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (PidginAccountItem),
				};
			}
		}
		
		public IEnumerable<Type> SupportedModifierItemTypes {
			get { return null; }
		}
		
		public bool SupportsItem (IItem item)
		{
			 Pidgin.IPurpleObject prpl;
			 try {
				prpl = Pidgin.GetPurpleObject ();
				if (prpl.PurpleAccountIsConnected ((item as PidginAccountItem).ID))
					return true;
			} catch { }
			
			return false;
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		
		public bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem)
		{
			return false;
		}
		
		public IEnumerable<IItem> DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}
		
		public IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems)
		{
			Pidgin.IPurpleObject prpl;
			try {
				prpl = Pidgin.GetPurpleObject ();
				prpl.PurpleAccountSetEnabled ((items.First () as PidginAccountItem).ID,
					"gtk-gaim", 0);
			} catch { }
			
			return null;
		}
	}
}
