/* LaunchpadBlueprintsItem.cs
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
using System.Text.RegularExpressions;

using Do.Universe;
using Do.Platform;

using Mono.Unix;

namespace Do.Launchpad
{
	public class LaunchpadBlueprintsItem : LaunchpadItem
	{
		public override string Name {
			get { return Catalog.GetString ("Project Blueprints"); }
		}
		
		public override string Description { 
			get { return Catalog.GetString ("Show blueprints for specified project at Launchpad"); }
		}

		public override string Icon { 
			get { return "LaunchpadBlueprints.png@" + GetType ().Assembly.FullName; }
		}

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			// Package name can't have a space
			Regex numbers = new Regex (@"\s+");
			return !items.Any (item => numbers.IsMatch (item.Text));
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			foreach (ITextItem item in items) {
				string url = "https://blueprints.launchpad.net/" + item.Text;
				Services.Environment.OpenUrl (url);
			}
		}
	}

	public class LaunchpadBlueprintSearchItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Blueprint Search"); } }
		public override string Description {
			get { return Catalog.GetString ("Search for blueprints at Launchpad"); }
		}

		public override string Icon
		{ 
			get { return "LaunchpadBlueprints.png@" + GetType ().Assembly.FullName; }
		}

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			Regex spaces = new Regex(@"\s+");
			foreach (ITextItem item in items) {
				string query = item.Text.Replace (" ", "+");
				Services.Environment.OpenUrl ("https://blueprints.launchpad.net/?searchtext=" + query);
			}
		}
	}

	public class LaunchpadBlueprintsRegisterItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Register Blueprints"); } }
		
		public override string Description {
			get { return Catalog.GetString ("Register a blueprint at Launchpad"); }
		}
		
		public override string Icon
		{ 
			get { return "LaunchpadBlueprints.png@" + GetType ().Assembly.FullName; }
		}

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			Services.Environment.OpenUrl ("https://blueprints.launchpad.net/specs/+new");
		}
	}
}
