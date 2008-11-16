/* GCalendarNewEvent.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Mono.Unix;

using Do.Addins;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Calendar;

namespace GCalendar
{
	public sealed class GCalendarNewEvent : IAction
	{
		public string Name {
			get { return Catalog.GetString ("New Event"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Create a new event in Google Calendar"); }
        }
			
		public string Icon {
			get { return "calIcon.png@" + GetType ().Assembly.FullName; }
		}
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}
		
		public IEnumerable<Type> SupportedModifierItemTypes {
		    get {
		        return new Type[] {
		            typeof (GCalendarItem),
                };
            }
        }
        
        public bool ModifierItemsOptional {
            get {return false; }
        }
        
        public bool SupportsItem (IItem item) 
        {
            return true;
        }
        
        public bool SupportsModifierItemForItems (IEnumerable<IItem> item, IItem modItem) 
        {
            if (modItem is GCalendarItem)
        		return !(modItem as GCalendarItem).Name.Equals ("All Events");
        		
            return true;
        }
        
        public IEnumerable<IItem> DynamicModifierItemsForItem (IItem item) 
        {
            return null;
        }
        
        public IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modifierItems) 
        {
            string calUrl = (modifierItems.First () as GCalendarItem).URL;
            string eventData = (items.First () as ITextItem).Text;
            
            return new IItem [] { GCal2.NewEvent (calUrl, eventData), };
        }
	}
}
