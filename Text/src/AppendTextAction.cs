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

using Do.Universe;

namespace Text {	
	public class AppendTextAction : IAction {

		public string Name { get { return "Append to..."; } }
		public string Description { get { return "Appends text to a selected file."; } }
		public string Icon { get { return "text-editor"; } }

		public Type[] SupportedItemTypes {
			get {
				return new Type[] { typeof (ITextItem) };
			}
		}
		
		public Type[] SupportedModifierItemTypes {
			get {
				return new Type[] { typeof (FileItem), typeof(ITextItem) };
			}
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		

		public bool SupportsItem (IItem item)
		{
			return true;
		}
		
		public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			if (modItem is FileItem) {
				string mime = (modItem as FileItem).MimeType;
				return mime == "x-directory/normal" || mime.StartsWith ("text/");
			}
			return modItem is ITextItem;
		}
		
		public IItem[] DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}

		public IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			if (modItems [0] is FileItem) {
				string text = (items [0] as ITextItem).Text;
				string mime = (modItems [0] as FileItem).MimeType;
				string file = (modItems [0] as FileItem).Path;
				
				if (mime == "x-directory/normal") return null;
				
				using (StreamWriter w = File.AppendText (file)) {
					w.WriteLine (text);
					w.Close ();
				}

				return null;
			}
			else {
				string text = (items [0] as ITextItem).Text;
				string text2 = (modItems [0] as ITextItem).Text;
				return new IItem[] { new TextItem (text + text2) };
			}
		}
	}
}

