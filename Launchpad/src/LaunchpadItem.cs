/* LaunchpadItem.cs
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

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Launchpad
{
	
	/// <summary>
	/// LaunchpadItems are used as modifier items to LaunchpadAction, and they
	/// are responsible for telling LaunchpadAction whether they support a
	/// given Item, as well as implementing the actual action on a given Item.
	///
	/// They are meant to behave just like Actions, but they need to be Items
	/// to be listed in the right-hand box.
	/// </summary>
	public class LaunchpadItem : Item
	{
		string name, description, icon_file, url;

		public LaunchpadItem (string name, string description, string iconFile, string url)
		{
			this.name = Catalog.GetString (name);
			this.description = Catalog.GetString (description);
			this.icon_file = iconFile;
			this.url = url;
		}

		public override string Name {
			get { return name; }
		}
		
		public override string Description { 
			get { return description; } 
		}

		public override string Icon { 
			get { return icon_file + "@" + GetType ().Assembly.FullName; }
		}
		
		public void Perform (IEnumerable<ITextItem> items)
		{
			foreach (ITextItem item in items)
				Perform (item);
		}

		public virtual void Perform (ITextItem item)
		{
			string query = item.Text.Replace (" ",  "+");
			Services.Environment.OpenUrl (FormatUrl (url, query));
		}

		protected virtual string FormatUrl (string url, string query)
		{
			return string.Format (url, query);
		}
	}
}
