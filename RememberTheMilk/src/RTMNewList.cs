/* RTMListRename.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	public class RTMNewList : Act
	{
		public override string Name {
			get { return Catalog.GetString ("New List"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Create a new task list."); }
        	}
			
		public override string Icon {
			get { return "list-add.png@" + GetType ().Assembly.FullName; }
		}
		
		public bool IsValidListName(string newName) {
			string[] invalidNames = {"Inbox", "Sent", "All Tasks"};
			bool isValid = true;
			
			foreach(string name in invalidNames) {
				if (newName == name)
					isValid = false;
			}
			
			return isValid;
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				return new Type[] {
					typeof (RTMListItem),
				};
			}
		}
        
		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override bool SupportsItem (Item item) {
			return true;
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> item, Item modItem) 
		{
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			string newListName = (items.First () as ITextItem).Text;
			
			// Check if user is empty.
			if (string.IsNullOrEmpty(newListName)) {
				// Need new name.
				Services.Notifications.Notify("Remember The Milk",
					"No new list name provided.");
				yield break;
			}

			if(!IsValidListName(newListName)) {
				Services.Notifications.Notify("Remember The Milk",
					"Invalid list name provided.");
				yield break;
			}
			
            		Services.Application.RunOnThread (() => {
				RTM.NewList (newListName);
			});
			yield break;
		}
	}
}
