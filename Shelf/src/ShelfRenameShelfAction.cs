using System;
using System.Collections.Generic;
using Mono.Unix;
using System.Linq;

namespace Do.Universe
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
			ShelfItemSource.Shelves.Add(shelf.Name, shelf);
			ShelfItemSource.Shelves.Remove(name);
			yield break;
		}
	}
}
