/* LaunchpadAnswersItem.cs
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
	public class LaunchpadAnswerSearchItem : LaunchpadItem
	{
		public LaunchpadAnswerSearchItem() { }
		public string Name { get { return "Answers Search"; } }
		public string Description { get { return "Search for answers at Launchpad"; } }
		public string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("LaunchpadAnswers.png"); }
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
			Util.Environment.Open("https://answers.launchpad.net/questions/+questions?field.search_text=" + string.Join("+", qwords));
		}
	}

	public class LaunchpadProjectAnswersItem : LaunchpadItem
	{
		public LaunchpadProjectAnswersItem() { }
		public string Name { get { return "Answers"; } }
		public string Description { get { return "Launchpad Answers"; } }

		public string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("LaunchpadAnswers.png"); }
		}

		public bool SupportsItems(IItem[] items)
		{
			//Package name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (IItem item)
		{
			Util.Environment.Open(string.Format("https://answers.launchpad.net/{0}", (item as ITextItem).Text));
		}
	}
}
