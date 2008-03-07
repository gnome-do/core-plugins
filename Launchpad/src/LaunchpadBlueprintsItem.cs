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
using System.Text.RegularExpressions;
using Do.Universe;

using Do.Addins;

namespace Do.Launchpad
{
	public class LaunchpadBlueprintsItem : LaunchpadItem
	{
		public LaunchpadBlueprintsItem() { }
		public string Name { get { return "Project Blueprints"; } }
		public string Description { get { return "Show blueprints for specified project at Launchpad"; } }

		public string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("LaunchpadBlueprints.png"); }
		}

		public bool SupportsItems(IItem[] items)
		{
			//Package name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open("https://blueprints.launchpad.net/" + (item as ITextItem).Text);
		}
	}

	public class LaunchpadBlueprintSearchItem : LaunchpadItem
	{
		public LaunchpadBlueprintSearchItem() { }
		public string Name { get { return "Blueprint Search"; } }
		public string Description { get { return "Search for blueprints at Launchpad"; } }

		public string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("LaunchpadBlueprints.png"); }
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
			Util.Environment.Open("https://blueprints.launchpad.net/?searchtext=" + string.Join("+", qwords));
		}
	}

	public class LaunchpadBlueprintsRegisterItem : LaunchpadItem
	{
		public LaunchpadBlueprintsRegisterItem() { }
		public string Name { get { return "Register Blueprints"; } }
		public string Description { get { return "Register a blueprint at Launchpad"; } }
		public string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("LaunchpadBlueprints.png"); }
		}

		public bool SupportsItems(IItem[] items)
		{
			return true;
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open("https://blueprints.launchpad.net/specs/+new");
		}
	}
}
