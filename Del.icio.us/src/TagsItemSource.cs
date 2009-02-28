/* TagsItemSource.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
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

namespace Delicious
{
	public class TagsItemSource : ItemSource
	{
		public override string Name {
			get { return Catalog.GetString ("Del.icio.us Tags"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Organizes your del.icio.us bookmarks by tag"); }
		}
		
		public override string Icon {
			get { return "delicious.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (TagItem); }
		}
		
		public override IEnumerable<Item> Items {
			get {
				foreach (string tag in Delicious.Tags.Keys) {
					yield return new TagItem (tag);
				}
			}
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			return Delicious.Tags [(item as TagItem).Name];
		}
		
		public override void UpdateItems ()
		{
		}
	}
}
