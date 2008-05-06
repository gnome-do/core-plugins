// CreateDirectory.cs
// 
// Copyright (C) 2008 [name of author]
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Do.Universe;

namespace Do.Files {
	public class CreateDirectory : AbstractAction {

		public override string Name {
			get {
				return "Create directory";
			}
		}
		
		public override string Description {
			get {
				return "Creates an empty directory";
			}
		}
		
		public override string Icon {
			get {
				return "folder-new";
			}
		}
		
		public override Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof(FileItem),
				};
			}
		}

		public override Type[] SupportedModifierItemTypes {
			get {
				return new Type[] { typeof(ITextItem) };
			}
		}
		
		public override bool SupportsItem (IItem item)
		{
			// Check for archive types
			FileItem fi = item as FileItem;
			return fi.MimeType == "x-directory/normal";
		}

		public override bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return true;
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			FileItem parent = items [0] as FileItem;

			// Don't create the file if the parent is not a dir
			if (parent.MimeType != "x-directory/normal") {
				return null;
			}

			ITextItem ti = modItems [0] as ITextItem;

			// Create the filename for the new file
			string dir_name = parent.Path + "/" + ti.Text;
			System.Diagnostics.Process.Start ("mkdir", dir_name);
			

			// Return the new file, so new actions can be used on it
			return new IItem[]{ new FileItem(dir_name) };
		}
	}
}

