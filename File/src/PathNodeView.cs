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
		enum Column {
			Path = 0,
			Depth,
			NumColumns
		}
		
		public PathNodeView () : base ()
		{
			ListStore store;
			CellRenderer cell;
			RulesHint = true;
			HeadersVisible = true;
			
			Model = store = new ListStore (typeof (string), typeof (string));

			cell = new CellRendererText ();
			(cell as CellRendererText).Width = 310;
			(cell as CellRendererText).Ellipsize = Pango.EllipsizeMode.Middle;
			AppendColumn (Catalog.GetString ("Folder"), cell, "text", Column.Path);
			
			cell = new CellRendererText ();
			(cell as CellRendererText).Editable = true;
			(cell as CellRendererText).Edited += OnDepthEdited;
			(cell as CellRendererText).Alignment = Pango.Alignment.Right;
			AppendColumn (Catalog.GetString ("Depth"), cell, "text", Column.Depth);
						
			foreach (IndexedFolder pair in Plugin.FolderIndex)
				store.AppendValues (pair.Path, pair.Level);
		}
		                    
		void OnDepthEdited (object o, EditedArgs e)
		{
			int depth;
			string path;
			TreeIter iter;
			ListStore store;
			
			store = Model as ListStore;
			store.GetIter (out iter, new Gtk.TreePath (e.Path));
			//store.GetValue (iter, (int)Column.Path, ref path);
			//store.GetValue (iter, (int)Column.Depth, ref depth);
			//Plugin.FolderIndex.UpdateIndexedFolderDepth (path, depth);
		}

		public void OnRemoveSelected (object sender, EventArgs e)
		{
			int depth;
			string path;
			TreeIter iter;
			ListStore store;

			store = Model as ListStore;
			Selection.GetSelected (out iter);
			//store.GetValue (iter, (int)Column.Path, ref path);
			//store.GetValue (iter, (int)Column.Depth, ref depth);
			//Plugin.FolderIndex.Remove (new IndexedFolder (path, depth));
			//store.Remove (ref iter);
		}
		
	}
}