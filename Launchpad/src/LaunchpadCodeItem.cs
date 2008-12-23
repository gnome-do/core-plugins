/* LaunchpadCodeItem.cs
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
	/*
	 * Needs a better way to find the main code tree for a project.
	 */
#if false
	public class LaunchpadCodeBrowseItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Code Browse"); } }
		
		public override string Description { 
			get { return Catalog.GetString ("Browse Code For Launchpad Project"); } 
		}

		public override string Icon
		{ 
			get { return "LaunchpadCode.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(Item[] items)
		{
            if (items == null) { return false; }
			//Project name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (Item item)
		{
			Util.Environment.Open(string.Format("https://codebrowse.launchpad.net/~vcs-imports/{0}/main/files", (item as ITextItem).Text));
		}
	}
#endif

	public class LaunchpadCodeOverviewItem : LaunchpadItem
	{
		public override string Name { get { return Catalog.GetString ("Code Overview"); } }
	
		public override string Description { 
			get { return Catalog.GetString ("Browse project code at Launchpad"); }
		}
		
		public override string Icon
		{ 
			get { return "LaunchpadCode.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(Item[] items)
		{
            if (items == null) { return false; }
			//Project name can't have a space
			Regex numbers = new Regex(@"\s+");
			return !numbers.IsMatch((items[0] as ITextItem).Text);
		}

		public void Perform (Item item)
		{
			Util.Environment.Open(string.Format("https://code.launchpad.net/{0}", (item as ITextItem).Text));
		}
	}
}
