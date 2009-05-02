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
using Do.Platform.Common;
using Do.Platform;
	
using Mono.Data.SqliteClient;
using Mono.Unix;
	
namespace Firefox
{
	public class FABarItemSource : ItemSource
	{
		const string BeginProfileName = "Path=";
		const string BeginDefaultProfile = "Default=1";
		// File Information
		string storedProfilePath;		
		string storedTempDatabasePath;
		// Generic History and Bookmark items for Menu
		BrowseHistoryItem historyItem;
		BrowseBookmarkItem bookmarksItem;
		// Collections for Storing Items to be used by Do
		List<Item> items;
		IEnumerable<PlaceItem> places;
		IEnumerable<FolderItem> folders;		
		
		public FABarItemSource ()
		{
			historyItem = new BrowseHistoryItem ();			
			bookmarksItem = new BrowseBookmarkItem ();
			items = new List<Item> ();
			places = Enumerable.Empty<PlaceItem> ();			
			folders = Enumerable.Empty<FolderItem> ();
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

		public override string Name 
		{
			get { return Catalog.GetString ("Firefox Awesome Bar"); }
		}

		public override string Description 
		{
			get { return Catalog.GetString ("Search through your bookmarks and history."); }
		}

		public override string Icon 
		{
			get { return "firefox-3.0"; }
		}

		public override IEnumerable<Item> Items 
		{
			get { return items; }
		}

		public override void UpdateItems ()
		{
			this.places = LoadPlaceItems ();
			this.folders = LoadFolderItems ();
			items.Clear();
			items.Add (bookmarksItem);
			items.Add (historyItem);			
			foreach (FolderItem folder in folders) 
				items.Add((Item) folder);
			foreach (PlaceItem place in places.Where (place => place.ParentId != null))
			    items.Add((Item) place);
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item) 
		{
			if (IsFirefox(item)) 
			{				
				yield return bookmarksItem;
				yield return historyItem;
			}
			
			if (item is BrowseBookmarkItem) 
			{
				foreach (FolderItem folder in folders)
					yield return folder;
				
				foreach (PlaceItem place in places.Where (place => place.ParentId != null))
					yield return place;
			}
			
			if (item is BrowseHistoryItem) 
			{
				foreach (FolderItem folder in folders)
					yield return folder;
				foreach (PlaceItem place in places)
					yield return place;
			}
			
			if (item is FolderItem) 
			{
				FolderItem parent = (FolderItem) item;
				
				foreach (FolderItem folder in folders.Where (folder => folder.ParentId == parent.Id))
					yield return folder;
				
				foreach (PlaceItem place in places.Where (place => place.ParentId == parent.Id))
					yield return place;
			}
		}
		
		bool IsFirefox(Item item) 
		{
			return item.Equals (Do.Platform.Services.UniverseFactory.MaybeApplicationItemFromCommand("firefox"));
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
		
		string ProfilePath 
		{
			get 
			{
				string line, profile, path, home;
				if (!String.IsNullOrEmpty (storedProfilePath)) 
					return storedProfilePath;
				home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				profile = null;
				
				path = Path.Combine (home, ".mozilla/firefox/profiles.ini");
				using (StreamReader r = File.OpenText (path)) {
					while ((line = r.ReadLine ()) != null) {
						if (line.StartsWith (BeginDefaultProfile)) 
							break;
						if (line.StartsWith (BeginProfileName)) 
						{
							line = line.Trim ();
							line = line.Substring (BeginProfileName.Length);
							profile = line;
						}
					}
				}
				return storedProfilePath = new [] {home, ".mozilla/firefox", profile}.Aggregate (Path.Combine); 
			}
		}


		string FirefoxDBPath
		{
			get 
			{
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
		string TempDatabasePath 
		{
			get 
			{				
				// Check if the stored temp file exists.				
				if  ((String.IsNullOrEmpty (storedTempDatabasePath)) 
				     || (!File.Exists (storedTempDatabasePath))) 
				{
					// If it doesn't, make one.
					storedTempDatabasePath = Path.GetTempFileName ();
					System.IO.File.Copy (FirefoxDBPath, storedTempDatabasePath, true);
				}
				else if (File.Exists (storedTempDatabasePath))
				{
					FileInfo firefoxDBFileInfo = new FileInfo (FirefoxDBPath);
					FileInfo tempDBFileInfo = new FileInfo (storedTempDatabasePath);
					if (firefoxDBFileInfo.Length != tempDBFileInfo.Length) 
						System.IO.File.Copy (FirefoxDBPath, storedTempDatabasePath, true);
				}
				return storedTempDatabasePath;
			}
		}
		
		/// <summary>
		/// Creates the current SQL connection string to the temporary database in use. 
		/// </summary>
		/// <returns>
		/// The current SQL connection string to the temporary database in use.
		/// </returns>
		string ConnectionString 
		{
			get { return String.Format ("URI=file:{0},version=3",TempDatabasePath); }
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
					dbcmd.CommandText = 
						"SELECT title, " + 
							   "id, " + 
							   "parent " + 
						"FROM moz_bookmarks " +
						"WHERE type = 2";
					using (IDataReader reader = dbcmd.ExecuteReader ()) 
					{
						while(reader.Read () ) {
							string title = reader.GetString (0);							
							int id = reader.GetInt32 (1);
							int parent = reader.GetInt32 (2);
							/* Firefox's parent system uses the field with ID 1 as a 
							 * parent for all other directories. It doesn't have a name,
							 * so we'll give it one. */							
							if (id == 1)
								yield return new FolderItem("Mozilla Bookmarks", 
								                            id, 
								                            parent);
							/* Firefox uses another field that doesn't have a name.
							 * It references portions of their menu that generate
							 * information dynamically from History, Tags, etc.
							 * Ignore it. */							
							else if (title != "") 
							{
								yield return new FolderItem(title, 
								                            id, 
								                            parent);
							}
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
					dbcmd.CommandText = 
						"SELECT moz_places.title, " +
							   "moz_places.url, " + 
							   "moz_bookmarks.parent, " +
							   "moz_bookmarks.title " +
						"FROM moz_places LEFT OUTER JOIN moz_bookmarks " + 
						"ON moz_places.id=moz_bookmarks.fk " +
						"ORDER BY moz_places.frecency DESC";
					using (IDataReader reader = dbcmd.ExecuteReader ()) {
						while(reader.Read () ) {				
							string title = reader.GetString (0);
							string url = reader.GetString (1);
							// Firefox stores some interesting non-url places. Ignore them.
							if (url[0] != 'p') {
								// If the place is a bookmark, use the title stored in Bookmarks.
								if (!reader.IsDBNull (2)) {
									int parent = reader.GetInt32 (2);
									string bookmarkTitle = reader.GetString (3);
									yield return new PlaceItem (bookmarkTitle,
									                            url,
									                            parent);
								}								
								// If the place has no title, use the url as a title so it's searchable.
								else if (title == "") 
									yield return new PlaceItem (url,
									                            url);
								else
									yield return new PlaceItem (title,
									                            url);
							}
						}
					}
				}
			}
		}
	}
}