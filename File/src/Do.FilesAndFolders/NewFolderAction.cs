// NewFolderAction.cs
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
	
	public class NewFolderAction : AbstractAction
	{

		public override string Name {
			get { return Catalog.GetString ("New Folder"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Creates an new, empty folder."); }
		}
		
		public override string Icon {
			get { return "folder-new"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (IFileItem); }
		}
		
		public override bool SupportsItem (IItem item)
		{
			string path = (item as ITextItem).Text.Replace ("~", Paths.UserHome);
			return !File.Exists (path) && !Directory.Exists (path);
		}

		public override bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem)
		{
			return Directory.Exists ((modItem as IFileItem).Path);
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems)
		{
			ITextItem text = items.First () as ITextItem;
			IFileItem parent = modItems.Any ()
				? modItems.First () as IFileItem
				: UniverseFactory.NewFileItem (Plugin.ImportantFolders.Desktop);
			string dir = Path.Combine (parent.Path, text.Text);

			Directory.CreateDirectory (dir);
			yield return UniverseFactory.NewFileItem (dir);
		}
	}
}

