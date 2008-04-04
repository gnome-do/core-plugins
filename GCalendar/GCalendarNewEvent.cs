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

using Do.Addins;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Calendar;

namespace Do.GCalendar
{
	public class GCalendarNewEvent : IAction
	{
		public GCalendarNewEvent()
		{
		}
		
		public string Name {
			get { return "New Event"; }
		}
		
		public string Description {
			get { return "Create a new event in Google Calendar"; }
        }
			
		public string Icon {
			get { return "stock_new-appointment"; }
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}
		
		public Type[] SupportedModifierItemTypes {
		    get {
		        return new Type[] {
		            typeof (GCalendarItem),
                };
            }
        }
        
        public bool ModifierItemsOptional {
            get {return false; }
        }
        
        public bool SupportsItem (IItem item) {
            return true;
        }
        
        public bool SupportsModifierItemForItems (IItem[] item, IItem modItem) {
            return true;
        }
        
        public IItem[] DynamicModifierItemsForItem (IItem item) {
            return null;
        }
        
        public IItem[] Perform (IItem[] items, IItem[] modifierItems) {
            DoGCal service = new DoGCal ();
            string cal_url = (modifierItems[0] as GCalendarItem).URL;
            string event_data = (items[0] as ITextItem).Text;
            
            EventEntry entry = service.NewEvent (cal_url, event_data);
            
            string eventUrl = entry.AlternateUri.Content;
            string eventDesc = entry.Content.Content;
            if (entry.Times.Count > 0) {
                string start = entry.Times[0].StartTime.ToString ();
                start = start.Substring (0,start.IndexOf (' '));
                eventDesc = start + " - " + eventDesc;
            }
            return new IItem[] { (new GCalendarEventItem (entry.Title.Text, eventUrl,
                    eventDesc)), };
        }
	}
}
