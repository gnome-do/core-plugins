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

using Do.Universe;

namespace Do.Addins.Pidgin
{
	public sealed class PidginSetStatusAction : IAction
	{
		private IItem [] pidginStatuses;
		
		public PidginSetStatusAction ()
		{
			pidginStatuses = new IItem [] {new PidginStatusTypeItem (1), 
				new PidginStatusTypeItem (2), new PidginStatusTypeItem (3),
				new PidginStatusTypeItem (4), new PidginStatusTypeItem (5)};
		}
		
		public string Name { get { return "Set status"; } }
		public string Description { get { return "Set pidgin status message"; } }
		public string Icon { get { return "pidgin"; } }
		
		public Type [] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (PidginSavedStatusItem),
					typeof (ITextItem),
				};
			}
		}
		
		public Type [] SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (PidginStatusTypeItem),
				};
			}
		}
		
		public bool SupportsItem (IItem item)
		{
			return true;
		}
		
		public bool ModifierItemsOptional {
			get { return true; }
		}
		
		public bool SupportsModifierItemForItems (IItem [] items, IItem modItem)
		{
			if (items [0] is ITextItem)
				return true;
			return false;
		}
		
		public IItem [] DynamicModifierItemsForItem (IItem item)
        {
            return pidginStatuses;
        }
		
		public IItem [] Perform (IItem [] items, IItem [] modItems)
		{
			int status;
			string message;
				try {
				Pidgin.IPurpleObject prpl = Pidgin.GetPurpleObject ();
				
				if (items [0] is PidginSavedStatusItem) {
					status = (items [0] as PidginSavedStatusItem).ID;
					prpl.PurpleSavedstatusActivate (status);
				} else {
					message = (items [0] as ITextItem).Text;
					if (modItems.Length > 0)
						status = (int) (modItems [0] as PidginStatusTypeItem).Status;
					else
						status = prpl.PurpleSavedstatusGetType (prpl.PurpleSavedstatusGetCurrent ());
					Pidgin.PurpleSetAvailabilityStatus ((uint) status, message);
				}
			} catch { }
			return null;
		}
	}
}