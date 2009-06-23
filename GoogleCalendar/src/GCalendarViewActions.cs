/* GCalendarViewActions.cs 
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
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace GCalendar
{
	
	public class ViewEventAction : Act
	{
		
		public ViewEventAction ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("View Event"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Open event in browser"); }
		}

		public override string Icon {
			get { return "internet-web-browser"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (GCalendarEventItem); }
		}

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			items.Cast<GCalendarEventItem> ().ForEach (item => Services.Environment.OpenUrl (item.Url));
			
			return Enumerable.Empty<Item> ();
		}
	}

	public class ViewCalendarAction : Act
	{
		
		public ViewCalendarAction ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("View Calendar"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Open calendar in browser"); }
		}

		public override string Icon {
			get { return "internet-web-browser"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (GCalendarItem); }
		}

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			items.Cast<GCalendarItem> ().ForEach (item => Services.Environment.OpenUrl (item.Url));
			
			return Enumerable.Empty<Item> ();
		}
	}
}
