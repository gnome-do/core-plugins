// RTMTagItemSource.cs
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

using Mono.Addins;

using Do.Platform.Linux;
using Do.Universe;

namespace RememberTheMilk
{
	/// <summary>
	/// ItemSource class for the tags used by tasks
	/// </summary>
	public class RTMTagItemSource : ItemSource
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Remember The Milk Tags"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Tags used by your Remember The Milk tasks."); }
		}
		
		public override string Icon {
			get { return "rtm.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (RTMTagItem); }
		}
		
		public override IEnumerable<Item> Items
		{
			get { return RTM.Tags; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			return RTM.TasksForTag ((parent as RTMTagItem).Name);
		}
		
		public override void UpdateItems ()
		{
		}
	}
}
