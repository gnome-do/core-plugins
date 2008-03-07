/* LaunchpadProjectItem.cs
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
	public class LaunchpadProjectPageItem : LaunchpadItem
	{
		public LaunchpadProjectPageItem() { }
		public string Name { get { return "Project Page"; } }
		public string Description { get { return "Go to project's page in launchpad"; } }
		
		public string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("LaunchpadRegister.png"); }
		}

		public bool SupportsItems(IItem[] items)
		{
			//Project name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open(string.Format("https://launchpad.net/{0}", (item as ITextItem).Text));
		}
	}

}
