/* LaunchpadTranslationsItem.cs
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


using Mono.Unix;

namespace Do.Launchpad
{
	public class LaunchpadTranslationSearchItem : LaunchpadItem
	{
		public LaunchpadTranslationSearchItem() { }
		public override string Name { get { return Catalog.GetString ("Translation Search"); } }
		public override string Description { 
			get { return Catalog.GetString ("Search for Translations in Launchpad"); } 
		}

		public override string Icon
		{ 
			get { return "LaunchpadTranslations.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(Item[] items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			Regex spaces = new Regex(@"\s+");
			string query = (item as ITextItem).Text;
			string[] qwords = spaces.Split(query);

			Util.Environment.Open ("https://translations.launchpad.net/projects/?text=" + string.Join("+", qwords));	
		}
	}

	public class LaunchpadTranslationReleaseItem : LaunchpadItem
	{
		public LaunchpadTranslationReleaseItem() { }
		public override string Name { get { return Catalog.GetString ("Release Translations"); } }
		public override string Description { 
			get { return Catalog.GetString ("Translations for Ubuntu Release Name"); }
		}

		public override string Icon
		{ 
			get { return "LaunchpadTranslations.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(Item[] items)
		{
            if (items == null) { return false; }
			//Package name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public override void Perform (IEnumerable<ITextItem> items))
		{
			Util.Environment.Open(string.Format(
						"https://translations.lauchpad.net/ubuntu/{0}/+translations",
						(item as ITextItem).Text));
		}
	}
}
