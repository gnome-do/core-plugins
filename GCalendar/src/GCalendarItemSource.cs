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
using System.Threading;
using System.Collections.Generic;
using Mono.Unix;

using Do.Addins;
using Do.Universe;

namespace GCalendar
{	
	public sealed class GCalendarItemSource : IItemSource, IConfigurable
	{
		public string Name {
			get { return Catalog.GetString ("Google Calendars"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Indexes your Google Calendars"); }
		}
		
		public string Icon {
			get { return "calIcon.png@" + GetType ().Assembly.FullName; }
		}

		public IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (GCalendarItem),
				};
			}
		}

		public IEnumerable<IItem> Items
		{
			get { return GCal2.Calendars; }
		}

		public IEnumerable<IItem> ChildrenOfItem (IItem parent)
		{
			return GCal2.EventsForCalendar ((parent as GCalendarItem).Name);
		}

		public void UpdateItems ()
		{
			Thread updateCalendars = new Thread (new ThreadStart (GCal2.UpdateCalendars));
			updateCalendars.IsBackground = true;
			updateCalendars.Start ();
			
			Thread updateEvents = new Thread (new ThreadStart (GCal2.UpdateEvents));
			updateEvents.IsBackground = true;
			updateEvents.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}
