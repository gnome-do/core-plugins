/* RequestTrackerItem.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace RequestTracker
{
	
	
	public class RequestTrackerItem : Item
	{
		string name, description;
		
		public RequestTrackerItem (string name, string description, string url)
		{
			this.name = AddinManager.CurrentLocalizer.GetString (name);
			this.description = AddinManager.CurrentLocalizer.GetString (description);
			this.URL = url;
		}
		
		public string URL { get; private set; }
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return description; }
		}

		public override string Icon { 
			get { return "rt.png@" + GetType ().Assembly.FullName; }
		}
	}
}
