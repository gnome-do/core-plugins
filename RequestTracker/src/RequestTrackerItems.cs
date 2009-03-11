/* RequestTrackerItems.cs
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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Do.Platform;

namespace RequestTracker
{
	
	public static class RequestTrackerItems
	{
		public static ICollection<RequestTrackerItem> Items = new Collection<RequestTrackerItem>();
		
		static RequestTrackerItems () {
			RTPreferences prefs = new RTPreferences ();

			if (string.IsNullOrEmpty (prefs.URLs)) {
				RequestTrackerItem defitem = new RequestTrackerItem (
					     "No Trackers Configured",
					     "Please use the GNOME Do Preferences to add some RT sites",
					     "FAIL{0}");
				Items.Add (defitem);
			} else {
				string[] urlbits = prefs.URLs.Split('|');
				for (int i = 0; i < urlbits.Length; i++) {
					string name = urlbits[i];
					string uri = urlbits[++i];
					Uri url;
					try {
						url = new System.Uri(uri);
					} catch (System.UriFormatException) {
						continue;
					}
					string description = url.Scheme + url.Host;
					
					Items.Add (new RequestTrackerItem (name, description, url.ToString ()));
				}
			}
		}
	}
}
