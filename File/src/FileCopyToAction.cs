/* IFileItemActions.cs
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

namespace FilePlugin {
	class CopyToAction : Act {

		public override string Name {
			get { return Catalog.GetString ("Copy to..."); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Copies a file or folder to another location."); }
		}
		
		public override string Icon { get { return "gtk-copy"; } } 

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (IFileItem),
				};
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (IFileItem),
				};
			}
		}

		public bool ModifierItemsOptional {
			get { return false; }
		}

		public bool SupportsItem (Item item)
		{
			return true;
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return IFileItem.IsDirectory (modItem as IFileItem);
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			return null;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			IFileItem dest;
			List<string> seenPaths;

			dest = modItems.First () as IFileItem;
			seenPaths = new List<string> ();
			foreach (IFileItem src in items) {
				if (seenPaths.Contains (src.Path)) continue;
				try {
					//File.Copy doesn't work on directories, WHY!?
					System.Diagnostics.Process.Start ("cp", "-a " +
						IFileItem.EscapedPath (src.Path) + " " +
						IFileItem.EscapedPath (dest.Path + "/" + src.Name));
						
					seenPaths.Add (src.Path);
					src.Path = Path.Combine (dest.Path, Path.GetFileName (src.Path));
				} catch (Exception e) {
					Console.Error.WriteLine ("CopyToAction could not copy "+
							src.Path + " to " + dest.Path + ": " + e.Message);
				}
			}
			return null;
		}
	}
}