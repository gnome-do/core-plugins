// RTMAddTags.cs
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
	/// Class to provide the "Add Tag(s)" action
	/// </summary>
	public class RTMAddTags : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Add Tag(s)"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Add one or more tags to a task."); }
		}
			
		public override string Icon {
			get { return "tag-add.png@" + GetType ().Assembly.FullName; }
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
				yield return typeof (RTMTagItem);
			}
		}
		
		public override bool SupportsItem (Item item) {
			if (item is RTMTaskItem)
				return true;
			else if (item is RTMTaskAttributeItem)
				return (item as RTMTaskAttributeItem).Description == "Tags";
			else
				return false;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			RTMTaskItem task = null;
			List<string> temp_tags = new List<string> ();
			string s = null;
			
			if (items.Any()) {
				if (items.First () is RTMTaskItem)
					task = (items.First () as RTMTaskItem);
				else if (items.First () is RTMTaskAttributeItem)
					task = (items.First () as RTMTaskAttributeItem).Parent;
			}
			
			if (modifierItems.Any () && task != null) {
				foreach (Item item in modifierItems) {
					s = GetText (item);
					if (!String.IsNullOrEmpty(s))
						temp_tags.Add (s);
				}
				
				Services.Application.RunOnThread (() => {
					RTM.AddTags ((items.First () as RTMTaskItem).ListId, (items.First () as RTMTaskItem).TaskSeriesId,
					             (items.First () as RTMTaskItem).Id, String.Join (",", temp_tags.ToArray ()));
				});
			}
			yield break;
		}
		
		protected string GetText (Item item)
		{
			if (item is ITextItem)
				return (item as ITextItem).Text;
			if (item is RTMTagItem)
				return (item as RTMTagItem).Name;
			throw new Exception ("Inappropriate Item type.");
		}
	}
}