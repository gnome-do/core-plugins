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
using Mono.Unix;

using Gtk;

namespace Do.FilesAndFolders
{
	
	// TODO: update this class to use spin buttons,
	public abstract class PathNodeView : NodeView
	{

		
		public PathNodeView () : base ()
		{
		}

		public void Refresh (bool indexed)
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
					if (indexed && pair.Status == FolderStatus.Indexed)
						store.AppendValues (pair.Path, pair.Level);
					else if (!indexed && pair.Status == FolderStatus.Ignored)
						store.AppendValues (pair.Path);
				}
				if (selected != null)
					this.Selection.SelectPath (selected);
			}
		}

		public virtual void OnRemoveSelected (object sender, EventArgs e)
		{
			string path;
			TreeIter iter;
			ListStore store;

			store = Model as ListStore;
			Selection.GetSelected (out iter);
			path = store.GetValue (iter, 0) as string;
			Plugin.FolderIndex.RemoveIndexedFolder (path);
			store.Remove (ref iter);
			
		}
		
	}
}