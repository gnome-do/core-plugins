/* RequestTracker.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using Do.Platform;
using Do.Platform.Linux;
using Do.Universe;
using Do.Universe.Common;

using Mono.Unix;


namespace RequestTracker
{
	/// <summary>
	/// Given an ITextItem, RequestTrackerAction will construct a URL
	///  and feed it to a web browser
	/// </summary>
	class RTAction : Act, IConfigurable
	{
		
		public override string Name {
			get { return Catalog.GetString ("Request Tracker"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Display tickets from Request Tracker."); }
		}
		
		public override string Icon	{
			get { return "rt.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (RequestTrackerItem);}
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			RequestTrackerItems items = new RequestTrackerItems ();
			return items.Items.OfType<Item> ();
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (RequestTrackerItem rt in modItems)
				rt.Perform (items.OfType<ITextItem> ());
			
			yield break;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new RTPrefs ();
		}
		
	}
	
}
