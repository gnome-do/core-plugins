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

using Do.Universe;
using Mono.Unix;

namespace Delicious
{
	public class TagsItemSource : IItemSource
	{
		public string Name {
			get { return Catalog.GetString ("Del.icio.us Tags"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Organizes your del.icio.bookmarks by tag"); }
		}
		
		public string Icon {
			get { return "delicious.png@" + GetType ().Assembly.FullName; }
		}
		
		public Type [] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (TagItem),
				};
			}
		}
		
		public ICollection<IItem> Items {
			get {
				List<IItem> tags = new List<IItem> ();
				foreach (string tag in Delicious.Tags.Keys) {
					tags.Add (new TagItem (tag));
				}
				return tags;
			}
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return Delicious.Tags [(item as TagItem).Name];
		}
		
		public void UpdateItems ()
		{
		}
	}
}
