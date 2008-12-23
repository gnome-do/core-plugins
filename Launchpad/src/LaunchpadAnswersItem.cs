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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Universe;
using Do.Platform;


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

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			Regex spaces = new Regex (@"\s+");
			foreach (ITextItem item in items) {
				string query = item.Text.Replace (" ", "+");
				string url = "https://answers.launchpad.net/questions/+questions?field.search_text=" + query;
				Services.Environment.OpenUrl (url);
			}
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

		public override bool SupportsItems (IEnumerable<ITextItem> items)
		{
			//Package name can't have a space
			Regex numbers = new Regex (@"\s+");
			return !items.Any (item => numbers.IsMatch (item.Text));
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			foreach (ITextItem item in items) {
				string url = "https://answers.launchpad.net/" + item.Text;
				Services.Environment.OpenUrl (url);
			}
		}
	}
}
