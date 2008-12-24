// WoofAction.cs
//
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
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

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Do.Universe;



using Do.Platform;


namespace Woof {

	public class WoofSendFileAction : Act {

		public override string Name {
			get { return "Woof!"; }
		}
		public override string Description {
			get { return "Send a file to this person via Woof"; }
		}
		public override string Icon {
			get { return "package-x-generic"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (ContactItem), };
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem) {
				foreach (string detail in (item as ContactItem).Details)
					if (detail.StartsWith ("prpl-"))
						return true;
			}
			return false;
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type[] { typeof (IFileItem) };
			}
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}


		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return true;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Item item = items.First ();
			Item moditem = modItems.First ();
			string name = null;

			if (item is ContactItem && moditem is IFileItem) {
				// Just grab the first protocol we see.
				ContactItem contact = item as ContactItem;
				foreach (string detail in contact.Details) {
					if (detail.StartsWith ("prpl-")) {
						name = contact[detail];
						// If this buddy is online, break, else keep looking.
						if (Pidgin.BuddyIsOnline (name)) break;
					}
				}
			}

			if (name != null) {
				new Thread ((ThreadStart) delegate {
						IFileItem file = moditem as IFileItem;
						WoofServer ws = new WoofServer (name);
						ws.ServeFile(file.Path);
						return;
						}).Start ();
			}

			return null;
		}
	}

	public class WoofSendToAction : Act {

		public override string Name {
			get { return "Woof!"; }
		}
		public override string Description {
			get { return "Send the file via Woof"; }
		}
		public override string Icon {
			get { return "package-x-generic"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (IFileItem), };
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is IFileItem) {
				return true;
			}
			return false;
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type[] { typeof (ContactItem) };
			}
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (modItem is ContactItem) {
				foreach (string detail in (modItem as ContactItem).Details)
					if (detail.StartsWith ("prpl-"))
						return true;
			}
			return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Item item = items.First ();
			Item moditem = modItems.First ();
			string name = null;

			if (moditem is ContactItem && item is IFileItem) {
				// Just grab the first protocol we see.
				ContactItem contact = moditem as ContactItem;
				foreach (string detail in contact.Details) {
					if (detail.StartsWith ("prpl-")) {
						name = contact[detail];
						// If this buddy is online, break, else keep looking.
						if (Pidgin.BuddyIsOnline (name)) break;
					}
				}
			}

			if (name != null) {
				new Thread ((ThreadStart) delegate {
						IFileItem file = item as IFileItem;
						WoofServer ws = new WoofServer (name);
						ws.ServeFile(file.Path);
						return;
						}).Start ();
			}

			return null;
		}
	}
}
