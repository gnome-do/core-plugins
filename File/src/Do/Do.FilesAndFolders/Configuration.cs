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
		PathNodeView[] nodeViews = new PathNodeView[2];
		
		string indexDialog = Catalog.GetString ("Choose a folder to index");
		string ignoreDialog = Catalog.GetString ("Choose a folder to ignore");
			
		public Configuration ()
		{			
			Build ();
			
			nodeViews[0] = new IndexPathNodeView ();
			nodeViews[1] = new IgnorePathNodeView ();

			foreach (PathNodeView nodeView in nodeViews)
				nodeView.Selection.Changed += OnPathNodeViewSelectionChange;
			
			index_node_scroll.Add (nodeViews[0]);
			ignore_node_scroll.Add (nodeViews[1]);
			
			show_hidden_chk.Active = Plugin.Preferences.IncludeHiddenFiles;

			index_remove_btn.Sensitive = false;
			ignore_remove_btn.Sensitive = false;
			
			notebook1.Page = 0;
			
		}
		
		private PathNodeView GetCurrentView ()
		{
			return nodeViews[this.notebook1.CurrentPage];
		}
		
		private void RefreshCurrentView ()
		{
			PathNodeView curr = GetCurrentView ();
			if (curr is IndexPathNodeView)
				curr.Refresh (true);
			else
				curr.Refresh (false);
		}
			
		
		protected virtual void OnAddBtnClicked (object sender, System.EventArgs e)
		{
			FileChooserDialog chooser;
			string dialogTitle;
			FolderStatus status;
			uint depth;
			
			if (GetCurrentView() is IndexPathNodeView) {
				dialogTitle = indexDialog;
				status = FolderStatus.Indexed;
				depth = 1;
			}
			else {
				dialogTitle = ignoreDialog;
				status = FolderStatus.Ignored;
				depth = 0;
			}
			
			chooser = new FileChooserDialog (
			    dialogTitle,
				new Dialog (), FileChooserAction.SelectFolder,
			    Gtk.Stock.Cancel, ResponseType.Cancel,
			    Gtk.Stock.Add, ResponseType.Accept);
				
			if (chooser.Run () == (int) ResponseType.Accept) {
				if (!Plugin.FolderIndex.ContainsFolder (chooser.Filename))
				    Plugin.FolderIndex.Add (new IndexedFolder (chooser.Filename, depth, status));
				RefreshCurrentView ();
			}
			chooser.Destroy ();
		}

		protected virtual void OnRemoveBtnClicked (object sender, EventArgs e)
		{
			GetCurrentView ().OnRemoveSelected (sender, e);
			RefreshCurrentView ();
		}
		
		protected void OnPathNodeViewSelectionChange (object sender, EventArgs e)
		{
			index_remove_btn.Sensitive = nodeViews[0].Selection.GetSelectedRows ().Any ();
			ignore_remove_btn.Sensitive = nodeViews[1].Selection.GetSelectedRows ().Any ();
		}

		protected virtual void OnShowHiddenChkClicked (object sender, EventArgs e)
		{
			Plugin.Preferences.IncludeHiddenFiles = show_hidden_chk.Active;
		}
		
	}
}
