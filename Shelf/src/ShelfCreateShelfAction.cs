/* ShelfCreateShelfAction.cs
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
using Mono.Unix;
using Do.Universe;
using System.Linq;

namespace Shelf
{
	public class ShelfCreateShelfAction : Act
	{
		
		public ShelfCreateShelfAction()
		{
		}
		
		public override string Name {
			get { return Catalog.GetString ("Create a new Shelf"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Adds a new Shelf"); }
		}

		public override string Icon {
			get { return "folder-saved-search"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
				get { yield return typeof (ITextItem); } 
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ITextItem);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			ShelfItemSource.CreateShelf((items.First () as ITextItem).Text);
			yield break;
		}
	}
}
