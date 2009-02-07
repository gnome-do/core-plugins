using System;
using System.Collections.Generic;
using Mono.Unix;
using System.Linq;

namespace Do.Universe
{
	public class ShelfCreateShelfAction : Act
	{
		
		public ShelfCreateShelfAction()
		{
		}
		
		public override string Name {
			get { return Catalog.GetString ("Create a new Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Adds a new Shelf"); }
		}

		public override string Icon {
			get { return "folder-saved-search"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
				get { yield return typeof (ITextItem); } 
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ITextItem);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			ShelfItemSource.CreateShelf((items.First () as ITextItem).Text);
			yield break;
		}
	}
}
