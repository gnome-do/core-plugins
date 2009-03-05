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
using Do.Universe;
using Do.Universe.Common;


namespace RequestTracker
{
	/// <summary>
	/// Given an ITextItem, RequestTrackerAction will construct a URL
	///  and feed it to a web browser
	/// </summary>
	public class RTAction : Act
	{

		// String indicating the tags surronding the pertinent info
		const string rturl = "https://rt.admin.canonical.com/Ticket/Display.html?id=";

		public RTAction ()
		{
		}
		
		public override string Name {
			get { return "Request Tracker"; }
		}
		
		public override string Description
		{
			get { return "Display tickets from Request Tracker."; }
		}
		
		public override string Icon
		{
			get { return "rt.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			string word;

			word = null;
			if (item is ITextItem) {
				word = (item as ITextItem).Text;
			}
			return !string.IsNullOrEmpty (word);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			string expression, url;
			expression = (items.First () as ITextItem).Text;

			foreach (ITextItem item in items) {
				url = RTURL (item.Text);
				string query = HttpUtility.UrlEncode (url);
			    Services.Environment.OpenUrl (FormatUrl (url, query));
			}
			
			yield break;
		}

		protected virtual string FormatUrl (string url, string query)
		{
			return string.Format (url, query);
		}
		
		string RTURL (string e)
		{
			return rturl + (e ?? "");
		}
	}
}
