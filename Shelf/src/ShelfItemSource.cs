/* ShelfItemSource.cs
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
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Do.Platform;
using Do.Universe;
using Mono.Unix;

namespace Shelf
{		
	public class ShelfItemSource : ItemSource
	{
		
		[Serializable]
		class ItemsRecord {
			
			public string UniqueId { get; protected set; }
			public string Shelf { get; protected set; }
			
			public ItemsRecord (string uniqueId, string shelf)
			{
				UniqueId = uniqueId;
				Shelf = shelf;
			}

			public Item MaybeGetItem ()
			{
				return Services.Core.GetItem (UniqueId);
			}
		}
		
		static string ShelfNamesFile {
			get {
				return Path.Combine (Services.Paths.UserDataDirectory, typeof (ShelfItemSource).FullName);
			}
		}
		
		static string ItemsFile {
			get {
				return Path.Combine (Services.Paths.UserDataDirectory, "ItemsInShelves");
			}
		}
		
		static List<ItemsRecord> AllItems
		{
			get{
				List<ItemsRecord> items =  new List<ItemsRecord>();
				foreach(ShelfItem shelf in ShelfItemSource.shelves.Values)
					foreach(Item i in shelf.Items){
						items.Add(new ItemsRecord(i.UniqueId, shelf.ShelfName));
				}
				return items;
			}
		}

		static ShelfItemSource()
		{
			ShelfItemSource.defaultName = "Default";
			shelvesList = new List<Item>();
			Deserialize ();
			ShelfItemSource.HasDeSerialized = false;
			if(!ShelfItemSource.shelves.ContainsKey("Default")){
				ShelfItemSource.shelves.Add("Default", new ShelfItem("Default"));
				Serialize();
			}
		}
		
		static void Deserialize ()
		{
			ShelfItemSource.shelves = new Dictionary<string,ShelfItem> ();
			
			IEnumerable<string> shelfNames = null;
			try {
				using (Stream s = File.OpenRead (ShelfNamesFile)) {
					BinaryFormatter f = new BinaryFormatter ();
					shelfNames = f.Deserialize (s) as IEnumerable<string>;
				}
			} catch (FileNotFoundException) {
			} catch (Exception e) {
				Log<ShelfItemSource>.Error ("Could not deserialize shelf names: {0}", e.Message);
				Log<ShelfItemSource>.Debug (e.StackTrace);
			}
			
			if(shelfNames == null)
				return;
			foreach(string name in shelfNames){
				ShelfItemSource.shelves.Add(name, new ShelfItem(name)); 
			}
			
			//Deserialize items in shelves
			List<ItemsRecord> itemsInShelves = null;
			try {
				using (Stream s = File.OpenRead (ItemsFile)) {
					BinaryFormatter f = new BinaryFormatter ();
					itemsInShelves = f.Deserialize (s) as List<ItemsRecord>;
				}
			} catch (FileNotFoundException) {
			} catch (Exception e) {
				Log<ShelfItemSource>.Error ("Could not deserialize items : {0}", e.Message);
				Log<ShelfItemSource>.Debug (e.StackTrace);
			}
			
			if (itemsInShelves==null){
				return;
			}
				
			foreach(ItemsRecord itemRecord in itemsInShelves){
				Item t = itemRecord.MaybeGetItem();
				if(t==null){
					continue;
				}
				ShelfItemSource.shelves[itemRecord.Shelf].AddItem(t);
			}			
			ShelfItemSource.HasDeSerialized = true;
		}
		
		public static void Serialize ()
		{
			//serialize shelves...
			try {
				using (Stream s = File.OpenWrite (ShelfNamesFile)) {
					BinaryFormatter f = new BinaryFormatter ();
					f.Serialize (s, new List<string>(ShelfItemSource.shelves.Keys));
				}
			} catch (Exception e) {
				Log<ShelfItemSource>.Error ("Could not serialize shelves names: {0}", e.Message);
				Log<ShelfItemSource>.Debug (e.StackTrace);
			}
			//then serialize the items in the shelves
			try {
				using (Stream s = File.OpenWrite (ItemsFile)) {
					BinaryFormatter f = new BinaryFormatter ();
					f.Serialize (s, AllItems);
				}
			} catch (Exception e) {
				Log<ShelfItemSource>.Error ("Could not serialize items: {0}", e.Message);
				Log<ShelfItemSource>.Debug (e.StackTrace);
			}
		}
				
		static Dictionary<string,ShelfItem> shelves;
		static string defaultName;
		static List<Item> shelvesList;
		static bool HasDeSerialized = false;
		
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
			get { return shelvesList ;}
		}
		
		public override void UpdateItems() 
		{
			
			if(!ShelfItemSource.HasDeSerialized)
			{
				ShelfItemSource.Deserialize();
			}
			if (shelves != null)
			{
				shelvesList.Clear();
				foreach (Item item in shelves.Values){
					shelvesList.Add(item);
				}
			}	
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			return (item as ShelfItem).Items;
		}

		static public void AddToDefault (Item item)
		{
			shelves[defaultName].AddItem (item);
			Serialize();
		}
		
		static public void RemoveFromAll (Item item)
		{			
			foreach(string key in ShelfItemSource.shelves.Keys)
				if(shelves[key].Items.Contains (item))
					shelves[key].RemoveItem(item);
			Serialize();
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
			Serialize();
		}
	}
}
