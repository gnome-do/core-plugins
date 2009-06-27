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

using Mono.Addins;


namespace RequestTracker
{
	/// <summary>
	/// Given an ITextItem, RequestTrackerAction will construct a URL
	///  and feed it to a web browser
	/// </summary>
	class RTAction : Act, IConfigurable
	{
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Request Tracker"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Display tickets from Request Tracker."); }
		}
		
		public override string Icon	{
			get { return "rt.png@" + GetType ().Assembly.FullName; }
		}
		
		public override bool ModifierItemsOptional {
			get { return false; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (RequestTrackerItem); }
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			if (item is ITextItem)
				return RequestTrackerItems.GetItems ((item as ITextItem).Text);
			return Enumerable.Empty<Item> ();
			
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			RequestTrackerItem rt;
			ITextItem item;
			
			if (items.First () is ITextItem && modItems.First () is RequestTrackerItem) {
				rt = (modItems.First() as RequestTrackerItem);
				item = (items.First() as ITextItem);
				
				yield return new TextItem (GetUrl (rt, item));
			}
			else yield break;
		}
		
		private string GetUrl (RequestTrackerItem tracker, ITextItem ticket)
		{
			if (tracker.URL.Substring (0, 4) == "FAIL") {
				Do.Platform.Services.Notifications.Notify ("Request Tracker", "No trackers are configured. Please use the GNOME Do preferences ");
				throw new UriFormatException ();
			}
			string newtext = Regex.Replace (ticket.Text, @"[^0-9]", "");
			
			if (string.IsNullOrEmpty (newtext)) {
				Do.Platform.Services.Notifications.Notify ("Request Tracker", "No ticket number provided");
				throw new ArgumentNullException ();
			}
			
			string query = HttpUtility.UrlEncode (newtext);
			return FormatUrl (tracker.URL, query);
		}

		private string FormatUrl (string url, string ticket)
		{
			return string.Format (url, ticket);
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new RTPrefs ();
		}
		
	}
	
}
