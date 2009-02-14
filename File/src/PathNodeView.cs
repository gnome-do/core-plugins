/* PathNodeView.cs
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
using System.IO;

using Mono.Unix;

using Gtk;

namespace Do.FilesAndFolders
{
	
	// TODO: update this class to use spin buttons,
	public class PathNodeView : NodeView
	{
		public enum Column {
			Index = 0,
			Path,
			Depth,
			NumColumns
		}
		
		public PathNodeView () : base ()
		{
			CellRenderer cell;
			RulesHint = true;
			HeadersVisible = true;
			
			Model = new ListStore (typeof (bool), typeof (string), typeof (uint));

			cell = new CellRendererText ();
			(cell as CellRendererText).Width = 310;
			(cell as CellRendererText).Ellipsize = Pango.EllipsizeMode.Middle;
			AppendColumn (Catalog.GetString ("Folder"), cell, "text", Column.Path);
			
			cell = new CellRendererText ();
			(cell as CellRendererText).Editable = true;
			(cell as CellRendererText).Edited += OnDepthEdited;
			(cell as CellRendererText).Alignment = Pango.Alignment.Right;
			AppendColumn (Catalog.GetString ("Depth"), cell, "text", Column.Depth);
						
			Refresh ();
		}

		public void Refresh ()
		{
			ListStore store = Model as ListStore;
			//try to keep the currently selected row across refreshes
			Gtk.TreePath selected = null;
			try {
				selected = this.Selection.GetSelectedRows ()[0];
			}
			catch {	}
			finally {
				store.Clear ();
				foreach (IndexedFolder pair in Plugin.FolderIndex) {
					store.AppendValues (pair.Index, pair.Path, pair.Level);
				}
				if (selected != null)
					this.Selection.SelectPath (selected);
			}
		}
		                    
		void OnDepthEdited (object o, EditedArgs e)
		{
			uint depth;
			string path;
			bool index;
			TreeIter iter;
			ListStore store;

			store = Model as ListStore;
			store.GetIter (out iter, new TreePath (e.Path));
			
			path = store.GetValue (iter, (int) Column.Path) as string;
			depth = uint.Parse (e.NewText);
			index = (bool) store.GetValue (iter, (int) Column.Index);
			Plugin.FolderIndex.UpdateIndexedFolder (path, path, depth, index);
			
			Refresh ();
		}

		public void OnRemoveSelected (object sender, EventArgs e)
		{
			string path;
			TreeIter iter;
			ListStore store;

			store = Model as ListStore;
			Selection.GetSelected (out iter);
			path = store.GetValue (iter, (int) Column.Path) as string;
			Plugin.FolderIndex.RemoveIndexedFolder (path);
			store.Remove (ref iter);
			
			Refresh ();
		}
		
	}
}