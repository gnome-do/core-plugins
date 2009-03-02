// FileItemSource.cs
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
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;
using Do.Platform.Linux;

namespace Do.FilesAndFolders {

	/// <summary>
	/// Indexes files recursively starting in a specific directory.
	/// </summary>
	public class FileItemSource : ItemSource, IConfigurable {

		IEnumerable<Item> items;
		bool maximum_files_warned;

		string MaximumFilesIndexedWarning {
			get {
				return string.Format (
					"{0} has indexed over {1} files, which is the maximum number of files it will index. "+
				    "This usually means that you are indexing more files that are useful to you. " +
				    "Do you really require instant access to all of these files? " +
				    "Consider indexing fewer files; a good rule of thumb is to only index the files you open at least once a month, " +
				    "and browse for files that you open less often."

				    , Name, Plugin.Preferences.MaximumFilesIndexed
				);
			}
		}

		public FileItemSource ()
		{
			items = Enumerable.Empty<Item> ();
		}

		public override IEnumerable<Item> Items {
			get { return items; }
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
			get { return Catalog.GetString ("Files and Folders"); }
		}
		
		public override string Description {
			get {
				return Catalog.GetString ("Catalogs important files and folders for quick access.");
			}
		}
		
		public override string Icon {
			get { return "folder"; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			IFileItem file = null;

			if (item is ITextItem)
				file = Plugin.NewFileItem ((item as ITextItem).Text);
			else if (item is IFileItem)
				file = item as IFileItem;
			else if (item is IApplicationItem)
				return Enumerable.Empty<Item> ();
			
			return RecursiveGetItems (file.Path, 1, Plugin.Preferences.IncludeHiddenFilesWhenBrowsing);
		}
		
		public override void UpdateItems ()
		{
			try {
				IEnumerable<IndexedFolder> ignored = Enumerable.Empty<IndexedFolder> ();
				
				ignored = Plugin.FolderIndex
					.Where (folder => !folder.Index);
				
				items = Plugin.FolderIndex
					.SelectMany (folder => RecursiveGetItems (folder.Path, folder.Level, Plugin.Preferences.IncludeHiddenFiles, ignored))
					.Take (Plugin.Preferences.MaximumFilesIndexed)
					.ToArray ();
								
				if (!maximum_files_warned && items.Count () == Plugin.Preferences.MaximumFilesIndexed) {
					Log.Warn (MaximumFilesIndexedWarning);
					Services.Notifications.Notify ("Do is indexing too many files.", MaximumFilesIndexedWarning);
					maximum_files_warned = true;
				}
			}
			catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		/// <summary>
		/// Recursively scan files in path to the given level, creating IFileItems
		/// and IApplicationItems from the files found.
		/// </summary>
		/// <remarks>
		/// This should remain lazy.
		/// </remarks>
		/// <param name="path">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="levels">
		/// A <see cref="System.UInt32"/>
		/// </param>
		/// <param name="includeHidden">
		/// A <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// A <see cref="IEnumerable"/>
		/// </returns>
		
		static IEnumerable<Item> RecursiveGetItems (string path, uint levels, bool includeHidden)
		{
			return RecursiveGetItems(path, levels, includeHidden, Enumerable.Empty<IndexedFolder> ());
		}
		
		static IEnumerable<Item> RecursiveGetItems (string path, uint levels, bool includeHidden, IEnumerable<IndexedFolder> ignored)
		{
			IEnumerable<string> files;
			IEnumerable<Item> fileItems, applicationItems;
			
			files = RecursiveListFiles (path, levels, includeHidden, ignored);

			fileItems = files
				.Select (filepath => Plugin.NewFileItem (filepath))
				.OfType<Item> ();

			applicationItems = files
				.Where (filepath => filepath.EndsWith (".desktop"))
				.Select (filepath => Plugin.NewApplicationItem (filepath))
				.OfType<Item> ();

			return applicationItems.Concat (fileItems);
		}
		
		static IEnumerable<string> RecursiveListFiles (string path, uint levels, bool includeHidden, IEnumerable<IndexedFolder> ignored)
		{
			IEnumerable<string> results = null;

			if (path == null) throw new ArgumentNullException ("path");
			
			if (levels == 0 || !Directory.Exists (path))
				return Enumerable.Empty<string> ();
			
			try {
				IEnumerable<string> files, directories, recursiveFiles;
				
				files = Directory.GetFiles (path)
					.Where (filepath => ShouldIndexPath (filepath, includeHidden, ignored));
				directories = Directory.GetDirectories (path)
					.Where (filepath => ShouldIndexPath (filepath, includeHidden, ignored));
				recursiveFiles = directories
					.SelectMany (dir => RecursiveListFiles (dir, levels - 1, includeHidden, ignored));
				results = files.Concat (directories).Concat (recursiveFiles);
				
			} catch (Exception e) {
				Log.Error ("Encountered an error while attempting to index {0}: {1}", path, e.Message);
				Log.Debug (e.StackTrace);
				results = Enumerable.Empty<string> ();
			}
			return results;
		}

		static bool ShouldIndexPath (string path, bool includeHidden, IEnumerable<IndexedFolder> ignored)
		{
			string filename = Path.GetFileName (path);
			bool isForbidden = filename == "." || filename == ".." || filename.EndsWith ("~");
			bool isHidden = filename.StartsWith (".");
			if (ignored.Where (folder => path == folder.Path).Any ()) 
				isForbidden = true;
			return !isForbidden && (includeHidden || !isHidden);
		}
		
	}
}
