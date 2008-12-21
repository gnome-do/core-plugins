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


using Mono.Unix;

namespace Do.Launchpad
{
	public class LaunchpadAnswerSearchItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Answers Search"); } }
		public override string Description { 
			get { return Catalog.GetString ("Search for answers at Launchpad"); } 
		}
		
		public override string Icon
		{ 
			get { return "LaunchpadAnswers.png@" + GetType ().Assembly.FullName; }
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
			Util.Environment.Open("https://answers.launchpad.net/questions/+questions?field.search_text=" + string.Join("+", qwords));
		}
	}

	public class LaunchpadProjectAnswersItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Answers"); } }
		public override string Description { get { return Catalog.GetString ("Launchpad Answers"); } }

		public override string Icon
		{ 
			get { return "LaunchpadAnswers.png@" + GetType ().Assembly.FullName; }
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
			Util.Environment.Open(string.Format("https://answers.launchpad.net/{0}", (item as ITextItem).Text));
		}
	}
}
