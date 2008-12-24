/* GCal2.cs
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
using System.Xml;
using System.Threading;
using System.Collections.Generic;

using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;

using Do.Universe;

namespace GCalendar
{
	public static class GCal2
	{
		private static CalendarService service;
		private static List<Item> calendars;
		private static object cal_lock;
		
		private static Dictionary<string,List<Item>> events;
		private static object events_lock;
		
		const string FeedUri = "http://www.google.com/calendar/feeds/default";
		private static string gAppName = "alexLauni-gnomeDoGCalPlugin-1.2";
		const int CacheSeconds  = 500;
		
		static GCal2 ()
		{
			string username, password;
			//Google works over SSL, we need accept the cert.
			System.Net.ServicePointManager.CertificatePolicy = new CertHandler ();
			
			calendars = new List<Item> ();
			cal_lock = new object ();
			
			events = new Dictionary<string,List<Item>> ();
			events_lock = new object ();
			
			Configuration.GetAccountData (out username, out password,
				typeof (Configuration));
			Connect (username, password);
		}
		
		public static string GAppName {
			get { return gAppName; }
		}
		
		public static bool Connect (string username, string password) 
		{
			try {
				service = new CalendarService (gAppName);
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return false;
			}
			return true;
		}
		
		public static List<Item> Calendars {
			get { return calendars; }
		}
		
		public static void UpdateCalendars ()
		{
			AtomFeed calendarFeed;
			FeedQuery query = new FeedQuery ();
			query.Uri = new Uri (FeedUri);
			try {
				calendarFeed = service.Query (query);
			} catch (Exception e) {
				calendarFeed = null;
				Console.Error.WriteLine (e.Message);
				return;
			}
			
			calendars.Clear ();
			calendars.Add  (new GCalendarItem ("All Events", ""));
			foreach (AtomEntry calendar in calendarFeed.Entries) {
				string cal_url = calendar.Id.Uri.Content.Replace ("/default","") + "/private/full";
				calendars.Add (new GCalendarItem (calendar.Title.Text, cal_url));
			}
		}
		
		public static List<Item> EventsForCalendar (string calendarName)
		{
			return events [calendarName];
		}
		
		public static void UpdateEvents ()
		{
			string eventUrl, eventDesc, start;
			Dictionary<string,string> cals = new Dictionary<string,string> ();
			lock (cal_lock) {
				try {
					foreach (GCalendarItem cal in calendars) {	
						cals.Add (cal.Name, cal.Url);
					}
					cals.Add ("All Events", "");
				} catch (ArgumentException) { }
			}
			if (cals.Count <= 1) return;
			
			lock (events_lock) {
				events.Clear ();
				events ["All Events"] = new List<Item> ();
				foreach (string cal in cals.Keys) {
					if (!events.ContainsKey (cal))
						events [cal] = new List<Item> ();
					
					EventQuery query = new EventQuery (cals [cal]);
					query.StartTime = DateTime.Now;
					query.EndTime = DateTime.Now.AddMonths (1);
					try {
						foreach (EventEntry entry in (service.Query (query) as EventFeed).Entries) {
							eventUrl = entry.AlternateUri.Content;
							eventDesc = entry.Content.Content;
							if (entry.Times.Count > 0) {
						    	start = entry.Times [0].StartTime.ToString ();
						    	start = start.Substring (0,start.IndexOf (' '));
						    	eventDesc = start + " - " + eventDesc;
			                }
							events [cal].Add (new GCalendarEventItem (entry.Title.Text, eventUrl,
								eventDesc));
							events ["All Events"].Add (new GCalendarEventItem (entry.Title.Text, eventUrl,
								eventDesc));
				         }
					} catch (WebException e) {			
						Console.Error.WriteLine (e.Message);
					} catch (GDataRequestException e) {
						Console.Error.WriteLine (e.Message);
					} catch (ArgumentException e) { 
						Console.Error.WriteLine (e.Message); 
					} catch (XmlException e) {
						Console.Error.WriteLine (e.Message);
					}
				}
			}
		}
		
		public static GCalendarEventItem NewEvent (string calUrl, string data) 
        {
            EventEntry entry = new EventEntry ();
            entry.QuickAdd = true;
            entry.Content.Content = data;
            Uri post_uri = new Uri(calUrl);
			EventEntry nevent;
			try {
				nevent = service.Insert(post_uri, entry) as EventEntry;
			} catch (WebException e) {
				Console.Error.WriteLine (e.Message);
				return null;
			}
			string eventUrl = nevent.AlternateUri.Content;
            string eventDesc = nevent.Content.Content;
            if (nevent.Times.Count > 0) {
                string start = nevent.Times[0].StartTime.ToString ();
                start = start.Substring (0,start.IndexOf (' '));
                eventDesc = start + " - " + eventDesc;
            }
            
            return new GCalendarEventItem (nevent.Title.Text, eventUrl, eventDesc);
        }
        
        public static Item [] SearchEvents (string calUrl, string needle)
        {
        	List<Item> events = new List<Item> ();
        	string [] keywords = {"from ","until ","in ", "after ", "before ", "on "};
        	string eventUrl, eventDesc, start;
       		EventQuery query = new EventQuery(calUrl);
			DateTime [] dates = ParseEventDates (needle,keywords);
			query.StartTime = dates[0];
			query.EndTime = dates[1];
        	query.Query = ParseSearchString (needle, keywords);
			try {
				foreach (EventEntry entry in (service.Query(query) as EventFeed).Entries) {
				    eventUrl = entry.AlternateUri.Content;
				    eventDesc = entry.Content.Content;
				    if (entry.Times.Count > 0) {
				        start = entry.Times[0].StartTime.ToString ();
				        start = start.Substring (0,start.IndexOf (' '));
				        eventDesc = start + " - " + eventDesc;
	                }
	                Console.Error.WriteLine ("Adding event {0}", entry.Title.Text);
	                events.Add (new GCalendarEventItem (entry.Title.Text, eventUrl,
			            eventDesc));
	            }
			} catch (WebException e) {
				Console.Error.WriteLine (e.Message);
				return null;
			}
			return events.ToArray ();
        }
        
        private static DateTime [] ParseEventDates (string needle, string [] keywords)
		{
			needle = needle.ToLower ();
			
			int keydex;
			// String string to just dates if search term + date range found
			foreach (string keyword in keywords) {
				if (needle.Contains(keyword)) {
					keydex = needle.IndexOf (keyword);
					needle = needle.Substring (keydex, needle.Length - keydex);
					//Console.Error.WriteLine ("Needle stripped to {0}",needle);
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
		
		private static DateTime [] ParseSingleDate (string needle)
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
		
		private static DateTime [] ParseDateRange (string needle)
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
					Console.Error.WriteLine (e.Message);
			}
			return dates;
		}
		
		private static string StripDatePrefix (string needle)
		{
			needle = needle.Trim ().ToLower ();
			needle = needle.Substring (needle.IndexOf (" "));
			return needle;
		}
		
		private static string ParseSearchString (string needle, string[] keywords)
		{
			needle = needle.ToLower ();
			foreach (string keyword in keywords) {
			if (needle.Contains (keyword))
				needle = needle.Substring(0,needle.IndexOf (keyword)).Trim ();
			}
			
			return needle;
		}
	}
}
