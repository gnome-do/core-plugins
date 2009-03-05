/* IndexPathNodeView.cs
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
using Mono.Unix;

using Gtk;

namespace Do.FilesAndFolders
{
	
	// TODO: update this class to use spin buttons,
	public class IndexPathNodeView : PathNodeView
	{
		public enum Column {
			Path = 0,
			Depth,
			NumColumns
		}
		
		public IndexPathNodeView () : base ()
		{
			CellRenderer cell;
			RulesHint = true;
			HeadersVisible = true;
			
			Model = new ListStore (typeof (string), typeof (uint));

			cell = new CellRendererText ();
			(cell as CellRendererText).Width = 280;
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
			base.Refresh (true);
		}
		                    
		public void OnDepthEdited (object o, EditedArgs e)
		{
			uint depth;
			string path;
			TreeIter iter;
			ListStore store;

			store = Model as ListStore;
			store.GetIter (out iter, new TreePath (e.Path));
			
			path = store.GetValue (iter, (int) Column.Path) as string;
			depth = uint.Parse (e.NewText);
			Plugin.FolderIndex.UpdateIndexedFolder (path, path, depth, FolderStatus.Indexed);
			
			Refresh ();
		}
	}
}