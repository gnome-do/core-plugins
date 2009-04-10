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

		
		List<Item> items;
		IEnumerable<PlaceItem> places;
		IEnumerable<FolderItem> folders;
		public FABarItemSource ()
		{
			places = Enumerable.Empty<PlaceItem> ();
			folders = Enumerable.Empty<FolderItem> ();
			items = new List<Item> ();
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (PlaceItem); 
				yield return typeof (FolderItem);
			}
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
			get { return items; }
		}

		public override void UpdateItems ()
		{
			this.places = LoadPlaceItems ();
			this.folders = LoadFolderItems ();
			foreach (PlaceItem place in places) items.Add((Item) place);
			foreach (FolderItem folder in folders) items.Add((Item) folder);
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item) {
			if (isFolder(item)) {
				FolderItem parent = (FolderItem) item;
				foreach (PlaceItem place in places) {
					if (parent.Id == place.ParentId) yield return (Item) place;
				}
				foreach (FolderItem folder in folders) {
					if (parent.Id == folder.ParentId) yield return (Item) folder;
				}
			}
		}
		
		bool isFolder(Item item) {
			return item.Icon.Contains("folder"); 
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
		
		/// <summary>
		/// Creates the current SQL connection string to the temporary database in use. 
		/// </summary>
		/// <returns>
		/// The current SQL connection string to the temporary database in use.
		/// </returns>
		string connectionString {
			get { return String.Format ("URI=file:{0},version=3",tempDatabasePath); }
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
			
			using (IDbConnection dbcon = (IDbConnection) new SqliteConnection(connectionString)) {
				dbcon.Open();
				using (IDbCommand dbcmd = dbcon.CreateCommand()) {
					dbcmd.CommandText = @"SELECT title, 
												 id, 
												 parent 
										  FROM moz_bookmarks
										  WHERE type = 2";
					using (IDataReader reader = dbcmd.ExecuteReader()) {
						while(reader.Read()) {
							if (!reader.IsDBNull(2)) {
								string title = reader.GetString(0);
								int id = reader.GetInt32(1);
								if (id == 1)
									yield return new FolderItem("Mozilla Bookmarks", 
									                            reader.GetInt32(1), 
									                            reader.GetInt32(2));
								else if ((title != "") || (title != "Mozilla Firefox")) {
									yield return new FolderItem(reader.GetString(0), 
									                            reader.GetInt32(1), 
									                            reader.GetInt32(2));
								}
							}
							else
								yield return new FolderItem(reader.GetString(0),
								                            reader.GetInt32(1));
						}
					}
				}
				dbcon.Close();
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
			using (IDbConnection dbcon = (IDbConnection) new SqliteConnection(connectionString)) {
				dbcon.Open();
				using (IDbCommand dbcmd = dbcon.CreateCommand()) {
					dbcmd.CommandText = @"SELECT moz_places.title,
												 moz_places.url, 
												 moz_bookmarks.parent,
												 moz_bookmarks.title
										  FROM moz_places LEFT OUTER JOIN moz_bookmarks 
										  ON moz_places.id=moz_bookmarks.fk 
										  ORDER BY moz_places.frecency DESC";
					using (IDataReader reader = dbcmd.ExecuteReader()) {
						while(reader.Read()) {							
							string url = reader.GetString(1);
							if (url[0] != 'p') {
								string title = reader.GetString(0);
								if (title != "") {
									if (!reader.IsDBNull(2)) {
										yield return new PlaceItem (reader.GetString(3),
										                            reader.GetString(1),
										                            reader.GetInt32(2));
									}
									else 
										yield return new PlaceItem (reader.GetString(0),
										                            reader.GetString(1));
								}
							}
						}
					}
				}
				dbcon.Close();
			}
		}
	}
}  


