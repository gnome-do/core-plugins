// PidginAccountItemSource.cs
// 
// Copyright (C) 2008 Alex Launi <alex.launi@gmail.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;

using Do.Universe;
using Mono.Unix;

namespace Do.Addins.Pidgin
{
	public class PidginAccountItemSource : IItemSource
	{
		List<IItem> items;
		public PidginAccountItemSource ()
		{
			items = new List<IItem> ();
			//UpdateItems ();
		}
		
		public string Name { get { return Catalog.GetString ("Pidgin Accounts"); } }
		public string Description { get { return Catalog.GetString ("Available Pidgin IM Accounts"); } }
		public string Icon { get { return "pidgin"; } }
		
		public Type [] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (PidginAccountItem),
				};
			}
		}
		
		public ICollection<IItem> Items {
			get { return items; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		public void UpdateItems ()
		{
			Pidgin.IPurpleObject prpl;
			prpl = Pidgin.GetPurpleObject ();
			string name, proto;
			
			items.Clear ();
			try {
				foreach (int account in prpl.PurpleAccountsGetAll ()) {
					proto = prpl.PurpleAccountGetProtocolName (account);
					name = prpl.PurpleAccountGetUsername (account);
					items.Add (new PidginAccountItem (name, proto, account));
				}
			} catch { 
			}
		}
	}
}
