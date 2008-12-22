//  PidginSetStatusAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
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

using System;
using System.Collections.Generic;
using System.Linq;

using Do.Universe;
using Mono.Unix;

namespace Do.Addins.Pidgin
{
	public sealed class PidginSetStatusAction : Act
	{
		private Item [] pidginStatuses;
		
		public PidginSetStatusAction ()
		{
			pidginStatuses = new Item [] {new PidginStatusTypeItem (1), 
				new PidginStatusTypeItem (2), new PidginStatusTypeItem (3),
				new PidginStatusTypeItem (4), new PidginStatusTypeItem (5)};
		}
		
		public override string Name { get { return Catalog.GetString ("Set status"); } }
		
		public override string Description {
			get { return Catalog.GetString ("Set pidgin status message"); }
		}
		
		public override string Icon { get { return "pidgin"; } }
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (PidginSavedStatusItem),
					typeof (ITextItem),
				};
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (PidginStatusTypeItem),
				};
			}
		}
		
		public bool SupportsItem (Item item)
		{
			return true;
		}
		
		public bool ModifierItemsOptional {
			get { return true; }
		}
		
		public bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (items.First () is ITextItem)
				return true;
			return false;
		}
		
		public IEnumerable<Item> DynamicModifierItemsForItem (Item item)
        {
            return pidginStatuses;
        }
		
		public IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			int status;
			string message;
				try {
				Pidgin.IPurpleObject prpl = Pidgin.GetPurpleObject ();
				
				if (items.First () is PidginSavedStatusItem) {
					status = (items.First () as PidginSavedStatusItem).ID;
					prpl.PurpleSavedstatusActivate (status);
				} else {
					message = (items.First () as ITextItem).Text;
					if (modItems.Any ())
						status = (int) (modItems.First () as PidginStatusTypeItem).Status;
					else
						status = prpl.PurpleSavedstatusGetType (prpl.PurpleSavedstatusGetCurrent ());
					Pidgin.PurpleSetAvailabilityStatus ((uint) status, message);
				}
			} catch { }
			return null;
		}
	}
}