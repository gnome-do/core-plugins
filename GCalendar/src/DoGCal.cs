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

using Google.GData.Client;
using Google.GData.Calendar;

using Gnome.Keyring;

namespace Do.GCalendar
{
	public class DoGCal
	{
		private static string username, password;
		private static CalendarService service;
		private static AtomFeed calendars = new AtomFeed (null,null);
			
		static DoGCal ()
		{	
			SetCredentials ();
			try {
				Connect ();
				UpdateCalendars ();
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		
		public static AtomFeed Calendars {
			get { return calendars; }
		}

		public static void Connect () 
		{
			try {
				service = new CalendarService("alexLauni-gnomeDoGCalPlugin-1");
				service.setUserCredentials(username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				Console.Error.WriteLine ("connect");
			}
		}
		
		public static void UpdateCalendars ()
		{
			if(!Monitor.TryEnter (calendars)) return;
			FeedQuery query = new FeedQuery ();
			query.Uri = new Uri ("http://www.google.com/calendar/feeds/default");
			calendars = service.Query (query);
			Monitor.Exit (calendars);
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
			EventFeed events = service.Query (query) as EventFeed;			
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
