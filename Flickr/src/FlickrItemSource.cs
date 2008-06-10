/* FlickrItemSource.cs
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

using Do.Addins;
using Do.Universe;

namespace Flickr
{	
	public class FlickrItemSource : IItemSource, IConfigurable
	{	
		public FlickrItemSource()
		{
		}
		
		public string Name {
			get { return "Account"; }
		}
		
		public string Description {
			get { return ""; }
		}
		
		public string Icon {
			get { return "flickr.png@" + GetType ().Assembly.FullName; }
		}
		
		public Type [] SupportedItemTypes {
			get {
				return new Type [] { };
			}
		}
		
		public ICollection<IItem> Items
		{
			get { return null; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;
		}
		
		public void UpdateItems ()
		{
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new AccountConfig ();
		}
	}
}