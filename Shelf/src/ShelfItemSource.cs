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
	public class ShelfItem : IItem
	{
		private string name;
		
		private List<IItem> items = new List<IItem> ();
		
		public List<IItem> Items {
			get {
				return items ?? items = new List<IItem> ();
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
		
		public string Name {
			get {
				return name + Catalog.GetString (" Shelf");
			}
		}
		
		public string Description {
			get {
				return Catalog.GetString ("Your ") + name + Catalog.GetString (" Shelf Items");
			}
		}

		public string Icon {
			get {
				return "folder-saved-search";
			}
		}
		
		public void AddItem (IItem item)
		{
			if (Items.Contains (item)) return;
		
			Items.Add (item); //temp items
		}
		
		public void RemoveItem (IItem item)
		{
			Items.Remove (item);
		}
	}
	
	public class ShelfItemSource : IItemSource
	{	
		
		static Dictionary<string,ShelfItem> shelf;
		static string defaultName;
		
		public string Name {
			get {
				return Catalog.GetString ("Shelf Item Source");
			}
		}

		public string Description {
			get {
				return Catalog.GetString ("Your Shelf Items");
			}
		}

		public string Icon {
			get {
				return "folder-saved-search";
			}
		}

		public Type[] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ShelfItem),
				};
			}
		}

		public ICollection<IItem> Items {
			get {
				List<IItem> items = new List<IItem> ();
				if (shelf != null) {
					foreach (IItem item in shelf.Values)
						items.Add (item);
				}
				
				return items;
			}
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
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

		public void UpdateItems ()
		{
		}
		
		static public void AddToDefault (IItem item)
		{
			shelf[defaultName].AddItem (item);
		}
		
		static public void RemoveFromDefault (IItem item)
		{
			shelf[defaultName].RemoveItem (item);
		}
		
		static public bool InShelf (IItem item)
		{
			return shelf[defaultName].Items.Contains (item);
		}
	}
}
