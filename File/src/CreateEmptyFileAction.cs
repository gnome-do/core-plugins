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
using System.Text;
using System.IO;

using Do.Universe;

namespace GnomeDoFile {
	public class CreateEmptyFileAction : AbstractAction {

		public override string Name {
			get {
				return "Create empty file";
			}
		}
		
		public override string Description {
			get {
				return "Creates an empty file";
			}
		}
		
		public override string Icon {
			get {
				return "filenew";
			}
		}
		
		public override Type [] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ITextItem),
				};
			}
		}

		public override Type [] SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (FileItem), 
				};
			}
		}
		
		public override bool SupportsItem (IItem item)
		{
			return !(item as ITextItem).Text.Contains ("/") &&
				!(item as ITextItem).Text.Equals (".") &&
				!(item as ITextItem).Text.Equals ("..");
		}

		public override bool SupportsModifierItemForItems (IItem [] items, IItem modItem)
		{
			// Check for archive types
			FileItem fi = modItem as FileItem;
			return fi.MimeType == "x-directory/normal";
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			FileItem parent;
			if (modItems.Length > 0)
				parent = modItems [0] as FileItem;
			else
				parent = new FileItem (Environment.GetEnvironmentVariable ("HOME") +
				                       "/Desktop");

			ITextItem ti = items [0] as ITextItem;

			// Create the filename for the new file
			string filename = parent.Path + "/" + ti.Text;

			try {
				using (FileStream w = File.Open (filename, FileMode.CreateNew, FileAccess.Write)) {
					// Do nothing just create the file
					w.Close ();
				}
			}
			catch (Exception) {
				return null;
			}

			// Return the new file, so new actions can be used on it
			return new IItem [] { new FileItem (filename) };
		}
	}
}

