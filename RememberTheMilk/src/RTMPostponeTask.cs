// RTMPostponeTask.cs
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
using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	/// <summary>
	/// Class for the "Postpone" action
	/// </summary>
	public class RTMPostponeTask : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Postpone"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Postpone a selected task in Remember The Milk"); }
        }
			
		public override string Icon {
			get { return "task-postpone.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (RTMTaskItem); }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			Services.Application.RunOnThread (() => {
				RTM.PostponeTask ((items.First () as RTMTaskItem).ListId, 
					(items.First () as RTMTaskItem).TaskSeriesId,
					(items.First () as RTMTaskItem).Id);
			});
			yield break;
		}
	}
}
