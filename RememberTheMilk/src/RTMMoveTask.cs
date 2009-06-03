// RTMMoveTask.cs
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
	/// <summary>
	/// Class for the "Move Task" action.
	/// </summary>
	public class RTMMoveTask : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Move to ..."); }
		}		
				
		public override string Description {
			get { return Catalog.GetString ("Move the seleted task to another list"); }
		}
			
		public override string Icon {
			get { return "forward"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (RTMTaskItem); }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
		    get { yield return typeof (RTMListItem); }
		}
		
		public override bool ModifierItemsOptional {
			get { return false; }
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> item, Item modItem) 
		{
			if (modItem is RTMListItem)
				return !(modItem as RTMListItem).Smart;
			
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			Services.Application.RunOnThread (() => {
				RTM.MoveTask ((items.First () as RTMTaskItem).ListId, (modifierItems.First () as RTMListItem).Id,
				              (items.First () as RTMTaskItem).TaskSeriesId, (items.First () as RTMTaskItem).Id);
			});
			yield break;
		}
	}
}
