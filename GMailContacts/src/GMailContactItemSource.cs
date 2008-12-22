// GMailContactItemSource.cs
// 
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform.Linux;

namespace GMail
{	
	public sealed class GMailItemSource : ItemSource, IConfigurable
	{
		public override string Name { 
			get { return Catalog.GetString ("GMail Contacts"); }
		}
		
		public override string Description { 
			get { return Catalog.GetString ("Index your GMail contacts"); }
		}
		
		public override string Icon { 
			get { return "gmail-logo.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ContactItem); }
		}
				
		public override IEnumerable<Item> Items {
			get { return GMail.Contacts; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item) 
		{
			ContactItem contact = item as ContactItem;
			
			foreach (string detail in contact.Details) {
				if (detail.Contains (".gmail"))
					yield return new GMailContactDetailItem (detail, contact [detail]) as Item;
			}
		}
		
		public override void UpdateItems () 
		{
			Thread thread = new Thread ((ThreadStart) (GMail.UpdateContacts));
			thread.IsBackground = true;
			thread.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new GMailConfig ();
		}
	}
}
