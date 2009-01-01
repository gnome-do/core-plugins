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
using System.Linq;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform.Linux;

namespace GCalendar
{	
	public sealed class GCalendarItemSource : ItemSource, IConfigurable
	{
		public GCalendarItemSource ()
		{
		}

		public override string Name {
			get { return Catalog.GetString ("Google Calendars"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Indexes your Google Calendars"); }
		}
		
		public override string Icon {
			get { return "calIcon.png@" + GetType ().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (GCalendarItem); }
		}

		public override IEnumerable<Item> Items
		{
			get { return GCal.Calendars.Cast<Item> (); }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			return GCal.EventsForCalendar (parent as GCalendarItem).Cast<Item> ();
		}

		public override void UpdateItems ()
		{
			Thread thread = new Thread ((ThreadStart) GCal.UpdateCalendars);
			thread.IsBackground = true;
			thread.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}
