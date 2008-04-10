// Shelf.cs
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
using System.Collections.Generic;

using Do.Universe;

namespace Shelf
{
	// Plugin Manifesto
	// -- Shelf is fashioned after but not cloned from the shelf plugin to quicksilver.  Ideally
	// shelf will allow users to add any item to the shelf, then retrieve those items for operation.
	// Example Usage:
	// -- Users selects folder item -> add to shelf
	// -- User goes and does other stuff
	// -- User returns, types "Shelf" -> "Folder they added" Enter Key!
	// -- Shelf returns the selected item for operation
	
	public class ShelfContainer
	{
		static List<IItem> items;
		
		static ShelfContainer()
		{
			items = new List<IItem> ();
		}
		
		public static List<IItem> Items {
			get { return items; }
		}
		
	}
	
	public class ShelfItem : IItem
	{
		
		public string Name {
			get {
				return "Shelf";
			}
		}

		public string Description {
			get {
				return "Your Shelf Items";
			}
		}

		public string Icon {
			get {
				return "folder-saved-search";
			}
		}

	}
	
	public class ShelfItemSource : IItemSource
	{
		
		public string Name {
			get {
				return "Shelf Item Source";
			}
		}

		public string Description {
			get {
				return "Your Shelf Items";
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
				items.Add (new ShelfItem ());
				return items;
			}
		}

		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return ShelfContainer.Items;
		}

		public void UpdateItems ()
		{
		}

	}
	
	public class ShelfRemoveAction : AbstractAction
	{
		public override string Name {
			get { return "Remove From Shelf"; }
		}
		
		public override string Description {
			get { return "Remove Selected Item From Shelf"; }
		}

		public override string Icon {
			get { return "remove"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override Type[] SupportedItemTypes {
			get { return new Type[] {
				typeof (IItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
<<<<<<< TREE
			if (ShelfContainer.Items.Contains (item))
				return true;
			return false;
=======
			return ShelfContainer.Items.Contains (item);
>>>>>>> MERGE-SOURCE
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			ShelfContainer.Items.Remove (items[0]);
			
			return null;
		}
	}
	
	
	public class ShelfAddAction : AbstractAction
	{
		public override string Name {
			get { return "Add To Shelf"; }
		}
		
		public override string Description {
			get { return "Add Selected Item to Shelf"; }
		}

		public override string Icon {
			get { return "bookmark_add"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override Type[] SupportedItemTypes {
			get { return new Type[] {
				typeof (IItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			if (item is ShelfItem) return false;
			return true;
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			ShelfContainer.Items.Add (items[0]);
			
			return null;
		}

	}
}





