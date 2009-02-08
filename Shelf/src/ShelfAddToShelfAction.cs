/* ShelfAddToShelfAction.cs
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
using Mono.Unix;
using Do.Platform;
using Do.Universe;

namespace Shelf
{	
	public class ShelfAddToShelfAction : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Add To Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Add Selected Item to one Shelf"); }
		}

		public override string Icon {
			get { return "bookmark_add"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (Item); }			
		}

		public override bool SupportsItem (Item item)
		{
			return (!(item is ShelfItem));
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item moditem)
		{
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof(ShelfItem); yield return typeof (ITextItem);}
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			foreach (Item i in ShelfItemSource.Shelves.Values)
					yield return i;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (modItems.Any ())
			{
				if(modItems.First () is ITextItem)
			    {
					string name = (modItems.First () as ITextItem).Text;
					ShelfItemSource.CreateShelf(name);
					ShelfItemSource.Shelves[name].AddItem(items.First ());
					ShelfItemSource.Serialize();
			    }
				else
				{
					(modItems.First () as ShelfItem).AddItem (items.First ());
					ShelfItemSource.Serialize();
				}
			}
			else
			{
				ShelfItemSource.AddToDefault (items.First ());
			}
			yield break;
		}

	}
}
