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

using Mono.Unix;

using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;

using Do.Platform;
using Do.Universe;

namespace GCalendar
{
		
	public class GCalClient
	{
		readonly string AllEventsCalName = Catalog.GetString ("All Events");
		readonly string ErrorInMethod = Catalog.GetString ("An error has occurred in {0}");

		const int MonthsToIndex = 1;
		const string GAppName = "alexLauni-gnomeDoGCalPlugin-1.5";
		const string CalendarUiUrl = "http://www.google.com/calendar/render";
		const string FeedUri = "http://www.google.com/calendar/feeds/default/allcalendars/full";
		
		CalendarService service;
		Dictionary<GCalendarItem, List<GCalendarEventItem>> events;
		
		public GCalClient()
		{
			service = new CalendarService (GAppName);
			service.setUserCredentials ("alex.launi@gmail.com", "l4z3rtr0ss");
			ServicePointManager.CertificatePolicy = new CertHandler ();
			events = new Dictionary<GCalendarItem, List<GCalendarEventItem>> ();
		}

		public IEnumerable<GCalendarItem> Calendars {
			get { return events.Keys; }
		}

		public IEnumerable<GCalendarEventItem> EventsForCalendar (GCalendarItem calendar)
		{
			List<GCalendarEventItem> calEvents;

			events.TryGetValue (calendar, out calEvents);
			return calEvents;
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
					string calUrl;
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

			events = cals;
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
					if (entry.Times.Count > 0) {
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
	}
}