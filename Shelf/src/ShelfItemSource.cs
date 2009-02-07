// ShelfItemSource.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free SoftwDictionary<string,ShelfItem> shelvesare Foundation; either version 2 of the License, or
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

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Do.Platform;
using Do.Universe;

using Mono.Unix;

using Do.Universe;

namespace Shelf
{		
	public class ShelfItemSource : ItemSource
	{	
		
		static Dictionary<string,ShelfItem> shelves;
		static string defaultName;
		
		static ShelfItemSource()
		{
			shelves = new Dictionary<string,ShelfItem> ();
		}
		
		public static Dictionary<string,ShelfItem> Shelves {
			get { return shelves; }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Shelves"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Your Shelves"); }
		}

		public override string Icon {
			get { return "folder-saved-search"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ShelfItem); }
		}

		public override IEnumerable<Item> Items {
			get {
				//problem here with duplicity
				if (shelves != null) {
					foreach (Item item in shelves.Values)
						yield return item;
				}
			}
		}
		
		public override void UpdateItems ()
		{
			Log<ShelfItemSource>.Debug("Total # of shelves: "+ ShelfItemSource.Shelves.Values.Count);
			
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			return (item as ShelfItem).Items;
		}
		
		public ShelfItemSource ()
		{
			defaultName = Catalog.GetString ("Default");
			
			if (shelves.Count == 0) {
				shelves.Add (defaultName, new ShelfItem (defaultName));
			}
		}

		static public void AddToDefault (Item item)
		{
			shelves[defaultName].AddItem (item);
		}
		
		static public void RemoveFromAll (Item item)
		{			
			foreach(string key in ShelfItemSource.shelves.Keys)
				if(shelves[key].Items.Contains (item))
					shelves[key].RemoveItem(item);
		}
		
		static public bool InSomeShelf (Item item)
		{
			bool b = false;
			foreach(string key in ShelfItemSource.shelves.Keys){
				b |= shelves[key].Items.Contains (item);
			}	
			return b;
		}
		
		static public void CreateShelf (string name)
		{
			shelves.Add (name, new ShelfItem (name));			
		}
	}
}
