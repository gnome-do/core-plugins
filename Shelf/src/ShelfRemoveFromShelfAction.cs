/* ShelfRemoveFromShelfAction.cs
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
using System.Linq;
using Mono.Addins;
using Do.Universe;

namespace Shelf
{
	public class ShelfRemoveFromShelfAction : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Remove From Shelf"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Remove Selected Item From Shelf"); }
		}

		public override string Icon {
			get { return "remove"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (Item); }
		}
		
		public override bool SupportsItem (Item item)
		{
			return ShelfItemSource.InSomeShelf (item);
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item moditem)
		{
			return (moditem as ShelfItem).Items.Contains(items.First());
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof(ShelfItem); }
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			foreach(string key in ShelfItemSource.Shelves.Keys)
				if(ShelfItemSource.Shelves[key].Items.Contains (item))
					yield return ShelfItemSource.Shelves[key];
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (!modItems.Any ())
				ShelfItemSource.RemoveFromAll (items.First ());
			else
			{
				(modItems.First () as ShelfItem).RemoveItem (items.First ());
				ShelfItemSource.Serialize();
			}
			yield break;
		}
	}
}