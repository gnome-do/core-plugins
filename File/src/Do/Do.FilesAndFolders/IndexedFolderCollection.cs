// IndexedFolderCollection.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Do.Platform;

namespace Do.FilesAndFolders
{
	
 	class IndexedFolderCollection : ICollection<IndexedFolder>
	{

		const uint LargeIndexLevel = 5;
		const string LargeIndexLevelWarning =
			"An IndexedFolder has been created that will index folder {0} {1} folders deep. " +
			"This may result in excessive memory consumption and poor performance. " +
			"Please make sure that you only index the files you open frequently.";

		IDictionary<string, IndexedFolder> Folders { get; set; }

		string SavedStateFile {
			get {
				return Path.Combine (Services.Paths.UserDataDirectory, GetType ().FullName);
			}
		}

		IEnumerable<IndexedFolder> GetDefaultFolders ()
		{
			yield return new IndexedFolder (Path.GetDirectoryName (Plugin.ImportantFolders.UserHome), 1, true);
			yield return new IndexedFolder (Plugin.ImportantFolders.UserHome, 1, true);
			yield return new IndexedFolder (Plugin.ImportantFolders.Desktop, 1, true);
			yield return new IndexedFolder (Plugin.ImportantFolders.Documents, 2, true);
		}

		public IndexedFolderCollection ()
		{
		}

		public void Initialize ()
		{
			Deserialize ();

			foreach (IndexedFolder folder in Folders.Values) {
				if (folder.Level > LargeIndexLevel)
					Log<IndexedFolderCollection>.Warn (LargeIndexLevelWarning, folder.Path, folder.Level);
			}
		}

		public void UpdateIndexedFolder (string path, string newPath, uint newDepth, bool newIndex)
		{
			if (newDepth > LargeIndexLevel)
				Log<IndexedFolderCollection>.Warn (LargeIndexLevelWarning, newPath, newDepth);
			UpdateIndexedFolder (path, new IndexedFolder (newPath, newDepth, newIndex));
		}

		public void UpdateIndexedFolder (string path, IndexedFolder folder)
		{
			// If the updated folder is not actually any different, don't udpate.
			if (Folders.ContainsKey (path) && Folders [path] == folder) return;
			RemoveIndexedFolder (path);
			Add (folder);
		}

		public void RemoveIndexedFolder (string path)
		{
			if (!Folders.ContainsKey (path)) return;
			Remove (Folders [path]);
		}
		
		public bool ContainsFolder (string path)
		{
			return Folders.ContainsKey (path);
		}

		#region ICollection<IndexedFolder>

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return Folders.Keys.GetEnumerator ();
		}
		
		public IEnumerator<IndexedFolder> GetEnumerator ()
		{
			return Folders.Values.GetEnumerator ();
		}
		
		public void Add (IndexedFolder pair)
		{
			Folders [pair.Path] = pair;
			Serialize ();
		}

		public bool Remove (IndexedFolder pair)
		{
			if (Folders.Remove (pair.Path)) {
				Serialize ();
				return true;
			}
			return false;
		}

		public void Clear ()
		{
			Folders.Clear ();
			Serialize ();
		}

		public void CopyTo (IndexedFolder [] folders, int count)
		{
			Folders.Values.CopyTo (folders, count);
		}

		public bool Contains (IndexedFolder folder)
		{
			return Folders.ContainsKey (folder.Path);
		}

		public int Count {
			get { return Folders.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}
		
		#endregion

		void Deserialize ()
		{
			try {
				using (Stream stream = File.OpenRead (SavedStateFile))
					Folders = new BinaryFormatter ().Deserialize (stream) as IDictionary<string, IndexedFolder>;
				//check & fix old version of database
				List<IndexedFolder> f = Folders.Values.ToList<IndexedFolder> ();
				foreach (IndexedFolder folder in f) {
					if ((folder.Level > 0) && !(folder.Index)) {
						Log<IndexedFolderCollection>.Debug ("Old DB entry for {0} found, fixing.", folder.Path);
						UpdateIndexedFolder(folder.Path, folder.Path, folder.Level, !folder.Index);
					}
				}
				Log<IndexedFolderCollection>.Debug ("Loaded Files and Folders plugin state.");
			} catch (FileNotFoundException) {
			} catch (Exception e) {
				Log<IndexedFolderCollection>.Error ("Failed to load Files and Folders plugin state: {0}", e.Message);
				Log<IndexedFolderCollection>.Debug (e.StackTrace);
			} finally {
				// Some sort of error occurred, so load the default data set and save it.
				if (Folders == null) {
					// TODO System.Linq.Enumerable.ToDictionary is not implemented
					// in earlier versions of mono.
					// Folders = GetDefaultFolders ().ToDictionary (pair => pair.Path);
					Folders = ToDictionary (GetDefaultFolders (), pair => pair.Path);
					Serialize ();
				}
			}
		}

		// TODO remove this when older versions of mono with unimplemented
		// System.Linq.Enumerable.ToDictionary are deprecated.
		Dictionary<TKey, TValue> ToDictionary<TKey, TValue> (IEnumerable<TValue> xs, Func<TValue, TKey> f)
		{
			Dictionary<TKey, TValue> d = new Dictionary<TKey, TValue> ();
			foreach (TValue x in xs)
				d [f (x)] = x;
			return d;
		}

		void Serialize ()
		{
			try {
				using (Stream stream = File.OpenWrite (SavedStateFile))
					new BinaryFormatter ().Serialize (stream, Folders);
				Log<IndexedFolderCollection>.Debug ("Saved Files and Folders plugin state.");
			} catch (Exception e) {
				Log<IndexedFolderCollection>.Error ("Failed to save lFiles and Folders plugin state: {0}", e.Message);
				Log<IndexedFolderCollection>.Debug (e.StackTrace);
			}
		}
		
	}
}
