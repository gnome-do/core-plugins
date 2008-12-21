/* AppendTextAction.cs
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
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Do.Universe;
using Mono.Unix;

namespace Text {	
	public class AppendTextAction : Act {

		public override string Name { get { return Catalog.GetString ("Append to..."); } }
		public override string Description { get { return Catalog.GetString ("Appends text to a selected file."); } }
		public override string Icon { get { return "text-editor"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (ITextItem) };
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type[] { typeof (FileItem), typeof(ITextItem) };
			}
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		

		public bool SupportsItem (Item item)
		{
			return true;
		}
		
		public bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (modItem is FileItem) {
				string mime = (modItem as FileItem).MimeType;
				return mime == "x-directory/normal" || mime.StartsWith ("text/");
			}
			return modItem is ITextItem;
		}
		
		public IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			return null;
		}

		public IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (modItems.First () is FileItem) {
				string text = (items.First () as ITextItem).Text;
				string mime = (modItems.First () as FileItem).MimeType;
				string file = (modItems.First () as FileItem).Path;
				
				if (mime == "x-directory/normal") return null;
				
				using (StreamWriter w = File.AppendText (file)) {
					w.WriteLine (text);
					w.Close ();
				}

				return null;
			}
			else {
				string text = (items.First () as ITextItem).Text;
				string text2 = (modItems.First () as ITextItem).Text;
				return new Item[] { new TextItem (text + text2) };
			}
		}
	}
}

