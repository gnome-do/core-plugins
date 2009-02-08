/* ShelfRenameShelfAction.cs
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
using System.Collections.Generic;
using Mono.Unix;
using System.Linq;
using Do.Universe;

namespace Shelf
{
	public class ShelfRenameShelfAction : Act
	{
		
		public ShelfRenameShelfAction()
		{
		}
		
		public override string Name {
			get { return Catalog.GetString ("Rename a Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Changes a shelf's name"); }
		}

		public override string Icon {
			get { return "folder-saved-search"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return false; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
				get { yield return typeof (ShelfItem); } 
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item moditem)
		{
			return true;
		}		
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem);}
		}

		public override bool SupportsItem (Item item)
		{
			return ((item is ShelfItem) && (item as ShelfItem).ShelfName != "Default");
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string name = (items.First () as ShelfItem).ShelfName;
			ShelfItem shelf = ShelfItemSource.Shelves[name];
			shelf.ShelfName = (modItems.First () as ITextItem).Text;
			ShelfItemSource.Shelves.Add(shelf.ShelfName, shelf);
			ShelfItemSource.Shelves.Remove(name);
			ShelfItemSource.Serialize();
			yield break;
		}
	}
}
