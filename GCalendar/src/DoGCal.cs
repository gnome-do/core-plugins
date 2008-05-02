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
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Google.GData.Client;
using Google.GData.Calendar;

using Gnome.Keyring;
using Do.Universe;

namespace GCalendar
{
	public class DoGCal
	{
		private static string username, password;
		private static CalendarService service;
		private static List<IItem> calendars;
		private static Timer ClearCalendarsTimer;
		const int CacheSeconds  = 500; //cache contacts
			
		static DoGCal ()
		{	
			calendars = new List<IItem> ();
			ClearCalendarsTimer = new Timer (ClearCalendars);
			SetCredentials ();
			try {
				Connect ();
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		
		public static List<IItem> Calendars {
			get { return calendars; }
		}

		public static void Connect () 
		{
			try {
				service = new CalendarService("alexLauni-gnomeDoGCalPlugin-1");
				service.setUserCredentials(username, password);
				//Console.Error.WriteLine("GCal: Connected");
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		public static void UpdateCalendars ()
		{		
			if (!Monitor.TryEnter (calendars)) return;
			if (calendars.Count > 0) {
				//Console.Error.WriteLine("GCal: Cache not cleared, returning");
				Monitor.Exit (calendars);
				return;
			}
			
			ClearCalendarsTimer.Change (CacheSeconds*1000, Timeout.Infinite);
			AtomFeed cal_list = QueryCalendars ();
			if (cal_list == null) return;
			
			for (int i = 0; i < cal_list.Entries.Count; i++) {
				string cal_url = cal_list.Entries[i].Id.Uri.ToString ();
				calendars.Add (new GCalendarItem (cal_list.Entries[i].Title.Text, 
				        cal_url.Replace ("/default","") + "/private/full"));
			}
			Monitor.Exit (calendars);
			//Console.Error.WriteLine("GCal: Calendar list set");
		}
		
		public static AtomFeed QueryCalendars ()
		{			
			FeedQuery query = new FeedQuery ();
			query.Uri = new Uri ("http://www.google.com/calendar/feeds/default");
			try {
				//Console.Error.WriteLine("GCal: Querying calendars");
				return service.Query (query);
			} catch (Exception e) {
				Console.Error.WriteLine (e);
			}
			return null;
		}
				
		public static EventFeed GetEvents (string calUrl)
		{
			return GetEvents (calUrl, DateTime.Now, DateTime.Now.AddMonths(1));
		}
		
		public static EventFeed GetEvents (string calUrl, DateTime startTime,
		                            DateTime endTime)
        {	
            EventQuery query = new EventQuery (calUrl);
            query.StartTime = startTime;
            query.EndTime = endTime;
			query.SortOrder = CalendarSortOrder.ascending;
			EventFeed events;
			try {
				//Console.Error.WriteLine("GCal: Getting list of events");
				events = service.Query (query) as EventFeed;
			} catch (WebException e) {
				events = null;
				Console.Error.WriteLine (e);
			}
			return events;
        }
        
        public static EventFeed SearchEvents (string calUrl, string needle) 
        {
			string [] keywords = {"from ","until "," in ", "after ", "before ", " on "};
            EventQuery query = new EventQuery(calUrl);
			DateTime [] dates = ParseEventDates (needle,keywords);
			query.StartTime = dates[0];
			query.EndTime = dates[1];
			//Console.Error.WriteLine ("Start Date: {0}",dates[0]);
			//Console.Error.WriteLine ("End Date: {0}",dates[1]);
            query.Query = ParseSearchString (needle,keywords);
			EventFeed events;
			try {
				//Console.Error.WriteLine("GCal: Searching events");
				events = service.Query(query) as EventFeed;
			} catch (WebException e) {
				events = null;
				Console.Error.WriteLine (e);
			}
            return events; 
        }
        
        public static EventEntry NewEvent (string calUrl, string data) 
        {
            EventEntry entry = new EventEntry ();
            entry.QuickAdd = true;
            entry.Content.Content = data;
            Uri post_uri = new Uri(calUrl);
			EventEntry nevent;
			try {
				nevent = service.Insert(post_uri, entry) as EventEntry;
			} catch (WebException e) {
				nevent = null;
				Console.Error.WriteLine (e);
			}
            return nevent;
        }
		
		private static void SetCredentials ()
		{
			string keyring_item_name = "Google Account";
			Hashtable request_attributes = new Hashtable();
			request_attributes["name"] = keyring_item_name;
			try {
				foreach(ItemData result in Ring.Find(ItemType.GenericSecret, request_attributes)) {
					if(!result.Attributes.ContainsKey("name") || !result.Attributes.ContainsKey("username") ||
						(result.Attributes["name"] as string) != keyring_item_name) 
						continue;
					
					username = (string)result.Attributes["username"];
					password = result.Secret;

					if (username == null || username == String.Empty || password == null || password == String.Empty)
						throw new ApplicationException ("Invalid username/password in keyring");
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				string account_info = Environment.GetEnvironmentVariable ("HOME")
					+ "/.config/gnome-do/google-name";
				try {
					StreamReader reader = File.OpenText(account_info);
					username = reader.ReadLine ();
					password = reader.ReadLine ();
					WriteAccount ();
				} catch { 
					Console.Error.WriteLine ("[ERROR] " + account_info + " cannot be read!");
				} 
			}
		}
		
		private static void ClearCalendars (object state)
		{
			lock (calendars) {
				calendars.Clear ();
			}		
		}
		
		private static DateTime [] ParseEventDates (string needle, string [] keywords)
		{
			needle = needle.ToLower ();
			Console.Error.WriteLine ("Raw String: {0}",needle);
			
			int keydex;
			// String string to just dates if search term + date range found
			foreach (string keyword in keywords) {
				if (needle.Contains(keyword)) {
					keydex = needle.IndexOf (keyword);
					needle = needle.Substring (keydex, needle.Length - keydex);
					Console.Error.WriteLine ("Needle stripped to {0}",needle);
					break;
				}
			}
			
			// Get date ranges for single date keywords
			if (needle.Contains ("in ") || needle.Contains ("before ") || needle.Contains ("after ")
			    || needle.Contains (" on ") || needle.Contains ("until ") || 
			    (needle.Contains ("from ") && !( needle.Contains (" to ") || needle.Contains("-"))))
				return ParseSingleDate (needle);
			else if (needle.Contains ("from "))
				return ParseDateRange (needle);
			else 
				return new DateTime [] {DateTime.Now, DateTime.Now.AddYears(1)};
		}
		
		private static DateTime [] ParseSingleDate (string needle)
		{
			DateTime [] dates = new DateTime [2];
			if (needle.Contains ("in ")) {
				needle = StripDatePrefix (needle);
				Console.Error.WriteLine ("Parsing {0}",needle);
				dates[0] = DateTime.Parse (needle);
				dates[1] = dates[0].AddMonths (1);
			}
			else if (needle.Contains ("before") || needle.Contains ("until ")) {
				needle = StripDatePrefix (needle);
				dates[0] = DateTime.Now;
				Console.Error.WriteLine ("Parsing {0}",needle);
				dates[1] = DateTime.Parse (needle);
			}
			else if (needle.Contains ("after ") || needle.Contains ("from ")) {
				needle = StripDatePrefix (needle);
				Console.Error.WriteLine ("Parsing {0}",needle);
				dates[0] = DateTime.Parse (needle);
				dates[1] = dates[0].AddYears (5);
			}
			else if (needle.Contains (" on ")) {
				needle = StripDatePrefix (needle);
				Console.Error.WriteLine ("Parsing {0}",needle);
				dates[0] = DateTime.Parse (needle);
				dates[1] = dates[0].AddDays(1);
			}
			return dates;
		}
		
		private static DateTime [] ParseDateRange (string needle)
		{
			DateTime [] dates = new DateTime [2];
			needle = needle.ToLower ();
			needle = needle.Substring (needle.IndexOf ("from "), needle.Length - needle.IndexOf ("from "));
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
			Console.Error.WriteLine ("Raw needle: {0}",needle);
			needle = needle.Trim ().ToLower ();
			Console.Error.WriteLine ("Needle phase 1: {0}",needle);
			Console.Error.WriteLine ("Index of first space: {0}",needle.IndexOf (" "));
			needle = needle.Substring (needle.IndexOf (" "));
			Console.Error.WriteLine ("Needle phase 2: {0}",needle);
			return needle;
		}
		
		private static string ParseSearchString (string needle, string[] keywords)
		{
			needle = needle.ToLower ();
			foreach (string keyword in keywords) {
			if (needle.Contains (keyword))
				needle = needle.Substring(0,needle.IndexOf (keyword)).Trim ();
			}
			
			Console.Error.WriteLine ("Searching for {0}",needle);
			return needle;				                        
		}
		
		private static void WriteAccount ()
		{
			string keyring_item_name = "Google Account";
			string keyring;
			try {
				keyring = Ring.GetDefaultKeyring();
			} catch {
				username = null;
				password = null;
				return;
			}
			Hashtable update_request_attributes = new Hashtable();
			update_request_attributes["name"] = keyring_item_name;
			update_request_attributes["username"] = username;

			try {
				Ring.CreateItem(keyring, ItemType.GenericSecret, keyring_item_name, update_request_attributes, password, true);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}
	}
}
