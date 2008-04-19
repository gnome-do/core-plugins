/* LaunchpadRegisterItem.cs
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
	public class LaunchpadRegisterItem : LaunchpadItem
	{
		public LaunchpadRegisterItem() { }
		public string Name { get { return "Register Project"; } }
		public string Description { get { return "Register a new project at Launchpad"; } }

		public string Icon
		{ 
			get { return "LaunchpadRegister.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(IItem[] items)
		{
			return true;
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open("https://launchpad.net/projects/+new");
		}
	}
}
