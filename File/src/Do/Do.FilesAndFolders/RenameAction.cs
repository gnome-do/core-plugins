// RenameAction.cs
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
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace Do.FilesAndFolders
{
	
	class RenameAction : AbstractFileAction
	{
		
		public override string Name { 
			get { return AddinManager.CurrentLocalizer.GetString ("Rename file..."); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Renames a file."); }
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
			string result = null;
			string destination = Path.Combine (Path.GetDirectoryName (source), newName);

			Log.Info ("Renaming {0} to {1}...", source, destination);
			Services.Application.RunOnThread (() => {
				try {
					result = Move (source, destination);
				} catch (Exception e) {
					Log.Error ("Could not move {0} to {1}: {2}", source, destination, e.Message);
					Log.Debug (e.StackTrace);
				}
			});
			// Wait for the other thread to begin moving the file. We may need to
			// yield.
			PerformWait ();
			if (string.IsNullOrEmpty (result))
				yield break;
			else
				yield return Plugin.NewFileItem (result) as Item;
		}
	}
}
