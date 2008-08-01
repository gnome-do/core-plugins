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
		private static AtomFeed calendar_feed;
		private static object cal_lock;
		
		const string FeedUri = "http://www.google.com/calendar/feeds/default";
		private static string gAppName = "alexLauni-gnomeDoGCalPlugin-1.2";
		
		static GCal2 ()
		{
			string username, password;
			//Google works over SSL, we need accept the cert.
			System.Net.ServicePointManager.CertificatePolicy = new CertHandler ();
			
			calendars = new List<IItem> ();
			cal_lock = new object ();
			
			Configuration.GetAccountData (out username, out password,
				typeof (Configuration));
			Connect (username, password);
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
		
		public static void UpdateCalendars ()
		{
			//if  (calendars.Count > 0) return;
			try {
				Thread updateCals = new Thread (new ThreadStart (UpdateCalFeed));
				updateCals.IsBackground = true;
				updateCals.Start ();
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
			
			lock (cal_lock) {
				foreach (AtomEntry calendar in calendar_feed.Entries) {
					string cal_url = calendar.Id.Uri.Content.Replace ("/default","") + "/private/full";
					calendars.Add (new GCalendarItem (calendar.Title.Text, cal_url));
				}
			}
		}
		
		public static void UpdateCalFeed ()
		{			
			FeedQuery query = new FeedQuery ();
			query.Uri = new Uri (FeedUri);
			try {
				calendar_feed = service.Query (query);
			} catch (Exception e) {
				calendar_feed = null;
				Console.Error.WriteLine (e.Message);
			}
		}
	}
}
