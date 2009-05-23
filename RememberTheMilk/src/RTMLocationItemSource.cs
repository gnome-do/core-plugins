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
using System.Collections.Generic;
using System.Threading;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	
	
	public class RTMLocationItemSource : ItemSource
	{
		public override string Name {
			get { return Catalog.GetString ("Remember The Milk Locations"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Locations associated with your Remember The Milk tasks."); }
		}
		
		public override string Icon {
			get { return "rtm.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (RTMLocationItem); }
		}
		
		public override IEnumerable<Item> Items
		{
			get { return RTM.Locations; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			return RTM.TasksForLocation ((parent as RTMLocationItem).Id);
		}

		public override void UpdateItems ()
		{
			Thread updateLocations = new Thread (new ThreadStart (RTM.UpdateLocations));
			updateLocations.IsBackground = true;
			updateLocations.Start ();
		}
	}
}
