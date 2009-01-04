// ClawsActionNewMail.cs 
// User: Karol Będkowski at 22:16 2008-10-14
//
// Copyright Karol Będkowski 2008

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

using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Unix;
using Do.Universe;

namespace Claws
{
	
	/// <summary>
	/// New mail action (with or without email).
	/// </summary>
	public class ClawsActionNewMail : ClawsActionBase {
		
		protected override string Command () {
			return "claws-mail --compose";
		}
		
		public override string Name {
			get { return Catalog.GetString ("Compose Email"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Compose a new email with ClawsMail"); }
		}
		
		public override string Icon {
			get { return "mail_new"; }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				yield return typeof (ITextItem);
				yield return typeof (ContactItem);	
			}
		}
				
		public override bool ModifierItemsOptional {
			get { return true; }
		}
		

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (!modItems.Any ()) { // no modifier
				System.Diagnostics.Process.Start(this.Command ());
			} else {
				Item firstModItem = modItems.First ();
				string email = string.Empty;
				if (firstModItem is ITextItem) { // modifier as text
					email = (firstModItem as ITextItem).Text;
				} else if (firstModItem is ContactItem) { // modifier as contact
					email = (firstModItem as ContactItem).AnEmailAddress;					
				}
				System.Diagnostics.Process.Start(this.Command () + " " + email);
			}
			yield break;	
		}

		public override bool SupportsModifierItemForItems (System.Collections.Generic.IEnumerable<Item> items, Item modItem)
		{
			if (modItem is ITextItem) {
				return true;
			}
			
			if (modItem is ContactItem) {
				string email = (modItem as ContactItem).AnEmailAddress;
				return !string.IsNullOrEmpty(email);
			}
			
			return true;
		}
	}
}
