/* CreateEmptyFileAction.cs
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mono.Unix;

using Do;
using Do.Universe;

namespace FilePlugin {
	public class CreateEmptyFileAction : Act {

		public override string Name {
			get {
				return Catalog.GetString ("Create empty file");
			}
		}
		
		public override string Description {
			get {
				return Catalog.GetString ("Creates an empty file");
			}
		}
		
		public override string Icon {
			get {
				return "filenew";
			}
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ITextItem),
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
		
		public override bool SupportsItem (Item item)
		{
			return !(item as ITextItem).Text.Contains ("/") &&
				!(item as ITextItem).Text.Equals (".") &&
				!(item as ITextItem).Text.Equals ("..");
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			// Check for archive types
			IFileItem fi = modItem as IFileItem;
			return fi.MimeType == "x-directory/normal";
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string file, dir;
			if (modItems.Any ()) {
				dir = (modItems.First () as IFileItem).Path;
			} else {
				dir = IFileItemSource.Desktop;
			}

			file = (items.First () as ITextItem).Text;
			file = Paths.Combine (dir, file);
			try {
				File.Create (file);
			} catch (Exception) {
				return null;
			}
			return new Item [] { new IFileItem (file) };
		}
	}
}

