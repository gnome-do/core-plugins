// RTMListItem.cs
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

using Do.Universe;

namespace RememberTheMilk
{	
	public class RTMListItem : Item, IUrlItem
	{
		string list_id;
		string name;
		int locked;
		int smart;
				
		public RTMListItem (string listId, string name) : this (listId, name, 0, 0)
		{
		}
		
		public RTMListItem (string listId, string name, int locked, int smart)
		{
			this.list_id = listId;
			this.name = name;
			this.locked = locked;
			this.smart = smart;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return "Remember The Milk Task List"; }
		}
		
		public override string Icon {
			get { 
				if (list_id == "Today's Tasks")
					return "task-due.png@" + GetType ().Assembly.FullName;
				else
					return "task.png@" + GetType ().Assembly.FullName; 
			}
		}
		
		public string Id {
			get { return list_id; }
		}
		
		public string Url {
			get {
				if (list_id == "All Tasks")
					return "http://www.rememberthemilk.com/home/" + RTM.Preferences.Username;
				else 
					return "http://www.rememberthemilk.com/home/" + RTM.Preferences.Username + "/" + list_id;
			}
		}
		
		public bool Locked {
			get { return (locked == 1) ? true : false; }
		}
		
		public bool Smart {
			get { return (smart == 1) ? true : false; }
		}
	}
}
