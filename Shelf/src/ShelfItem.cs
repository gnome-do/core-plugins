/* ShelfItem.cs
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
using Mono.Addins;
using Do.Universe;

namespace Shelf
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
		
		public ShelfItem (string name, List<Item> items)
		{
			this.name = name;
			this.items = items;
		}
		
		public string ShelfName {
			get { return this.name; }
			set { this.name = value; }
		}
		
		public override string Name {
			get { return this.name + AddinManager.CurrentLocalizer.GetString (" Shelf"); }
		}
		
		public override string Description {
			get {
				return string.Format (
						AddinManager.CurrentLocalizer.GetString ("Your {0} shelf items."), this.name);
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
		
		public List<Item> ItemList
		{
			get{ return this.items ;}
			set{ this.items = value;}
		}
	}
}
