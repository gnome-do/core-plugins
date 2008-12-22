/* RecentIFileItemSource.cs
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
using System.Collections.Generic;
using Mono.Unix;
using Do.Universe;

namespace FilePlugin
{
	public class RecentIFileItemSource : ItemSource
	{
		List<Item> files;
		
		public RecentIFileItemSource()
		{
			files = new List<Item> ();
			Gtk.RecentManager.Default.Changed += OnRecentChanged;

			//UpdateItems ();
		}
		
		protected void OnRecentChanged (object sender, EventArgs args)
		{
			UpdateItems ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { return new Type[] {
					typeof (IFileItem),
				};
			}
		}
		
		public override string Name
		{
			get { return Catalog.GetString ("Recent Files"); }
		}
		
		public override string Description
		{
			get { return Catalog.GetString ("Finds recently-opened files."); }
		}
		
		public override string Icon
		{
			get { return "document"; }
		}
		
		public override IEnumerable<Item> Items {
			get { return files; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item) {
			return null;
		}
		
		public override void UpdateItems ()
		{
			/*
			files.Clear ();
			foreach (Gtk.RecentInfo info in Gtk.RecentManager.Default.Items) {
				Console.WriteLine ("Recent items source adding item: {0}", info);
				files.Add (new IFileItem (info.Uri));
			}
			*/
		}
	}

}
