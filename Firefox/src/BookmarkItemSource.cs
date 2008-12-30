/* BookmarkItemSource.cs
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
using System.Linq;
using System.Collections.Generic;

using Do.Universe;
using Do.Universe.Common;

using Mono.Unix;

namespace Mozilla.Firefox {

	public class BookmarkItemSource : ItemSource {
		const string BeginProfileName = "Path=";
        const string BeginDefaultProfile = "Default=1";
        
        const string BeginTitle = "\"title\":\"";
        const string BeginUri = "\"uri\":\"";
        const string BeginChildren = "\"children\":[";
		
		IEnumerable<BookmarkItem> bookmarks;
		
		public BookmarkItemSource ()
		{
			bookmarks = Enumerable.Empty<BookmarkItem> ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (BookmarkItem); }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Firefox Bookmarks"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Finds Firefox bookmarks in your default profile."); }
		}
		
		public override string Icon {
			get { return "firefox-3.0"; }
		}
		
		public override IEnumerable<Item> Items {
			get { return bookmarks.OfType<Item> (); }
		}
		
		public override void UpdateItems ()
		{
			// No updating for now -- Firefox only dumps JSON bookmarks
			// data once each day.
			if (bookmarks.Any ()) return;
			bookmarks = LoadBookmarkItems ().ToArray ();
		}
		
		/// <summary>
		/// Looks in the firefox profiles file (~/.mozilla/firefox/profiles.ini)
		/// for the name of the default profile, and returns the path to the
		/// default profile.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> containing the absolute path to the
		/// bookmarks.html file of the default firefox profile for the current
		/// user.
		/// </returns>
		static string profile_path;
		static string ProfilePath {
			get {
				string line, profile, path, home;
				
				if (null != profile_path) return profile_path;

				home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				profile = null;
				path = Path.Combine (home, ".mozilla/firefox/profiles.ini");
				using (StreamReader r = File.OpenText (path)) {
					while (null != (line = r.ReadLine ())) {
						if (line.StartsWith (BeginDefaultProfile)) break;
						if (line.StartsWith (BeginProfileName)) {
							line = line.Trim ();
							line = line.Substring (BeginProfileName.Length);
							profile = line;
						}
					}
				}
				return profile_path =
					Path.Combine (Path.Combine (home, ".mozilla/firefox"), profile);
			}
		}
				
		static string BookmarkJSONPath {
			get {
				string dir;
				string[] backups;
				
				dir = Path.Combine (ProfilePath, "bookmarkbackups");
				backups = Directory.GetFiles (dir, "*.json");
				Array.Sort (backups);
				return Path.Combine (dir, backups [backups.Length-1]);
			}
		}
		
		IEnumerable<BookmarkItem> LoadBookmarkItems () {
			int iTitle, iUri, iChildren;
			string json = File.ReadAllText (BookmarkJSONPath);
			
			iTitle = iUri = iChildren = 0;
			while (true) {
				iTitle = json.IndexOf (BeginTitle, iUri);
				if (iTitle < 0) break;
				iTitle += BeginTitle.Length;
				
				iChildren = json.IndexOf (BeginChildren, iTitle);
				
				iUri = json.IndexOf (BeginUri, iTitle);
				if (iUri < 0) break;
				iUri += BeginUri.Length;
				
				if (iUri < iChildren) {
					string title, uri;
					title = json.Substring (iTitle,
						json.IndexOf ("\"", iTitle) - iTitle);
					uri = json.Substring (iUri,
						json.IndexOf ("\"", iUri) - iUri);
					if (string.IsNullOrEmpty (title) ||
						uri.StartsWith ("place:")) continue;
					yield return new BookmarkItem (title, uri);
				}
			}
		}
	}
}
