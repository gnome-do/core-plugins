/* GCalClient.cs 
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
using System.Net;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;

using Do.Platform;
using Do.Universe;

namespace GCalendar
{
		
	public class GCalClient
	{
		readonly string AllEventsCalName = AddinManager.CurrentLocalizer.GetString ("All Events");
		readonly string ErrorInMethod = AddinManager.CurrentLocalizer.GetString ("An error has occurred in {0}");

		const int MonthsToIndex = 1;
		const string GAppName = "alexLauni-gnomeDoGCalPlugin-1.5";
		const string CalendarUiUrl = "http://www.google.com/calendar/render";
		readonly string [] keywords = {"from", "until", "in", "after", "before", "on"};
		const string FeedUri = "http://www.google.com/calendar/feeds/default/allcalendars/full";
		
		CalendarService service;
		Dictionary<GCalendarItem, List<GCalendarEventItem>> events;
		
		public GCalClient(string username, string password)
		{
			service = new CalendarService (GAppName);
			service.setUserCredentials (username, password);
			ServicePointManager.CertificatePolicy = new CertHandler ();
			events = new Dictionary<GCalendarItem, List<GCalendarEventItem>> ();
		}

		public IEnumerable<GCalendarItem> Calendars {
			get { return events.Keys; }
		}

		public List<GCalendarEventItem> EventsForCalendar (GCalendarItem calendar)
		{
			List<GCalendarEventItem> calEvents;

			events.TryGetValue (calendar, out calEvents);
			return calEvents;
		}

		public GCalendarEventItem NewEvent (GCalendarItem calendar, string data) 
        {
            EventEntry entry;
            GCalendarEventItem newEvent;
            string url, desc, title, start;

			url = desc = title = start = "";
			
            entry = new EventEntry ();
            entry.QuickAdd = true;
            entry.Content.Content = data;
            			
			try {
				entry = service.Insert (new Uri (calendar.Url), entry);
				
				title = entry.Title.Text;
				desc = entry.Content.Content;
				url = entry.AlternateUri.Content;
				
	            if (entry.Times.Any ()) {	                
	                start  = entry.Times[0].StartTime.ToShortDateString ();
	                desc = start + " - " + desc;
	            }
	            
	        } catch (WebException e) {
				Log.Error (ErrorInMethod, "NewEvent", e.Message);
				Log.Debug (e.StackTrace);
				
				return null;
			}
			
			newEvent = new GCalendarEventItem (title, url, desc);
			events [calendar].Add (newEvent);
            
            return newEvent;
        }

		public IEnumerable<GCalendarEventItem> SearchEvents (IEnumerable<GCalendarItem> calendars, string needle)
        {
        	EventFeed events;
        	EventQuery query;

       		foreach (GCalendarItem calendar in calendars) {
       			query = BuildSearchQuery (calendar, needle);
        	
				try {
					events = service.Query (query);
				} catch (Exception e) {
					Log.Error (ErrorInMethod, "SearchEvents", e.Message);
					Log.Debug (e.StackTrace);
					yield break;
				}
					
				foreach (EventEntry entry in events.Entries) {
					string title, url, desc, start;
				    	
				    title = entry.Title.Text;
				    desc = entry.Content.Content;
				    url = entry.AlternateUri.Content;
				    
				    if (entry.Times.Any ()) {				    	
				        start = entry.Times [0].StartTime.ToShortDateString ();
				        desc = start + " - " + desc;
	                }
	
					yield return new GCalendarEventItem (title, url, desc);
	            }
			}
        }

		public void UpdateCalendars ()
		{
			CalendarQuery query;
			CalendarFeed calendarFeed;
			GCalendarItem allEventsCal;
			Dictionary<GCalendarItem, List<GCalendarEventItem>> cals;

			query = new CalendarQuery (FeedUri);			
			cals = new Dictionary<GCalendarItem, List<GCalendarEventItem>> ();
			allEventsCal = new GCalendarItem (AllEventsCalName, CalendarUiUrl);
			
			// we add this default meta-calendar which contains all events
			cals [allEventsCal] = new List<GCalendarEventItem> ();

			try {
				calendarFeed = service.Query (query);
				
				foreach (CalendarEntry calendar in calendarFeed.Entries) {
					GCalendarItem gcal;
	
					gcal = new GCalendarItem (calendar.Title.Text, calendar.Content.AbsoluteUri);
					cals [gcal] = UpdateEvents (gcal);
					// append the previous calendar's events to the All Calendars calendar
					cals [allEventsCal].AddRange (cals [gcal]);
				}
			} catch (Exception e) {
				Log.Error (ErrorInMethod, "UpdateCalendars", e.Message);
				Log.Debug (e.StackTrace);
			}

			events = new Dictionary<GCalendarItem, List<GCalendarEventItem>> (cals);
		}

		List<GCalendarEventItem> UpdateEvents (GCalendarItem calendar)
		{
			EventQuery query;
			EventFeed eventFeed;
			List<GCalendarEventItem> events;

			query = new EventQuery (calendar.Url);
			query.StartTime = DateTime.UtcNow;
			query.EndTime = DateTime.Now.AddMonths (MonthsToIndex);

			events = new List<GCalendarEventItem> ();

			try {
				eventFeed = service.Query (query);
			
				foreach (EventEntry entry in eventFeed.Entries) {
					string eventTitle, eventUrl, eventDesc, start;

					eventTitle = entry.Title.Text;
					eventDesc = entry.Content.Content;
					eventUrl = entry.AlternateUri.Content;

					// check if the event has associated dates
					if (entry.Times.Any ()) {
				    	start = entry.Times [0].StartTime.ToShortDateString ();
				    	eventDesc = start + " - " + eventDesc;
	                }

					events.Add (new GCalendarEventItem (eventTitle, eventUrl, eventDesc));
		         }
			} catch (Exception e) {			
				Log.Error (ErrorInMethod, "UpdateEvents", e.Message);
				Log.Debug (e.StackTrace);
			}

			return events;
		}

		EventQuery BuildSearchQuery (GCalendarItem calendar, string needle)
		{
			EventQuery query;
       		
       		query = new EventQuery(calendar.Url);
			DateTime [] dates = ParseEventDates (needle);
			query.StartTime = dates [0];
			query.EndTime = dates [1];
        	query.Query = ParseSearchString (needle);

			return query;
		}

		DateTime [] ParseEventDates (string needle)
		{
			needle = needle.ToLower ();
			
			int keydex;
			// String string to just dates if search term + date range found
			foreach (string keyword in keywords) {
				if (needle.Contains(keyword)) {
					keydex = needle.IndexOf (keyword);
					needle = needle.Substring (keydex, needle.Length - keydex);
					break;
				}
			}
			
			// Get date ranges for single date keywords
			if ((needle.StartsWith ("in ") || needle.Contains (" in ")) || 
				needle.Contains ("before ") || needle.Contains ("after ") ||
				(needle.StartsWith ("on ") || needle.Contains (" on ")) ||
			    needle.Contains ("until ") || (needle.Contains ("from ") &&
			    !( needle.Contains (" to ") || needle.Contains("-"))))
					return ParseSingleDate (needle);
					
			else if (needle.Contains ("from "))
				return ParseDateRange (needle);
			else 
				return new DateTime [] {DateTime.Now, DateTime.Now.AddYears(1)};
		}
		
		DateTime [] ParseSingleDate (string needle)
		{
			DateTime [] dates = new DateTime [2];
			if (needle.StartsWith ("in") || needle.Contains (" in ")) {
				needle = StripDatePrefix (needle);
				dates[0] = DateTime.Parse (needle);
				dates[1] = dates[0].AddMonths (1);
			}
			else if (needle.Contains ("before") || needle.Contains ("until ")) {
				needle = StripDatePrefix (needle);
				dates[0] = DateTime.Now;
				dates[1] = DateTime.Parse (needle);
			}
			else if (needle.Contains ("after ") || needle.Contains ("from ")) {
				needle = StripDatePrefix (needle);
				dates[0] = DateTime.Parse (needle);
				dates[1] = dates[0].AddYears (5);
			}
			else if (needle.StartsWith ("on ") || needle.Contains (" on ")) {
				needle = StripDatePrefix (needle);
				dates[0] = DateTime.Parse (needle);
				dates[1] = dates[0].AddDays(1);
			}
			return dates;
		}
		
		DateTime [] ParseDateRange (string needle)
		{
			DateTime [] dates = new DateTime [2];
			needle = needle.ToLower ();
			needle = needle.Substring (needle.IndexOf ("from "), 
				needle.Length - needle.IndexOf ("from "));
			try {
				int seperatorIndex = needle.IndexOf ("-");
				if (seperatorIndex == -1 )
					seperatorIndex = needle.IndexOf (" to ");
				if (seperatorIndex == -1 ) {
					dates[0] = DateTime.Now;
					dates[1] = new DateTime (2012,12,27);
					return dates;
				}
				string start = needle.Substring (0, seperatorIndex);
				if (start.Substring(0,4).Equals ("from"))
					start = start.Substring (4).Trim ();
				dates[0] = DateTime.Parse (start.Trim ());
				
				string end = needle.Substring (seperatorIndex + 1);
				if (end.Contains ("to "))
					end = end.Substring (3);
				dates[1] = DateTime.Parse (end.Trim ());
			} catch (FormatException e) {
				Log.Error (e.Message);
				Log.Debug (e.StackTrace);
			}
			return dates;
		}
		
		string StripDatePrefix (string needle)
		{
			needle = needle.Trim ().ToLower ();
			needle = needle.Substring (needle.IndexOf (" "));
			return needle;
		}
		
		string ParseSearchString (string needle)
		{
			needle = needle.ToLower ();
			foreach (string keyword in keywords) {
			if (needle.Contains (keyword))
				needle = needle.Substring (0,needle.IndexOf (keyword)).Trim ();
			}
			
			return needle;
		}
	}
}
