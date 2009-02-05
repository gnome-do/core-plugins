/* Configuration.cs
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;

using Gtk;
using Mono.Unix;

namespace Do.FilesAndFolders
{	
	[System.ComponentModel.Category("File")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Configuration : Gtk.Bin
	{
		PathNodeView nview;
		
		public Configuration ()
		{
			Build ();
			
			nview = new PathNodeView ();
			nview.Selection.Changed += OnPathNodeViewSelectionChange;
			node_scroll.Add (nview);
			
			show_hidden_chk.Active = Plugin.Preferences.IncludeHiddenFiles;
			remove_btn.Sensitive = false;
		}

		protected virtual void OnAddBtnClicked (object sender, System.EventArgs e)
		{
			FileChooserDialog chooser;

			chooser = new FileChooserDialog (
			    Catalog.GetString ("Choose a folder to index"),
				new Dialog (), FileChooserAction.SelectFolder,
				Catalog.GetString ("Cancel"), ResponseType.Cancel,
				Catalog.GetString ("Choose folder"), ResponseType.Accept);
					
			
			if (chooser.Run () == (int) ResponseType.Accept) {
				Plugin.FolderIndex.Add (new IndexedFolder (chooser.Filename, 1));
				nview.Refresh ();
			}
			chooser.Destroy ();
		}

		protected virtual void OnRemoveBtnClicked (object sender, EventArgs e)
		{
			nview.OnRemoveSelected (sender, e);
		}
		
		protected void OnPathNodeViewSelectionChange (object sender, EventArgs e)
		{
			remove_btn.Sensitive = nview.Selection.GetSelectedRows ().Any ();
		}

		protected virtual void OnShowHiddenChkClicked (object sender, EventArgs e)
		{
			Plugin.Preferences.IncludeHiddenFiles = show_hidden_chk.Active;
		}
	}
}
