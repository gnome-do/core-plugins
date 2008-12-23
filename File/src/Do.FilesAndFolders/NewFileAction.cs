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
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Do.FilesAndFolders
{
	
	public class NewFileAction : Act
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
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (IFileItem);
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (IFileItem);
			}
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}

		/// <summary>
		/// Supports using an IFileItem as the parent if it is a folder
		/// that already exists.
		/// </summary>
		/// <param name="item">
		/// A <see cref="IFileItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		bool Supports (IFileItem item)
		{
			return Directory.Exists (item.Path);
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
		bool Supports (ITextItem item)
		{
			string path = item.Text.Replace ("~", Plugin.ImportantFolders.UserHome);
			return !File.Exists (path) && !Directory.Exists (path);
		}
		
		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) return Supports (item as ITextItem);
			if (item is IFileItem) return Supports (item as IFileItem);
			return false;
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
			Func<Item, bool> isDirectory = item =>
				item is IFileItem && Supports (item as IFileItem);

			return !isDirectory (items.First ()) || !isDirectory (modItem);
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
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IFileItem parent = null;
			ITextItem fileName = null;
			Item first = items.First (), mod = modItems.FirstOrDefault ();
			
			if (first is IFileItem) {
				parent = first as IFileItem;
				// We provide a default folder name if none is provided.
				fileName = mod as ITextItem ??
					Plugin.NewTextItem (GetNewFileName (parent.Path));
			} else if (first is ITextItem) {
				fileName = first as ITextItem;
				// We provide a default parent folder if none is provided.
				parent = mod as IFileItem ??
					Plugin.NewFileItem (Plugin.ImportantFolders.Desktop);
			}
			
			string path = Path.Combine (parent.Path, fileName.Text);
			CreateFile (path);
			yield return Plugin.NewFileItem (path) as Item;
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

