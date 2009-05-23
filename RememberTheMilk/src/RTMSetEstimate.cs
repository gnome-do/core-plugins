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
	public class RTMSetEstimate : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Set Estimated Time"); }
		}		
				
		public override string Description {
			get { return Catalog.GetString ("Set or reset the estimated time for a task"); }
		}
			
		public override string Icon {
			get { return "task-setdue.png@" + GetType ().Assembly.FullName; }
		}
				
		public bool CheckValidTime(string timeEntered) {
			// These are the valid time values specified by the RTM API.
			// See if the user has entered one of them.
			string[] times = {"minute", "minutes", "hour",
						"hours", "day", "days"};
			bool hasValidTime = false;
			
			foreach(string timeValue in times)
			{
				if(timeEntered.IndexOf(timeValue) != -1) {
					hasValidTime = true;
					break;
				}
			}
			
			return hasValidTime;
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (RTMTaskItem);
				yield return typeof (RTMTaskAttributeItem);
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				yield return typeof (ITextItem);
			}
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}
        
		public override bool SupportsItem (Item item) 
		{
			if (item is RTMTaskItem)
				return true;
			else if (item is RTMTaskAttributeItem)
				return (item as RTMTaskAttributeItem).Description == "Time Estimate";
			else
				return false;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			RTMTaskItem task = null;
			string est = String.Empty;
			
			if (items.Any ()) {
				if (items.First () is RTMTaskItem)
					task = (items.First () as RTMTaskItem);
				else if (items.First () is RTMTaskAttributeItem)
					task = (items.First () as RTMTaskAttributeItem).Parent;
			}
			
			if (modifierItems.Any ())
				est = ((modifierItems.First () as ITextItem).Text);
			
			if (!string.IsNullOrEmpty(est)) {
				if (!CheckValidTime(est)) {
					Services.Notifications.Notify("Remember The Milk",
						"Invalid estimated time provided.");
					yield break;
				}
			}
			
			if (task != null)
				Services.Application.RunOnThread (() => {
					RTM.SetEstimateTime (task.ListId, task.TaskSeriesId, task.Id, est);
				});
			
			yield break;
		}
	}
}
