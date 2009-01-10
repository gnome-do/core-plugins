// NewFileAction.cs
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

namespace Do.FilesAndFolders
{
	
	public class NewFileAction : AbstractFileAction
	{

		public override string Name {
			get { return Catalog.GetString ("Create New File"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Creates an new, empty file."); }
		}
		
		public override string Icon {
			get { return "filenew"; }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		/// <summary>
		/// Supports using an ITextItem as the folder name if
		/// it is not the name of a folder that already exists.
		/// </summary>
		/// <param name="item">
		/// A <see cref="ITextItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		protected override bool SupportsItem (ITextItem item)
		{
			string path = GetPath (item);
			return path.Length < MaxPathLength && !File.Exists (path) && !Directory.Exists (path);
		}

		protected override bool SupportsItem (IFileItem item)
		{
			return Directory.Exists (GetPath (item));
		}

		/// <summary>
		/// Prevents 1st and 3rd pane from both containing IFileItems.
		/// </summary>
		/// <param name="items">
		/// A <see cref="IEnumerable"/>
		/// </param>
		/// <param name="modItem">
		/// A <see cref="Item"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			Item first = items.First ();
			return
				(first is IFileItem && !(modItem is IFileItem)) ||
				(first is ITextItem && modItem is IFileItem && SupportsItem (modItem as IFileItem));
		}

		/// <summary>
		/// Creates and returns the new folder as an IFileItem.
		/// </summary>
		/// <param name="items">
		/// A <see cref="IEnumerable"/>
		/// </param>
		/// <param name="modItems">
		/// A <see cref="IEnumerable"/>
		/// </param>
		/// <returns>
		/// A <see cref="IEnumerable"/>
		/// </returns>
		protected override IEnumerable<Item> Perform (Item source, Item destination)
		{
			IFileItem parent = null;
			ITextItem fileName = null;
			
			if (source is IFileItem) {
				parent = source as IFileItem;
				// We provide a default folder name if none is provided.
				fileName = destination as ITextItem ??
					Plugin.NewTextItem (GetNewFileName (parent.Path));
			} else if (source is ITextItem) {
				fileName = source as ITextItem;
				// We provide a default parent folder if none is provided.
				parent = destination as IFileItem ??
					Plugin.NewFileItem (Plugin.ImportantFolders.Desktop);
			}
			
			string path = Path.Combine (parent.Path, fileName.Text);
			CreateFile (path);
			yield return Plugin.NewFileItem (path) as Item;
		}

		protected override IEnumerable<Item> Perform (Item source)
		{
			return Perform (source, null);
		}

		/// <summary>
		/// Given a directory, returns a name of a file that does not exist
		/// in that directory.
		/// </summary>
		/// <param name="parent">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public static string GetNewFileName (string parent)
		{
			if (File.Exists (parent))
				throw new ArgumentException ("Parent must be a directory", "parent");
			if (!Directory.Exists (parent))
				throw new FileNotFoundException ("Parent directory must exist", "parent");
			
			return GetNewFileName (parent, Catalog.GetString ("Untitled"), 0);
		}

		static string GetNewFileName (string parent, string name, uint suffix)
		{
			string newName = name + (suffix == 0 ? "" : suffix.ToString ());
			string path = Path.Combine (parent, newName);
			
			if (File.Exists (path) || Directory.Exists (path))
				return GetNewFileName (parent, name, suffix + 1);
			return newName;
		}

		protected virtual void CreateFile (string path)
		{
			File.Create (path);
		}
	}
}

