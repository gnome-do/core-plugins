/* LaunchpadBugsItem.cs
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
	public class LaunchpadBugNumberItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Bug Number"); } }
		
		public override string Description { 
			get { return Catalog.GetString ("Find bug by number"); }
		}
		
		public override string Icon { 
			get { return "LaunchpadBugs.png@" + GetType ().Assembly.FullName; }
		}

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			// Numbers only.
			Regex numbers = new Regex (@"^\d+$");
			return items.All (item => numbers.IsMatch (item.Text));
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			foreach (ITextItem item in items) {
				string url = "https://bugs.launchpad.net/bugs/" + item.Text;
				Services.Environment.OpenUrl (url);
			}
		}
	}

	public class LaunchpadBugReportItem : LaunchpadItem
	{
		public LaunchpadBugReportItem() { }
		public override string Name { get { return Catalog.GetString ("Bug Report"); } }
		public override string Description { 
			get { return Catalog.GetString ("Report a bug at Launchpad"); }
		}
		
		public override string Icon { 
			get { return "LaunchpadBugs.png@" + GetType ().Assembly.FullName; }
		}

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			foreach (ITextItem item in items) {
				string url = "https://bugs.launchpad.net/bugs/+filebug/" + item.Text;
				Services.Environment.OpenUrl (url);
			}
		}
	}

	public class LaunchpadPackageBugsItem : LaunchpadItem
	{
		public LaunchpadPackageBugsItem ()
		{
		}
		
		public override string Name {
			get { return Catalog.GetString ("Project Bugs"); }
		}
		
		public override string Description { 
			get { return Catalog.GetString ("Show open bugs in a project at Launchpad"); } 
		}
		
		public override string Icon { 
			get { return "LaunchpadBugs.png@" + GetType ().Assembly.FullName; }
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
				string url = "https://bugs.launchpad.net/" + item.Text;
				Services.Environment.OpenUrl (url);
			}
		}
	}

	public class LaunchpadBugSearchItem : LaunchpadItem
	{
		public LaunchpadBugSearchItem() { }
		public override string Name { get { return "Bug Search"; } }
		public override string Description { get { return "Search for bugs at Launchpad"; } }
		public override string Icon
		{ 
			get { return "LaunchpadBugs.png@" + GetType ().Assembly.FullName; }
		}

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			foreach (ITextItem item in items) {
				string query = item.Text.Replace (" ", "+");	
				string url = "https://bugs.launchpad.net/bugs/+bugs?field.searchtext=" + query;
				Services.Environment.OpenUrl (url);
			}
		}
	}
}
