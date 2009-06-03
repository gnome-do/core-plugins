// RTMNewList.cs
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
	/// Class for the "New List" action.
	/// </summary>
	public class RTMNewList : Act
	{
		public override string Name {
			get { return Catalog.GetString ("New List"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Create a new task list."); }
		}
		
		public override string Icon {
			get { return "task-add.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
				
		public override bool SupportsItem (Item item) {
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{			
			if (items.Any ()) {
				string newListName = (items.First () as ITextItem).Text;
				if (String.IsNullOrEmpty (newListName)) {
					Log.Debug ("[RememberTheMilk] No list name provided for RTMNewList action");
					yield break;
				} else {
					Services.Application.RunOnThread (() => {
						RTM.NewList (newListName);
					});
				}
			}
			yield break;
		}
	}
}
