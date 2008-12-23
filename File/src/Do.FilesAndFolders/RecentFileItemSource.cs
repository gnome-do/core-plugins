/* RecentFileItemSource.cs
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
using System.Collections.Generic;

using Gtk;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Do.FilesAndFolders
{
	public class RecentFileItemSource : IItemSource
	{
		public IEnumerable<IItem> Items { get; private set; }
		
		public RecentFileItemSource ()
		{
			Items = Enumerable.Empty<IItem> ();
			OnRecentChanged (this, EventArgs.Empty);
			RecentManager.Default.Changed += OnRecentChanged;
		}
		
		void OnRecentChanged (object sender, EventArgs e)
		{
			// We update Files here because this is called on the main thread.
			// We call ToArray to that Files is immutable and safe to enumerate.
			Items = GetRecentFiles ().OfType<IItem> ().ToArray ();
		}

		IEnumerable<IFileItem> GetRecentFiles ()
		{
			// These lines always cause mono to blow up:
			//foreach (RecentInfo info in RecentManager.Default.Items) {
			//	yield return UniverseFactory.NewFileItem (info.Uri);
			//}
			return Enumerable.Empty<IFileItem> ();
		}
		
		public IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}
		
		public string Name {
			get { return Catalog.GetString ("Recent Files"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Finds recently-opened files."); }
		}
		
		public string Icon {
			get { return "document"; }
		}
		
		public IEnumerable<IItem> ChildrenOfItem (IItem item)
		{
			return Enumerable.Empty<IItem> ();
		}
		
		public void UpdateItems ()
		{
		}
	}

}
