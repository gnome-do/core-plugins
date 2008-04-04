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
		string username, password;
		CalendarService service;
		
		public DoGCal ()
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
		}

		public void Connect () 
		{
			service = new CalendarService("alexLauni-gnomeDoGCalPlugin-1");
            service.setUserCredentials(username, password);	
		}
		
		public AtomFeed GetCalendars ()
		{
			FeedQuery query = new FeedQuery ();
            query.Uri = new Uri ("http://www.google.com/calendar/feeds/default");
            
            return service.Query (query);
		}
		
		public EventFeed GetEvents (string calUrl)
		{
			return GetEvents (calUrl, DateTime.Now, DateTime.Now.AddMonths(1));
		}
		
		public EventFeed GetEvents (string calUrl, DateTime startTime,
		                            DateTime endTime)
        {
            EventQuery query = new EventQuery (calUrl);
            query.StartTime = startTime;
            query.EndTime = endTime;
                    
            return service.Query (query) as EventFeed;
        }
        
        public EventFeed SearchEvents (string calUrl, string needle) 
        {
            EventQuery query = new EventQuery(calUrl);
            query.Query = needle;

            return service.Query(query) as EventFeed;
        }
        
        public EventEntry NewEvent (string calUrl, string data) 
        {
            EventEntry entry = new EventEntry ();
            entry.QuickAdd = true;
            entry.Content.Content = data;
            Uri post_uri = new Uri(calUrl);
            
            return (EventEntry) service.Insert(post_uri, entry);
        }
	}
}
