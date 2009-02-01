using System;
using System.Collections.Generic;
using Mono.Unix;

namespace Do.Universe
{
	public class ShelfItem : Item
	{
		string name;
		List<Item> items = new List<Item> ();
		
		public List<Item> Items {
			get { return items; }
		}
		
		public ShelfItem (string name)
		{
			this.name = name;
		}
		
		public string ShelfName {
			get { return name; }
			set { this.name = value; }
		}
		
		public override string Name {
			get { return name + Catalog.GetString (" Shelf"); }
		}
		
		public override string Description {
			get {
				return string.Format (
						Catalog.GetString ("Your {0} shelf items."), name);
			}
		}

		public override string Icon {
			get { return "folder-saved-search"; }
		}
		
		public void AddItem (Item item)
		{
			if (Items.Contains (item)) return;		
			Items.Add (item); //temp items
		}
		
		public void RemoveItem (Item item)
		{
			Items.Remove (item);
		}
	}
}
