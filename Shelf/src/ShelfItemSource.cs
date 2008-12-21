// ShelfItemSource.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Do.Universe;
using Mono.Unix;

namespace Do.Universe
{	
	public class ShelfItem : Item
	{
		private string name;
		
		private List<Item> items = new List<Item> ();
		
		public List<Item> Items {
			get {
				return items ?? items = new List<Item> ();
			}
		}
		
		public ShelfItem (string name)
		{
			this.name = name;
		}
		
		public string ShelfName {
			get {
				return name;
			}
		}
		
		public override string Name {
			get {
				return name + Catalog.GetString (" Shelf");
			}
		}
		
		public override string Description {
			get {
				return Catalog.GetString ("Your ") + name + Catalog.GetString (" Shelf Items");
			}
		}

		public override string Icon {
			get {
				return "folder-saved-search";
			}
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
	
	public class ShelfItemSource : ItemSource
	{	
		
		static Dictionary<string,ShelfItem> shelf;
		static string defaultName;
		
		public override string Name {
			get {
				return Catalog.GetString ("Shelf Item Source");
			}
		}

		public override string Description {
			get {
				return Catalog.GetString ("Your Shelf Items");
			}
		}

		public override string Icon {
			get {
				return "folder-saved-search";
			}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ShelfItem),
				};
			}
		}

		public override IEnumerable<Item> Items {
			get {
				List<Item> items = new List<Item> ();
				if (shelf != null) {
					foreach (Item item in shelf.Values)
						items.Add (item);
				}
				
				return items;
			}
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			return (item as ShelfItem).Items;
		}

		static ShelfItemSource ()
		{
			shelf = new Dictionary<string,ShelfItem> ();
		}
		
		public ShelfItemSource ()
		{
			defaultName = Catalog.GetString ("Default");
			
			if (shelf.Count == 0) {
				shelf.Add (defaultName, new ShelfItem (defaultName));
			}
		}

		public override void UpdateItems ()
		{
		}
		
		static public void AddToDefault (Item item)
		{
			shelf[defaultName].AddItem (item);
		}
		
		static public void RemoveFromDefault (Item item)
		{
			shelf[defaultName].RemoveItem (item);
		}
		
		static public bool InShelf (Item item)
		{
			return shelf[defaultName].Items.Contains (item);
		}
	}
}
