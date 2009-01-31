using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Unix;

namespace Do.Universe
{
	public class ShelfRemoveFromShelfAction : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Remove From Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Remove Selected Item From Shelf"); }
		}

		public override string Icon {
			get { return "remove"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {
				typeof (Item),
				};
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			return ShelfItemSource.InSomeShelf (item);
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item moditem)
		{
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof(ShelfItem); }
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			foreach (Item i in ShelfItemSource.Shelves.Values)
					yield return i;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (!modItems.Any ())
				ShelfItemSource.RemoveFromDefault (items.First ());
			else
				(modItems.First () as ShelfItem).RemoveItem (items.First ());
			return null;
		}
	}
}