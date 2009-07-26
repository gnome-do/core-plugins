/* GCal.cs 
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
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace GCalendar
{
	
	public static class GCal
	{
		static readonly string ConnectionErrorMessage = AddinManager.CurrentLocalizer.GetString ("Failed to connect to GCal service");
		static GCalClient client;
		
		static GCal()
		{
			Preferences = new GCalPreferences ();
			Connect (Preferences.Username, Preferences.Password);
		}

		public static GCalPreferences Preferences { get; private set; }

		public static bool TryConnect (string username, string password)
		{
			GCalClient test;

			try {
				test = new GCalClient (username, password);
				test.UpdateCalendars ();
				Connect (username, password);
			} catch (Exception) {
				Log.Error (ConnectionErrorMessage);
				return false;
			}
			
			return true;
		}

		public static IEnumerable<GCalendarItem> Calendars {
			get { return client.Calendars; }
		}

		public static IEnumerable<GCalendarEventItem> EventsForCalendar (GCalendarItem calendar)
		{
			return client.EventsForCalendar (calendar);
		}

		public static void UpdateCalendars ()
		{
			client.UpdateCalendars ();
		}

		public static GCalendarEventItem NewEvent (GCalendarItem calendar, string data)
		{
			return client.NewEvent (calendar, data);
		}

		public static IEnumerable<GCalendarEventItem> SearchEvents (IEnumerable<GCalendarItem> calendars, string data)
		{
			return client.SearchEvents (calendars, data);
		}

		static void Connect (string username, string password) 
		{
			try {
				client = new GCalClient (username, password);
			} catch (Exception) {
				Log.Error (ConnectionErrorMessage);
			}
		}
	}
}
