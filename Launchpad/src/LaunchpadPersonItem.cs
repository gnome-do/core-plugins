/* LaunchpadPersonItem.cs
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
	public class LaunchpadUserPageItem : LaunchpadItem
	{
		public LaunchpadUserPageItem() { }
		public string Name { get { return "User Page"; } }
		public string Description { get { return "Go to user's page in Launchpad"; } }
		
		public string Icon
		{ 
			get { return "LaunchpadUser.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(IItem[] items)
		{
			//Project name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open(string.Format("https://launchpad.net/~{0}", (item as ITextItem).Text));
		}
	}

	public class LaunchpadUserSearchItem : LaunchpadItem
	{
		public LaunchpadUserSearchItem() { }
		public string Name { get { return "User Search"; } }
		public string Description { get { return "Search for a user in Launchpad"; } }
		
		public string Icon
		{ 
			get { return "LaunchpadUser.png@" + GetType ().Assembly.FullName; }
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
			Util.Environment.Open(string.Format("https://launchpad.net/people?name={0}&searchfor=peopleonly", string.Join("+", qwords)));
		}
	}

}
