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

using Mono.Addins;

using Do.Universe;
using Do.Universe.Common;
using Do.Platform;

namespace Epiphany
{

	public class EpiphanyBookmarkItemSource : ItemSource
	{
		List<Item> items;

		public EpiphanyBookmarkItemSource ()
		{
			items = new List<Item> ();
		}

		public override string Name { get { return AddinManager.CurrentLocalizer.GetString ("Epiphany Bookmarks"); } }
		
		public override string Description { 
			get { return AddinManager.CurrentLocalizer.GetString ("Indexes your Epiphany bookmarks."); }
		}
		
		public override string Icon { get { return "gnome-web-browser"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (BookmarkItem); 
				yield return typeof (IApplicationItem);
				yield return typeof (EpiphanyBrowseBookmarksItem);
			}
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			if (parent is IApplicationItem && (parent as IApplicationItem).Exec.Contains ("epiphany-browser"))
				yield return new EpiphanyBrowseBookmarksItem ();
			if (parent is EpiphanyBrowseBookmarksItem) {
				foreach (BookmarkItem item in Items)
					yield return item;
			}
			yield break;
		}

		public override void UpdateItems ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string bookmarksFile = "~/.gnome2/epiphany/bookmarks.rdf".Replace ("~", home);

			items.Clear ();
			try {
				using (XmlReader reader = XmlReader.Create (bookmarksFile)) {
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
				Log.Error ("Could not read Epiphany Bookmarks file {0}: {1}", bookmarksFile, e.Message);
				Log.Debug (e.StackTrace);
			}
		}

	}
}

