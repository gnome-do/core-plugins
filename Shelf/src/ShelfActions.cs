// ShelfActions.cs
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
using Mono.Unix;

namespace Do.Universe
{
	
	public class ShelfExploreAction : AbstractAction
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
		
		public override Type[] SupportedItemTypes {
			get { return new Type[] { 
					typeof (ShelfItem), 
				}; 
			}
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			return (items[0] as ShelfItem).Items.ToArray ();
		}
	}
	
	public class ShelfRemoveAction : AbstractAction
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

		public override Type[] SupportedItemTypes {
			get { return new Type[] {
				typeof (IItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			return ShelfItemSource.InShelf (item);
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			if (modItems.Length == 0)
				ShelfItemSource.RemoveFromDefault (items[0]);
			else
				(modItems[0] as ShelfItem).RemoveItem (items[0]);
			
			return null;
		}
	}
	
	public class ShelfAddAction : AbstractAction
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

		public override Type[] SupportedItemTypes {
			get { return new Type[] {
				typeof (IItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			return (!(item is ShelfItem) && !ShelfItemSource.InShelf (item));
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			if (modItems.Length == 0) {
				ShelfItemSource.AddToDefault (items[0]);
			} else {
				(modItems[0] as ShelfItem).AddItem (items[0]);
			}
			return null;
		}

	}
}
