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


using Mono.Unix;

namespace Do.Launchpad
{
	public class LaunchpadBlueprintsItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Project Blueprints"); } }
		public override string Description { 
			get { return Catalog.GetString ("Show blueprints for specified project at Launchpad"); }
		}

		public override string Icon
		{ 
			get { return "LaunchpadBlueprints.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(Item[] items)
		{
            if (items == null) { return false; }
			//Package name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (Item item)
		{
			Util.Environment.Open("https://blueprints.launchpad.net/" + (item as ITextItem).Text);
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

		public bool SupportsItems(Item[] items)
		{
			return true;
		}

		public void Perform (Item item)
		{
			Regex spaces = new Regex(@"\s+");
			string query = (item as ITextItem).Text;
			string[] qwords = spaces.Split(query);
			Util.Environment.Open("https://blueprints.launchpad.net/?searchtext=" + string.Join("+", qwords));
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

		public bool SupportsItems(Item[] items)
		{
			return true;
		}

		public void Perform (Item item)
		{
			Util.Environment.Open("https://blueprints.launchpad.net/specs/+new");
		}
	}
}
