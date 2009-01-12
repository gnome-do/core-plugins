/* RenameAction.cs
 * 
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using Do.Platform;

namespace Do.FilesAndFolders
{
	
	class RenameAction : MoveAction
	{
		
		public override string Name { 
			get { return Catalog.GetString ("Rename file..."); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Renames a file."); }
		}
		
		public override string Icon {
			get { return "forward"; }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			string dir = Path.GetDirectoryName (GetPath (items.First ()));
			string renamed = Path.Combine (dir, (modItem as ITextItem).Text);
			return !File.Exists (renamed) && !Directory.Exists (renamed);
		}

		protected override IEnumerable<Item> Perform (string source, string newName)
		{
			string renamed = Path.Combine (Path.GetDirectoryName (source), newName);
			
			Log.Info ("Renaming {0} to {1}; new file is {2}.", source, newName, renamed);
			base.Perform (source, renamed);
			
			System.Threading.Thread.Sleep (2 * 1000);
			yield return Plugin.NewFileItem (renamed) as Item;
		}
	}
}
