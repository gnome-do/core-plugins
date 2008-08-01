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
		
		private static List<IItem> calendars;
		private static object cal_lock;
		
		private static Dictionary<string,List<IItem>> events;
		private static object events_lock;
		
		const string FeedUri = "http://www.google.com/calendar/feeds/default";
		private static string gAppName = "alexLauni-gnomeDoGCalPlugin-1.2";
		const int CacheSeconds  = 500;
		
		static GCal2 ()
		{
			string username, password;
			//Google works over SSL, we need accept the cert.
			System.Net.ServicePointManager.CertificatePolicy = new CertHandler ();
			
			calendars = new List<IItem> ();
			cal_lock = new object ();
			
			events = new Dictionary<string,List<IItem>> ();
			events_lock = new object ();
			
			Configuration.GetAccountData (out username, out password,
				typeof (Configuration));
			Connect (username, password);
			
			GLib.Timeout.Add (CacheSeconds * 1000, delegate { 
				ClearCollection (calendars, cal_lock); 
				return true; 
			});
			
			GLib.Timeout.Add (CacheSeconds * 1000, delegate {
				ClearCollection (events, events_lock); 
				return true; 
			});
		}
		
		public static string GAppName {
			get { return gAppName; }
		}
		
		private static void Connect (string username, string password) 
		{
			try {
				service = new CalendarService (gAppName);
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		
		public static List<IItem> Calendars {
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
			
			lock (cal_lock) {
				foreach (AtomEntry calendar in calendarFeed.Entries) {
					string cal_url = calendar.Id.Uri.Content.Replace ("/default","") + "/private/full";
					calendars.Add (new GCalendarItem (calendar.Title.Text, cal_url));
				}
			}
		}
		
		public static List<IItem> EventsForCalendar (string calendarName)
		{
			Console.Error.WriteLine ("Looking for {0}.\nFound:", calendarName);
			foreach (string name in events.Keys) {
				Console.Error.WriteLine (name);
			}
			
			return events [calendarName];
		}
		
		public static void UpdateEvents ()
		{
			string eventUrl, eventDesc, start;
			Dictionary<string,string> cals = new Dictionary<string,string> ();
			lock (cal_lock) {
				foreach (GCalendarItem cal in calendars) {
					try {
						cals.Add (cal.Name, cal.URL);
					} catch (ArgumentException) { }
				}
				if (cals.Count == 0) return;
			}
			
			lock (events_lock) {
				foreach (string cal in cals.Keys) {
					if (!events.ContainsKey (cal))
						events [cal] = new List<IItem> ();
					//If our event list hasn't ben cleared, dont bother updating it
					if (events [cal].Count != 0) continue;
					
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
				         }
					} catch (WebException e) {			
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
        
        public static GCalendarEventItem [] SearchEvents (string calUrl, string needle)
        {
        	List<GCalendarEventItem> events = new List<GCalendarEventItem> ();
        	string eventUrl, eventDesc, start;
       		EventQuery query = new EventQuery(calUrl);
			//DateTime [] dates = ParseEventDates (needle,keywords);
			//query.StartTime = dates[0];
			//query.EndTime = dates[1];
        	query.Query = ""; //ParseSearchString (needle,keywords);
			try {
				foreach (EventEntry entry in (service.Query(query) as EventFeed).Entries) {
				    eventUrl = entry.AlternateUri.Content;
				    eventDesc = entry.Content.Content;
				    if (entry.Times.Count > 0) {
				        start = entry.Times[0].StartTime.ToString ();
				        start = start.Substring (0,start.IndexOf (' '));
				        eventDesc = start + " - " + eventDesc;
	                }
	                events.Add (new GCalendarEventItem (entry.Title.Text, eventUrl,
			            eventDesc));
	            }
			} catch (WebException e) {
				Console.Error.WriteLine (e.Message);
				return null;
			}
			return events.ToArray ();
        }
        
        private static void ClearCollection<T> (ICollection<T> calendars, object listLock)
        {
        	lock (listLock) {
        		calendars.Clear ();
        	}
        }
	}
}
