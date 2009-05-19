// PlacesItemSource.cs -- Modification of BookmarkItemSource
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

using Do.Platform;
using Do.Platform.Common;
using Do.Universe;
using Do.Universe.Common;

using Mono.Unix;
using Mono.Data.SqliteClient;
	
namespace Firefox
{
	public class PlacesItemSource : ItemSource
	{
		const string BeginProfileName = "Path=";
		const string BeginDefaultProfile = "Default=1";
		
		// Collections for Storing Items to be used by Do
		List<Item> items;
		IEnumerable<PlaceItem> places;
		IEnumerable<FolderItem> folders;
		
		string stored_temp_db_path;
		
		public PlacesItemSource ()
		{
			items = new List<Item> ();
			places = Enumerable.Empty<PlaceItem> ();			
			folders = Enumerable.Empty<FolderItem> ();

			ProfilePath = FindProfilePath ();
		}

		~PlacesItemSource ()
		{
			try {
				if (File.Exists (stored_temp_db_path))
					File.Delete (stored_temp_db_path);
			} catch (IOException e) {
				Log<PlacesItemSource>.Error ("Could not delete stored db file: {0}", e.Message);
				Log<PlacesItemSource>.Debug (e.StackTrace);
			}
		}

		public override string Name {
			get { return Catalog.GetString ("Firefox Places"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Search your bookmarks and history."); }
		}

		public override string Icon {
			get { return "firefox-3.0"; }
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override IEnumerable<Type> SupportedItemTypes 
		{
			get {
				yield return typeof (PlaceItem); 
				yield return typeof (FolderItem);
				yield return typeof (IApplicationItem);				
				yield return typeof (BrowseHistoryItem);				
				yield return typeof (BrowseBookmarkItem);				
			}
		}

		public override IEnumerable<Item> ChildrenOfItem (Item item) 
		{
			if (IsFirefox (item)) {
				yield return new BrowseHistoryItem ();
				yield return new BrowseBookmarkItem ();
			} else if (item is BrowseBookmarkItem || item is BrowseHistoryItem) {
				foreach (FolderItem folder in folders)
					yield return folder;
				
				if (item is BrowseBookmarkItem)
					foreach (PlaceItem place in PlacesWithParent)
						yield return place;
				else
					foreach (PlaceItem place in places)
						yield return place;
			} else if (item is FolderItem) {
				FolderItem parent = (FolderItem) item;
				
				foreach (FolderItem folder in folders.Where (folder => folder.ParentId == parent.Id))
					yield return folder;
				
				foreach (PlaceItem place in PlacesWithParent.Where (place => place.ParentId == parent.Id))
					yield return place;
			}
		}
		
		public override void UpdateItems ()
		{
			places = LoadPlaceItems ();
			folders = LoadFolderItems ();
			
			items.Clear();

			items.Add (new BrowseHistoryItem ());
			items.Add (new BrowseBookmarkItem ());
			
			items.AddRange (folders.OfType<Item> ());
			items.AddRange (PlacesWithParent.OfType<Item> ());
		}

		IEnumerable<PlaceItem> PlacesWithParent {
			get { return places.Where (place => place.ParentId.HasValue); }
		}
		
		bool IsFirefox (Item item) 
		{
			return item.Equals (Do.Platform.Services.UniverseFactory.MaybeApplicationItemFromCommand ("firefox"));
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
		string ProfilePath { get; set; }

		string FindProfilePath ()
		{
			string line, profile, path, home;

			profile = null;
			home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			
			path = Path.Combine (home, ".mozilla/firefox/profiles.ini");
			using (StreamReader r = File.OpenText (path)) {
				while ((line = r.ReadLine ()) != null) {
					if (line.StartsWith (BeginDefaultProfile)) {
						break;
					} else if (line.StartsWith (BeginProfileName)) {
						line = line.Trim ();
						line = line.Substring (BeginProfileName.Length);
						profile = line;
					}
				}
			}
			return new [] {home, ".mozilla", "firefox", profile}.Aggregate (Path.Combine); 
		}


		string FirefoxDBPath {
			get {
				return Path.Combine (ProfilePath, "places.sqlite");
			}
		}
		
		/// <summary>
		/// Looks at the file currently saved in the temp folder and sees if it
		/// needs to be updated.
		/// </summary>
		/// <returns>
		/// The path of the current database file in memory.
		/// </returns>
		string TempDatabasePath {
			get {				
				// Check if the stored temp file exists and if it doesn't, make one.
				if  (string.IsNullOrEmpty (stored_temp_db_path) || !File.Exists (stored_temp_db_path)) {
					stored_temp_db_path = Path.GetTempFileName ();
					System.IO.File.Copy (FirefoxDBPath, stored_temp_db_path, true);
				} else if (File.Exists (stored_temp_db_path)) {
					FileInfo firefoxDBFileInfo = new FileInfo (FirefoxDBPath);
					FileInfo tempDBFileInfo = new FileInfo (stored_temp_db_path);
					
					if (firefoxDBFileInfo.LastWriteTimeUtc > tempDBFileInfo.LastWriteTimeUtc)
						System.IO.File.Copy (FirefoxDBPath, stored_temp_db_path, true);
				}
				return stored_temp_db_path;
			}
		}
		
		/// <summary>
		/// Creates the current SQL connection string to the temporary database in use. 
		/// </summary>
		/// <returns>
		/// The current SQL connection string to the temporary database in use.
		/// </returns>
		string ConnectionString {
			get { return String.Format ("URI=file:{0},version=3", TempDatabasePath); }
		}

		/// <summary>
		/// Opens a connection to the current temporary database copy, 
		/// and searches for bookmark directories, then adds those to 
		/// the IEnumerable collection to be returned. 
		/// </summary>
		/// <returns>
		/// A collection of bookmark directory Folder Items.
		/// </returns>
		IEnumerable<FolderItem> LoadFolderItems ()
		{
			using (IDbConnection dbcon = (IDbConnection) new SqliteConnection (ConnectionString)) {
				dbcon.Open ();

				using (IDbCommand dbcmd = dbcon.CreateCommand ())
				{
					dbcmd.CommandText = "SELECT title, id, parent "
						+ "FROM moz_bookmarks "
						+ "WHERE type = 2";
						
					using (IDataReader reader = dbcmd.ExecuteReader ()) 
					{
						while (reader.Read ()) {
							string title = reader.GetString (0);							
							int id = reader.GetInt32 (1);
							int parent = reader.GetInt32 (2);

							/* Firefox's parent system uses the field with ID 1 as a 
							 * parent for all other directories. It doesn't have a name,
							 * so we'll give it one. */							
							if (id == 1)
								yield return new FolderItem (Catalog.GetString ("Mozilla Bookmarks"), id, parent);
							/* Firefox uses another field that doesn't have a name.
							 * It references portions of their menu that generate
							 * information dynamically from History, Tags, etc.
							 * Ignore it. */							
							else if (!string.IsNullOrEmpty (title)) 
								yield return new FolderItem(title, id, parent);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Opens a connection to the current temporary database, and searches for all
		/// place entries then adds those to the IEnumerable collection to be returned. 
		/// </summary>
		/// <returns>
		/// A collection of bookmarks and history Place Items.
		/// </returns>
		IEnumerable<PlaceItem> LoadPlaceItems ()
		{	
			using (IDbConnection dbcon = (IDbConnection) new SqliteConnection (ConnectionString)) {
				dbcon.Open ();
				
				using (IDbCommand dbcmd = dbcon.CreateCommand ()) {
					dbcmd.CommandText =  "SELECT moz_places.title, moz_places.url, moz_bookmarks.parent, moz_bookmarks.title "
						+ "FROM moz_places LEFT OUTER JOIN moz_bookmarks "
						+ "ON moz_places.id=moz_bookmarks.fk "
						+ "ORDER BY moz_places.frecency DESC";
						
					using (IDataReader reader = dbcmd.ExecuteReader ()) {
						while (reader.Read () ) {				
							string title = reader.GetString (0);
							string url = reader.GetString (1);
							
							// Firefox stores some interesting non-url places. Ignore them.
							if (url [0] != 'p') {
								// If the place is a bookmark, use the title stored in Bookmarks.
								if (!reader.IsDBNull (2)) {
									int parent = reader.GetInt32 (2);
									string bookmarkTitle = reader.GetString (3);
									
									yield return new PlaceItem (bookmarkTitle, url, parent);
								} else if (string.IsNullOrEmpty (title)) {
									// If the place has no title, use the url as a title so it's searchable.
									yield return new PlaceItem (url, url);
								} else {
									yield return new PlaceItem (title, url);
								}
							}
						}
					}
				}
			}
		}
	}
}