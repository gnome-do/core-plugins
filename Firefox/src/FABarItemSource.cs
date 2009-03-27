// FABarItemSource.cs -- Modification of BookmarkItemSource
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Do.Universe;
using Do.Universe.Common;

using Mono.Data.SqliteClient;
using Mono.Unix;

namespace Mozilla.Firefox
{
	public class FABarItemSource : ItemSource
	{
		const string BeginProfileName = "Path=";
		const string BeginDefaultProfile = "Default=1";

		IEnumerable<BookmarkItem> bookmarks;

		public FABarItemSource ()
		{
			bookmarks = Enumerable.Empty<BookmarkItem> ();
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (BookmarkItem); }
		}

		public override string Name {
			get { return Catalog.GetString ("Firefox Awesome Bar"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Searches Bookmarks and History."); }
		}

		public override string Icon {
			get { return "firefox-3.0"; }
		}

		public override IEnumerable<Item> Items {
			get { return bookmarks.OfType<Item> (); }
		}

		public override void UpdateItems ()
		{
			bookmarks = LoadBookmarkItems ();
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
		static string storedProfilePath;
		static string ProfilePath {
			get {
				string line, profile, path, home;

				if (!String.IsNullOrEmpty(storedProfilePath)) return storedProfilePath;
				
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
				return storedProfilePath =
					Path.Combine (Path.Combine (home, ".mozilla/firefox"), profile);
			}
		}

		/// <summary>
		/// Looks at the file currently saved in the temp folder and sees if it
		/// needs to be updated.
		/// </summary>
		/// <returns>
		/// The path of the current database file in memory.
		/// </returns>
		static string storedTempDatabasePath;
		static string tempDatabasePath {
			get {
				// Create a reference to the current firefox file.
				string firefoxPath = Path.Combine (ProfilePath, "places.sqlite");				
				
				// Check if the stored temp file exists.				
				if  ((String.IsNullOrEmpty(storedTempDatabasePath)) || (!File.Exists(storedTempDatabasePath))) {
					// If it doesn't, make one.
					string copyPath = Path.GetTempFileName();
					System.IO.File.Copy(firefoxPath, copyPath, true);
					storedTempDatabasePath = copyPath;
				}
				else if (File.Exists(storedTempDatabasePath))
				{
					FileInfo currentFileInfo = new FileInfo(firefoxPath);
					FileInfo firefoxFileInfo = new FileInfo(storedTempDatabasePath);
					
					if (currentFileInfo.Length != firefoxFileInfo.Length) 
						System.IO.File.Copy(firefoxPath, storedTempDatabasePath, true);
				}
				
				return storedTempDatabasePath;
			}
		}

		IEnumerable<BookmarkItem> LoadBookmarkItems ()
		{
			string connectionString = String.Format ("URI=file:{0},version=3",tempDatabasePath);
			
			using (IDbConnection dbcon = (IDbConnection) new SqliteConnection(connectionString)) {
				using (IDbCommand dbcmd = dbcon.CreateCommand()) {
					dbcmd.CommandText = "SELECT title, url FROM moz_places ORDER BY frecency DESC";
					using (IDataReader reader = dbcmd.ExecuteReader()) {
						while(reader.Read()) {
							string title = reader.GetString (0);
							string url = reader.GetString (1);
							if (url[0] != 'p') yield return new BookmarkItem (title, url);
						}
					}
				}
			}
		}
	}
}  


