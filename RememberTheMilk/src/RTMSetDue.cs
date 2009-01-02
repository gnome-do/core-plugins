/* RTMSetDue.cs
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

namespace RememberTheMilk
{
	public class RTMSetDue : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Set Due Date/Time"); }
		}		
				
		public override string Description {
			get { return Catalog.GetString ("Set the due date/time of a task"); }
        }
			
		public override string Icon {
			get { return "task-setdue.png@" + GetType ().Assembly.FullName; }
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
            get { return true; }
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
			RTM.SetDueDateTime ((items.First () as RTMTaskItem).ListId, (items.First () as RTMTaskItem).TaskSeriesId,
			                    (items.First () as RTMTaskItem).Id, (modifierItems.FirstOrDefault () as ITextItem).Text);
			return null;
        }
	}
}