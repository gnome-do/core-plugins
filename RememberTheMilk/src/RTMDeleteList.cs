// 
// Copyright (C) 2009 GNOME Do
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
using System.Linq;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	public class RTMDeleteList : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Delete List"); }
		}		
		
		public override string Description {
			get { return Catalog.GetString ("Delete the selected task list from Remember The Milk"); }
		}
		
		public override string Icon {
			get { return "task-delete.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (RTMListItem),
				};
			}
		}
		
		public override bool SupportsItem (Item item) 
		{
			return !(item as RTMListItem).Locked;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			string listname = (items.First () as RTMListItem).Id;
			
			Services.Application.RunOnThread (() => {
				RTM.DeleteList (listname);
			});
			yield break;
		}
	}
}
