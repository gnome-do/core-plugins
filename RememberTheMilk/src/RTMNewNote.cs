// RTMNewNote.cs
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
using System.Text.RegularExpressions;
using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	/// <summary>
	/// Class for the "New Note" action
	/// </summary>
	public class RTMNewNote : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("New Note"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Add a new note to a task."); }
		}
			
		public override string Icon {
			get { return "note-add.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (RTMTaskItem); }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}
        
		public override bool ModifierItemsOptional {
			get { return false; }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			string note = String.Empty;
			
			if (modifierItems.FirstOrDefault() != null) {
				note = ((modifierItems.FirstOrDefault() as ITextItem).Text);
			}
			
			Services.Application.RunOnThread (() => {
				RTM.NewNote ((items.First () as RTMTaskItem).ListId,
					(items.First () as RTMTaskItem).TaskSeriesId,
					(items.First () as RTMTaskItem).Id, note);
			});
			yield break;
		}
	}
}