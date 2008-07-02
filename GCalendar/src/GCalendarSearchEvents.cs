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
using Mono.Unix;

using Do.Addins;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Calendar;

namespace GCalendar {
    public sealed class GCalendarSearchEvents : IAction
    {
        public GCalendarSearchEvents()
        {
        }
        
        public string Name {
            get { return Catalog.GetString ("Search Events"); }
        }
        
        public string Description {
            get { return Catalog.GetString ("Search Google Calendar for Events"); }
        }
        
        public string Icon {
            get { return "calIcon.png@" + GetType ().Assembly.FullName; }
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
            string eventUrl, eventDesc, start;
            foreach (IItem item in items) {
                search_text += (item as ITextItem).Text;
            }
            EventFeed events = GCal.SearchEvents ((modifierItems[0] as GCalendarItem).URL,
                            search_text);
			foreach (EventEntry entry in events.Entries) {
			    eventUrl = entry.AlternateUri.Content;
			    eventDesc = entry.Content.Content;
			    if (entry.Times.Count > 0) {
			        start = entry.Times[0].StartTime.ToString ();
			        start = start.Substring (0,start.IndexOf (' '));
			        eventDesc = start + " - " + eventDesc;
                }
			    cal_items.Add (new GCalendarEventItem (entry.Title.Text, eventUrl,
			            eventDesc));
            }
			return cal_items.ToArray ();
        }
    }
}
            
