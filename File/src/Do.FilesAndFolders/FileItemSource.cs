/* FileItemSource.cs
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

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Do.FilesAndFolders {

	/// <summary>
	/// Indexes files recursively starting in a specific directory.
	/// </summary>
	public class FileItemSource : ItemSource, IConfigurable {

		public IEnumerable<IItem> Items { get; protected set; }

		public FileItemSource ()
		{
			Items = Enumerable.Empty<IItem> ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (IFileItem);
				yield return typeof (ITextItem);
				yield return typeof (IApplicationItem);
			}
		}
		
		public override string Name {
			get { return Catalog.GetString ("File Indexer"); }
		}
		
		public override string Description {
			get {
				return Catalog.GetString ("Frequently used files and folders.");
			}
		}
		
		public override string Icon {
			get { return "folder"; }
		}
		
		public override IEnumerable<IItem> ChildrenOfItem (IItem item)
		{
			IFileItem file = null;

			if (item is ITextItem)
				file = Services.UniverseFactory.NewFileItem ((item as ITextItem).Text);
			else if (item is IFileItem)
				file = item as IFileItem;
			else if (item is IApplicationItem)
				return Enumerable.Empty<IItem> ();
			
			return RecursiveGetFileItems (file.Path, 0).Cast<IItem> ();
		}
		
		public void UpdateItems ()
		{
			Items = Plugin.FolderIndex
				.SelectMany (folder => RecursiveGetFileItems (folder.Path, folder.Level))
				.Take (Plugin.Preferences.MaximumFilesIndexed)
				.ToArray ();
		}

		static IEnumerable<IFileItem> RecursiveGetFileItems (string path, int levels)
		{
			return RecursiveListFiles (path, levels)
				.Select (filepath => Services.UniverseFactory.NewFileItem (filepath));
		}
		
		static IEnumerable<string> RecursiveListFiles (string path, int levels)
		{
			IEnumerable<string> results = null;

			if (path == null) throw new ArgumentNullException ("path");
			
			if (levels < 0 || !Directory.Exists (path))
				return Enumerable.Empty<string> ();
			
			try {
				IEnumerable<string> files, directories, recursiveFiles;
				
				files = Directory.GetFiles (path).Where (ShouldIndexFile);
				directories = Directory.GetDirectories (path).Where (ShouldIndexFile);
				recursiveFiles = directories.SelectMany (dir => RecursiveListFiles (dir, levels - 1));
				results = files.Concat (directories).Concat (recursiveFiles);
			} catch (Exception e) {
				Log.Error ("Encountered an error while attempting to index {0}: {1}", path, e.Message);
				Log.Debug (e.StackTrace);
				results = Enumerable.Empty<string> ();
			}
			return results;
		}

		static bool ShouldIndexFile (string path)
		{
			string filename = Path.GetFileName (path);
			return filename != "." && filename != ".." &&
				(Plugin.Preferences.IncludeHiddenFiles || !filename.StartsWith ("."));
		}
		
	}
}
