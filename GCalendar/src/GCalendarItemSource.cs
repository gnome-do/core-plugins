/* GCalendarItemSource.cs
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
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;

using Do.Addins;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;

namespace Do.GCalendar
{	
	public sealed class GCalendarItemSource : IItemSource
	{
		List<IItem> items;

		public GCalendarItemSource ()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}

		public string Name { get { return "Google Calendars"; } }
		public string Description { get { return "Indexes your Google Calendars"; } }
		public string Icon { get { return "date"; } }

		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (GCalendarItem),
					typeof (GCalendarEventItem),
				};
			}
		}

		public ICollection<IItem> Items
		{
			get { return items; }
		}

		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			GCalendarItem calItem = parent as GCalendarItem;
			List<IItem> children = new List<IItem> ();
			string eventUrl, eventDesc, start;
			EventFeed events;
			try {
				events = DoGCal.GetEvents (calItem.URL);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return null;
			}
			foreach (EventEntry entry in events.Entries) {
			    eventUrl = entry.AlternateUri.Content;
			    eventDesc = entry.Content.Content;
			    if (entry.Times.Count > 0) {
			        start = entry.Times[0].StartTime.ToString ();
			        start = start.Substring (0,start.IndexOf (' '));
			        eventDesc = start + " - " + eventDesc;
                }
			    children.Add (new GCalendarEventItem (entry.Title.Text, eventUrl,
			            eventDesc));
            }
			return children;
		}

		public void UpdateItems ()
		{	
			try {
				Thread updateRunner = new Thread (new ThreadStart (DoGCal.UpdateCalendars));
				updateRunner.Start ();
				items = DoGCal.Calendars;
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return;
			}
		}
	}
}
