/* EpiphanyBookmarkItemSource.cs
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
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Mono.Unix;
using Do.Addins;
using Do.Universe;

namespace Epiphany
{

	public class EpiphanyBookmarkItemSource : IItemSource
	{
		List<IItem> items;

		public EpiphanyBookmarkItemSource ()
		{
			items = new List<IItem> ();
		}

		public string Name { get { return Catalog.GetString ("Epiphany Bookmarks"); } }
		
		public string Description { 
			get { return Catalog.GetString ("Indexes your Epiphany bookmarks."); }
		}
		public string Icon { get { return "gnome-web-browser"; } }

		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (BookmarkItem),
				};
			}
		}

		public ICollection<IItem> Items
		{
			get { return items; }
		}

		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;	
		}

		public void UpdateItems ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string bookmarks_file = "~/.gnome2/epiphany/bookmarks.rdf".Replace ("~", home);


			items.Clear ();
			try {
				using (XmlReader reader = XmlReader.Create (bookmarks_file)) {
					while (reader.ReadToFollowing ("item")) {
						string title, link;
						
						reader.ReadToFollowing ("title");
						title = reader.ReadString ();
						reader.ReadToFollowing ("link");
						link = reader.ReadString ();

						items.Add (new BookmarkItem (title, link));
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Epiphany Bookmarks file {0}: {1}",
						bookmarks_file, e.Message);
			}
		}

	}
}

