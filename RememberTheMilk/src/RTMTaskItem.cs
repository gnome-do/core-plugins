// RTMTaskItem.cs
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

using Do.Universe;

namespace RememberTheMilk
{
	/// <summary>
	/// Item class for the Remember The Milk tasks.
	/// </summary>
	public class RTMTaskItem : Item, IUrlItem
	{
		string list_id;
		string taskseries_id;
		string task_id;
		string name;
		DateTime due;
		DateTime completed;
		string task_url;
		string priority;
		int has_due_time;
		string estimate;
		string location_id;
		string tags;
		
		public RTMTaskItem (string listId, string taskSeriesId, string taskId, string name) :
			this (listId, taskSeriesId, taskId, name, DateTime.MinValue, DateTime.MinValue, "", "N", 0, "", "", "")
		{
		}
		
		public RTMTaskItem (string listId, string taskSeriesId, string taskId, string name, 
		                    DateTime due, DateTime completed, string taskUrl, 
		                    string priority, int hasDueTime, string estimate, string locationId, string tags)
		{
			this.list_id = listId;
			this.taskseries_id = taskSeriesId;
			this.task_id = taskId;
			this.name = name;
			this.due = due;
			this.completed = completed;
			this.task_url = taskUrl;
			this.priority = priority;
			this.has_due_time = hasDueTime;
			this.estimate = estimate;
			this.location_id = locationId;
			this.tags = tags;
		}

		public override string Name { get { return name; } }
		
		public override string Description {
			get {
				string desc = "";
				
				if (!String.IsNullOrEmpty (tags))
					desc += "[" + tags + "]  ";
				
				if (due != DateTime.MinValue) {
					desc += "Due " + due.ToString ((has_due_time != 0) ? "g" : "d");
					if (completed != DateTime.MinValue)
						desc += " (completed at " + completed.ToString ("g") + ")";
				} else if (completed != DateTime.MinValue)
					desc += "Completed at " + completed.ToString ("g");
				
				return desc;
			}
		}
		
		public override string Icon {
			get {
				string iconName;
				if (completed != DateTime.MinValue)
					iconName = "task-complete";
				else {
					if (priority == "3")
						iconName = "task-low";
					else if (priority == "2")
						iconName = "task-medium";
					else if (priority == "1")
						iconName = "task-high";
					else
						iconName = "task";

					if ((due < DateTime.Now.AddDays (1.0) && due >= DateTime.Now && has_due_time == 1)
					    || (due.Date == DateTime.Today && has_due_time == 0))
						iconName += "-due";
					else if ((due > DateTime.MinValue) &&
					         ((due < DateTime.Now && has_due_time == 1) || due.Date < DateTime.Today))
						iconName += "-overdue";
				}
				return iconName + ".png@" + GetType ().Assembly.FullName;
			}
		}
		
		public string Url {
			get {
				return "http://www.rememberthemilk.com/home/" +
					RTMPreferences.Username + "/" + list_id + "/" + task_id;
			}
		}
		
		public string Id {
			get { return task_id; }
		}
		
		public string ListId {
			get { return list_id; }
		}
		
		public string TaskSeriesId {
			get { return taskseries_id; }
		}
		
		public string TaskUrl {
			get { return task_url; }
		}
		
		public DateTime Due {
			get { return due; }
		}
		
		public DateTime Completed {
			get { return completed; }
		}
		
		public int HasDueTime {
			get { return has_due_time; }
		}
		
		public string Estimate {
			get { return estimate; }
		}
		
		public string LocationId {
			get { return location_id; }
		}

		public string Tags {
			get { return tags; }
		}
	}
}
