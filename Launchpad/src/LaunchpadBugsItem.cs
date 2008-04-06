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
using System.Text.RegularExpressions;
using Do.Universe;

using Do.Addins;

namespace Do.Launchpad
{
	public class LaunchpadBugNumberItem : LaunchpadItem
	{
		public LaunchpadBugNumberItem() { }
		public string Name { get { return "Bug Number"; } }
		public string Description { get { return "Find bug by number"; } }
		
		public string Icon
		{ 
			get { return "LaunchpadBugs.png@Launchpad"; }
		}

		public bool SupportsItems(IItem[] items)
		{
			//Numbers only.
			Regex numbers = new Regex(@"^\d+$");
			return numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open(string.Format("https://bugs.launchpad.net/ubuntu/+bug/{0}", (item as ITextItem).Text));
		}
	}

	public class LaunchpadBugReportItem : LaunchpadItem
	{
		public LaunchpadBugReportItem() { }
		public string Name { get { return "Bug Report"; } }
		public string Description { get { return "Report an Ubuntu bug at Launchpad"; } }
		
		public string Icon
		{ 
			get { return "LaunchpadBugs.png@Launchpad"; }
		}

		public bool SupportsItems(IItem[] items)
		{
			return true;
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open("https://launchpad.net/ubuntu/+filebug");
		}
	}

	public class LaunchpadPackageBugsItem : LaunchpadItem
	{
		public LaunchpadPackageBugsItem() { }
		public string Name { get { return "Project Bugs"; } }
		public string Description { get { return "Show open bugs in a project at Launchpad"; } }
		
		public string Icon
		{ 
			get { return "LaunchpadBugs.png@Launchpad"; }
		}

		public bool SupportsItems(IItem[] items)
		{
			//Package name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open(string.Format("https://bugs.launchpad.net/{0}", (item as ITextItem).Text));
		}
	}

	public class LaunchpadBugSearchItem : LaunchpadItem
	{
		public LaunchpadBugSearchItem() { }
		public string Name { get { return "Ubuntu Bug Search"; } }
		public string Description { get { return "Search for Ubuntu bugs at Launchpad"; } }
		public string Icon
		{ 
			get { return "LaunchpadBugs.png@Launchpad"; }
		}

		public bool SupportsItems(IItem[] items)
		{
			return true;
		}

		public void Perform (IItem item)
		{
			Regex spaces = new Regex(@"\s+");
			string query = (item as ITextItem).Text;
			string[] qwords = spaces.Split(query);
			Util.Environment.Open("https://bugs.launchpad.net/ubuntu/+bugs?field.searchtext=" + string.Join("+", qwords));
		}
	}
}
