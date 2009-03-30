/* RTMSetEstimate.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
				return new Type[] {
					typeof (RTMTaskItem),
				};
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}
        
		public override bool SupportsItem (Item item) 
		{
			return true;
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> item, Item modItem) 
		{
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			string estimatedTime = String.Empty;
			
			if (modifierItems.FirstOrDefault() != null)
				estimatedTime = ((modifierItems.FirstOrDefault() as ITextItem).Text);
			
			if (!string.IsNullOrEmpty(estimatedTime)) {
				if (!CheckValidTime(estimatedTime)) {
					Services.Notifications.Notify("Remember The Milk",
						"Invalid estimated time provided.");
					yield break;
				}
			}
			
			Services.Application.RunOnThread (() => {
			RTM.SetEstimateTime ((items.First () as RTMTaskItem).ListId,
				(items.First () as RTMTaskItem).TaskSeriesId,
				(items.First () as RTMTaskItem).Id,
				estimatedTime);
			});
			yield break;
		}
	}
}
