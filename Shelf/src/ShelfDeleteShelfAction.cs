using System;
using System.Collections.Generic;
using Mono.Unix;
using System.Linq;
using Do.Universe;

namespace Shelf
{
	public class ShelfDeleteShelfAction : Act
	{
		
		public ShelfDeleteShelfAction()
		{
		}
		
		public override string Name {
			get { return Catalog.GetString ("Remove a shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Removes a shelf from your shelves"); }
		}

		public override string Icon {
			get { return "remove"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
				get { yield return typeof (ShelfItem); } 
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ShelfItem);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			ShelfItemSource.Shelves.Remove((items.First () as ShelfItem).Name);
			yield break;
		}
	}
}