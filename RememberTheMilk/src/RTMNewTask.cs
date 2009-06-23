// RTMNewTask.cs
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
	/// Class for the "New Task" action.
	/// </summary>
	public class RTMNewTask : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("New Task"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Create a new task in Remember The Milk"); }
		}
			
		public override string Icon {
			get { return "task-add.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (RTMListItem); }
		}
        
		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> item, Item modItem) 
		{
			if (modItem is RTMListItem)
				return !(modItem as RTMListItem).Smart;
			
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			string listId = String.Empty;
			string taskData = (items.First () as ITextItem).Text;
			
			if (string.IsNullOrEmpty(taskData)) {
				Services.Notifications.Notify ("Remember The Milk", 
					AddinManager.CurrentLocalizer.GetString ("No title provided for new task."));
				yield break;
			}
			
			if (modifierItems.FirstOrDefault () != null)
				listId = (modifierItems.FirstOrDefault () as RTMListItem).Id;
			
			if (RTMPreferences.ReturnNewTask)
				yield return RTM.NewTask (listId, taskData);
			else {
				Services.Application.RunOnThread (() => {
					RTM.NewTask (listId, taskData);
				});
				yield break;
			}
		}
	}
}
