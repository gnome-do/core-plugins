// ShelfActions.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
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
using System.Linq;
using Mono.Unix;

namespace Do.Universe
{
	
	public class ShelfExploreAction : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Explore Shelf"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Get a list of everything in your shelf"); }
		}

		public override string Icon {
			get { return "folder-saved-search"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] { 
					typeof (ShelfItem), 
				}; 
			}
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			return (items.First () as ShelfItem).Items.ToArray ();
		}
	}
	
	public class ShelfRemoveAction : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Remove From Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Remove Selected Item From Shelf"); }
		}

		public override string Icon {
			get { return "remove"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {
				typeof (Item),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			return ShelfItemSource.InShelf (item);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (!modItems.Any ())
				ShelfItemSource.RemoveFromDefault (items.First ());
			else
				(modItems.First () as ShelfItem).RemoveItem (items.First ());
			
			return null;
		}
	}
	
	public class ShelfAddAction : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Add To Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Add Selected Item to Shelf"); }
		}

		public override string Icon {
			get { return "bookmark_add"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {
				typeof (Item),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			return (!(item is ShelfItem) && !ShelfItemSource.InShelf (item));
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (!modItems.Any ()) {
				ShelfItemSource.AddToDefault (items.First ());
			} else {
				(modItems.First () as ShelfItem).AddItem (items.First ());
			}
			return null;
		}

	}
}
