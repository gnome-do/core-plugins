/* IndexedFolderCollection.cs
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
			yield return new IndexedFolder (Path.GetDirectoryName (Plugin.ImportantFolders.UserHome), 0);
			yield return new IndexedFolder (Plugin.ImportantFolders.UserHome, 1);
			yield return new IndexedFolder (Plugin.ImportantFolders.Desktop, 1);
			yield return new IndexedFolder (Plugin.ImportantFolders.Documents, 2);
		}

		public IndexedFolderCollection ()
		{
		}

		public void Initialize ()
		{
			Deserialize ();

			foreach (IndexedFolder folder in Folders.Values) {
				if (folder.Level < LargeIndexLevel) continue;
				Log.Warn (LargeIndexLevelWarning, folder.Path, folder.Level);
			}
		}

		public void UpdateIndexedFolder (string path, IndexedFolder folder)
		{
			if (Folders.ContainsKey (path)) Folders.Remove (path);
			Folders [path] = folder;
			Serialize ();
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
			return Folders.ContainsKey (folder.Path) && Folders [folder.Path] == folder;
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
				Log.Debug ("Loaded Files and Folders plugin state.");
			} catch (FileNotFoundException) {
			} catch (Exception e) {
				Log.Error ("Failed to load Files and Folders plugin state: {0}", e.Message);
				Log.Debug (e.StackTrace);
			} finally {
				// Some sort of error occurred, so load the default data set and save it.
				if (Folders == null) {
					Folders = GetDefaultFolders ().ToDictionary (pair => pair.Path);
					Serialize ();
				}
			}
		}

		void Serialize ()
		{
			try {
				using (Stream stream = File.OpenWrite (SavedStateFile))
					new BinaryFormatter ().Serialize (stream, Folders);
				Log.Debug ("Saved Files and Folders plugin state.");
			} catch (Exception e) {
				Log.Error ("Failed to save lFiles and Folders plugin state: {0}", e.Message);
				Log.Debug (e.StackTrace);
			}
		}
		
	}
}
