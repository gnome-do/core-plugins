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
			numFiles.Changed += OnNumFilesEdited;
			IndexIgnore.Clicked += OnIndexIgnoreClick;
			node_scroll.Add (nview);
			
			show_hidden_chk.Active = Plugin.Preferences.IncludeHiddenFiles;
			numFiles.Text = Plugin.Preferences.MaximumFilesIndexed.ToString ();
			remove_btn.Sensitive = false;
			
			UpdateIndexIgnoreBtnImg (true);
			IndexIgnore.Sensitive = false;
			IndexIgnore.Label = "";
			IndexIgnore.HeightRequest = 35;
			IndexIgnoreLabel.Text = Catalog.GetString ("Folder status:");
			
		}
		
		private void UpdateIndexIgnoreBtnImg (bool index)
		{
			string stock;
			if (index) { 
				stock = "gtk-apply";
				IndexIgnoreLabel.Text = Catalog.GetString ("Folder is indexed.");
			}
			else {
				stock = "gtk-cancel";
				IndexIgnoreLabel.Text = Catalog.GetString ("Folder is ignored.");
			}
			Image btnImage = new Image(stock, IconSize.Button);
			IndexIgnore.Image = btnImage;
		}
		
		protected virtual void OnIndexIgnoreClick (object sender, System.EventArgs e)
		{
			Gtk.TreeIter iter;
			string path;
			uint depth;
			bool newIndexMode;
			try {
				nview.Model.GetIter (out iter, nview.Selection.GetSelectedRows ()[0]);
				newIndexMode = !(bool) nview.Model.GetValue (iter, (int) Do.FilesAndFolders.PathNodeView.Column.Index);
				path = nview.Model.GetValue (iter, (int) Do.FilesAndFolders.PathNodeView.Column.Path) as string;
				depth = (uint) nview.Model.GetValue (iter, (int) Do.FilesAndFolders.PathNodeView.Column.Depth);
				// if we ARE indexing this folder
				if (newIndexMode && depth == 0) {
					depth = 1;
				}
				else
					depth = 0;
				//update folder
				nview.Model.SetValue (iter, (int) Do.FilesAndFolders.PathNodeView.Column.Index, newIndexMode);
				Plugin.FolderIndex.UpdateIndexedFolder (path, path, depth, newIndexMode);
				UpdateIndexIgnoreBtnImg (newIndexMode);
				nview.Refresh ();
			}
			catch (Exception ex) {
				Console.WriteLine ("Error: {0}", ex.ToString());
			}
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
				Plugin.FolderIndex.Add (new IndexedFolder (chooser.Filename, 1, true));
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
			Gtk.TreeIter iter;
			try {
				nview.Model.GetIter (out iter, nview.Selection.GetSelectedRows ()[0]);
				UpdateIndexIgnoreBtnImg ((bool )nview.Model.GetValue (iter, (int)Do.FilesAndFolders.PathNodeView.Column.Index));
			}
			catch { }
			finally {
				remove_btn.Sensitive = nview.Selection.GetSelectedRows ().Any ();
				IndexIgnore.Sensitive = nview.Selection.GetSelectedRows ().Any ();
			}
		}

		protected virtual void OnShowHiddenChkClicked (object sender, EventArgs e)
		{
			Plugin.Preferences.IncludeHiddenFiles = show_hidden_chk.Active;
		}
		
		protected virtual void OnNumFilesEdited (object sender, EventArgs e)
		{
			try {
				Plugin.Preferences.MaximumFilesIndexed = int.Parse (numFiles.Text);
			}
			catch (Exception ex) {
				numFiles.Text = "";
				Plugin.Preferences.MaximumFilesIndexed = 0;
			}
		}
	}
}
