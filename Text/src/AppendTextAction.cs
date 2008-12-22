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

using Mono.Unix;

using Do.Universe;
using Do.Universe.Common;

namespace Text {	
	public class AppendTextAction : Act {

		public override string Name { get { return Catalog.GetString ("Append to..."); } }
		public override string Description { get { return Catalog.GetString ("Appends text to a selected file."); } }
		public override string Icon { get { return "text-editor"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				//yield return typeof (IFileItem);
				yield return typeof (ITextItem);
			}
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			/*
			if (modItem is IFileItem) {
				string mime = (modItem as IFileItem).MimeType;
				return mime == "x-directory/normal" || mime.StartsWith ("text/");
			}
			*/
			return modItem is ITextItem;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			/*
			if (modItems.First () is IFileItem) {
				string text = (items.First () as ITextItem).Text;
				string mime = (modItems.First () as IFileItem).MimeType;
				string file = (modItems.First () as IFileItem).Path;
				
				if (mime == "x-directory/normal") return null;
				
				using (StreamWriter w = File.AppendText (file)) {
					w.WriteLine (text);
					w.Close ();
				}

				yield break;
			} else {
			*/
				string text = (items.First () as ITextItem).Text;
				string text2 = (modItems.First () as ITextItem).Text;
				yield return new TextItem (text + text2);
			//}
		}
	}
}

