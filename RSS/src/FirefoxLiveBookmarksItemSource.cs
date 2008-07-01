/* FirefoxLiveBookmarksItemSource.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Universe;

namespace Do.Plugins.Rss
{
	public class FirefoxLiveBookmarksItemSource : IItemSource
	{
		const string BeginProfileName = "Path=";
		const string BeginDefaultProfile = "Default=1";
		const string BeginURL = "<DT><A HREF=\"";
		const string EndURL = "\"";
		const string BeginShortcut = "SHORTCUTURL=\"";
		const string EndShortcut = "\"";
		const string BeginName = "\">";
		const string EndName = "</A>";
		List<IItem> bookmarks;

		/// <summary>
		/// Initialize the item source.
		/// </summary>
		public FirefoxLiveBookmarksItemSource ()
		{
			bookmarks = new List<IItem> ();
			//UpdateItems ();
		}

		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (RssFeedItem),
				};
			}
		}

		public string Name {
			get { return "Firefox Live Bookmarks"; }
		}

		public string Description {
			get { return "Finds Firefox Live bookmarks in your default profile."; }
		}

		public string Icon {
			get { return "firefox"; }
		}

		public ICollection<IItem> Items {
			get { return bookmarks; }
		}

		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}

		public void UpdateItems ()
		{
			bookmarks.Clear ();
			string path = GetFirefoxBookmarkFilePath ();
			string firefox3Path = Path.Combine (path, "bookmarks.postplaces.html");
			string firefox2Path = Path.Combine (path, "bookmarks.html");
			// Get Firefox 3 live bookmarks 
			foreach (IItem item in ReadBookmarksFromFile (firefox3Path))
				bookmarks.Add (item);
			// Get Firefox 2 live bookmarks
			foreach (IItem item in ReadBookmarksFromFile (firefox2Path))
				bookmarks.Add (item);
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
		public static string GetFirefoxBookmarkFilePath ()
		{
			string home, path, profile;
			StreamReader reader;

			profile = null;
			home = System.Environment.GetFolderPath 
				(System.Environment.SpecialFolder.Personal);
			path = Path.Combine (home, ".mozilla/firefox/profiles.ini");
			try {
				reader = File.OpenText (path);
			} catch {
				return null;
			}

			for (string line = reader.ReadLine (); line != null; 
					line = reader.ReadLine ()) {
				if (line.StartsWith (BeginDefaultProfile)) break;
				if (line.StartsWith (BeginProfileName)) {
					line = line.Trim ();
					line = line.Substring (BeginProfileName.Length);
					profile = line;
				}
			}
			reader.Close ();

			if (profile == null) {
				return null;
			}
			path = Path.Combine (home, ".mozilla/firefox");
			path = Path.Combine (path, profile);
			return path;
		}

		/// <summary>
		/// Given a bookmarks file, create a RssFeedItem for each bookmark found
		/// in the file, returning a collection of BookmarkItems created.
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/> containing the absolute path to a 
		/// Firefox bookmarks.postplaces.html file (e.g. the path returned by 
		/// GetFirefoxBookmarkFilePath).
		/// </param>
		/// <returns>
		/// A <see cref="ICollection`1"/> of RssFeedItems.
		/// </returns>
		protected ICollection<RssFeedItem> ReadBookmarksFromFile (string file)
		{
			ICollection<RssFeedItem> list;
			string link, title;
			Regex regex = new Regex (@"FEEDURL=""([^""]+)""[^>]+>([^<]+)");
			list = new List<RssFeedItem> ();
			try {    
				using (StreamReader reader = File.OpenText (file)) {  
					string content = reader.ReadToEnd ();
					MatchCollection matches = regex.Matches (content);
					foreach (Match match in matches) {
						link = match.Groups[1].Value;
						title = match.Groups[2].Value;
						list.Add (new RssFeedItem (title, link));
					}
				}
			} catch (Exception e) {
				// Something went horribly wrong, so we print the error message.
				Console.Error.WriteLine ("Could not read live bookmarks from " +
						"firefox bookmarks file {0}: {1}", file, e.Message);
			}
			return list;

		}
	}
}
