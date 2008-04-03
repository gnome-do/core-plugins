/* GCalendarEventItem.cs
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

namespace Do.GCalendar {
    public class GCalendarSearchEvents : IAction
    {
        public GCalendarSearchEvents()
        {
        }
        
        public string Name {
            get { return "Search Google Calendar"; }
        }
        
        public string Description {
            get { return "Search Google Calendar for Events"; }
        }
        
        public string Icon {
            get { return "date"; }
        }
        
        public Type[] SupportedItemTypes {
            get {
                return new Type[] {
                    typeof (GCalendarItem),
                };
            }
        }
        
        public Type[] SupportedModifierItemTypes {
            get { 
                return new Type[] {
                    typeof (ITextItem),
                };
            }
        }
        
        public bool ModifierItemsOptional {
            get { return false; }
        }
        
        public bool SupportsItem (IItem item) {
            return true;
        }
        
        public bool SupportsModifierItemForItems (IItem[] items, IItem modItem) {
            return true;
        }
        
        public IItem[] DynamicModifierItemsForItem (IItem item) {
            return null;
        }
        
        public IItem[] Perform (IItem[] items, IItem[] modifierItems) {
            List<IItem> cal_items = new List<IItem> ();
            string search_text = "";
            string eventUrl, eventDesc;
            DoGCal service = new DoGCal ();
            foreach (IItem item in modifierItems) {
                search_text += (item as ITextItem).Text;
            }
            
            EventFeed events = service.SearchEvents ((items[0] as GCalendarItem).URL,
                            search_text);
            for (int i = 0 ; i < events.Entries.Count; i++) {
			    eventUrl = events.Entries[i].AlternateUri.Content;
			    eventDesc = events.Entries[i].Content.Content;
				cal_items.Add (new GCalendarEventItem (events.Entries[i].Title.Text, eventUrl,
				        eventDesc));
			}
			return cal_items.ToArray ();
        }
    }
}
            
