/* DoGCal.cs
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
using System.IO;
using System.Net;

using Google.GData.Client;
using Google.GData.Calendar;

using GConf;

namespace Do.GCalendar
{
	public class DoGCal
	{
		private static string username, password;
		private static CalendarService service;
		private static bool is_updating_calendars = false;
		private static bool is_updating_events = false;
		private static AtomFeed calendars;
		
		static DoGCal ()
		{
			GConf.Client gconf = new GConf.Client ();
			try {
				username = gconf.Get ("/apps/gnome-do/plugins/gcal/username") as string;
				password = gconf.Get ("/apps/gnome-do/plugins/gcal/password") as string;
			} catch (GConf.NoSuchKeyException) {
				gconf.Set ("/apps/gnome-do/plugins/gcal/username","");
				gconf.Set ("/apps/gnome-do/plugins/gcal/password","");
			}
			Connect ();
			UpdateCalendars ();
		}
		
		public static AtomFeed Calendars {
			get { return calendars; }
		}

		public static void Connect () 
		{
			service = new CalendarService("alexLauni-gnomeDoGCalPlugin-1");
            service.setUserCredentials(username, password);	
		}
		
		public static void UpdateCalendars ()
		{
			if(is_updating_calendars) return;
			
			is_updating_calendars = true;
			
			FeedQuery query = new FeedQuery ();
			query.Uri = new Uri ("http://www.google.com/calendar/feeds/default");
			calendars = service.Query (query);
			
			is_updating_calendars = false;
		}
		
		public static EventFeed GetEvents (string calUrl)
		{
			return GetEvents (calUrl, DateTime.Now, DateTime.Now.AddMonths(1));
		}
		
		public static EventFeed GetEvents (string calUrl, DateTime startTime,
		                            DateTime endTime)
        {
			if (is_updating_events) return null;
			
			is_updating_events = true;
			
            EventQuery query = new EventQuery (calUrl);
            query.StartTime = startTime;
            query.EndTime = endTime;
			query.SortOrder = CalendarSortOrder.ascending;
			EventFeed events = service.Query (query) as EventFeed;
			
			is_updating_events = false;
			
			return events;
        }
        
        public static EventFeed SearchEvents (string calUrl, string needle) 
        {
            EventQuery query = new EventQuery(calUrl);
			query.StartTime = DateTime.Now;
            query.Query = needle;

            return service.Query(query) as EventFeed;
        }
        
        public static EventEntry NewEvent (string calUrl, string data) 
        {
            EventEntry entry = new EventEntry ();
            entry.QuickAdd = true;
            entry.Content.Content = data;
            Uri post_uri = new Uri(calUrl);
            
            return (EventEntry) service.Insert(post_uri, entry);
        }
	}
}
