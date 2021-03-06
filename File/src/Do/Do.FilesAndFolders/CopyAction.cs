// CopyAction.cs
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
	
	class CopyAction : AbstractFileAction
	{

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Copy to..."); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Copies a file or folder to another location."); }
		}
		
		public override string Icon {
			get { return "gtk-copy"; }
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return Directory.Exists (GetPath (modItem));
		}

		protected override IEnumerable<Item> Perform (string source, string destination)
		{
			string result = null;
			Log.Info ("Copying {0} to {1}...", source, destination);
			Services.Application.RunOnThread (() => {
				try {
					result = Copy (source, destination);
				} catch (Exception e) {
					Log.Error ("Could not copy {0} to {1}: {2}", source, destination, e.Message);
					Log.Debug (e.StackTrace);
				}
			});
			
			// Give the move some time to run, then try to yield the result.
			PerformWait ();
			if (string.IsNullOrEmpty (result))
				yield break;
			else
				yield return Plugin.NewFileItem (result) as Item;
		}

	}
}
