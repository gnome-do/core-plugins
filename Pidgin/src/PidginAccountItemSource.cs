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

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace PidginPlugin
{

	public class PidginAccountItemSource : ItemSource
	{

		List<Item> items;

		public PidginAccountItemSource ()
		{
			items = new List<Item> ();
		}
		
		public override string Name {
			get { return Catalog.GetString ("Pidgin Accounts"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Available Pidgin IM Accounts"); }
		}

		public override string Icon {
			get { return "pidgin"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (PidginAccountItem); }
		}
		
		public override IEnumerable<Item> Items {
			get { return items; }
		}
		
		public override void UpdateItems ()
		{
			Pidgin.IPurpleObject prpl;
			prpl = Pidgin.GetPurpleObject ();
			string name, proto;
			if (Pidgin.InstanceIsRunning) {
				items.Clear ();
				try {
					foreach (int account in prpl.PurpleAccountsGetAll ()) {
						proto = prpl.PurpleAccountGetProtocolName (account);
						name = prpl.PurpleAccountGetUsername (account);
						items.Add (new PidginAccountItem (name, proto, account));
					}
				} catch (Exception e) { 
					Log<PidginAccountItemSource>.Error ("Could not get Pidgin accounts: {0}", e.Message);
					Log<PidginAccountItemSource>.Debug (e.StackTrace);
				}
			}
		}
	}
}
