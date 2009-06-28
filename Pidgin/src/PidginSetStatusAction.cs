//  PidginSetStatusAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
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
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace PidginPlugin
{

	public class PidginSetStatusAction : Act
	{


		public PidginSetStatusAction ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Set status"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Set pidgin status message"); }
		}

		public override string Icon {
			get { return "pidgin"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (PidginStatusTypeItem);
				yield return typeof (PidginSavedStatusItem);
				yield return typeof (ITextItem);
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				yield return typeof (ITextItem); 
				yield return typeof (PidginStatusTypeItem);
			}
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (items.First () is PidginStatusTypeItem && modItem is ITextItem)
				return true;
			if (items.First () is ITextItem && modItem is PidginStatusTypeItem)
				return true;
			return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			int status;
			string message = "";
			try {
				Pidgin.IPurpleObject prpl = Pidgin.GetPurpleObject ();

				if (items.First () is PidginSavedStatusItem) {
					status = (items.First () as PidginSavedStatusItem).ID;
					prpl.PurpleSavedstatusActivate (status);
				} else if (items.First () is PidginStatusTypeItem) {
					status = (items.First () as PidginStatusTypeItem).Status;
					if (modItems.Any ())
						message = (modItems.First () as ITextItem).Text;
					Pidgin.PurpleSetAvailabilityStatus (status, message);
				} else if (items.First () is ITextItem) {
					if (modItems.Any ())
						status = (modItems.First () as PidginStatusTypeItem).Status;
					else
						status = prpl.PurpleSavedstatusGetType (prpl.PurpleSavedstatusGetCurrent ());
					message = (items.First () as ITextItem).Text;
					Pidgin.PurpleSetAvailabilityStatus (status, message);
				}
			} catch (Exception e) {
				Log<PidginSetStatusAction>.Error ("Could not set Pidgin status: {0}", e.Message);
				Log<PidginSetStatusAction>.Debug (e.StackTrace);
			}

			yield break;
		}
	}
}
