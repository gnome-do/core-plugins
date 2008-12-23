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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Do.Universe;


using Mono.Unix;

namespace Do.Launchpad
{
	public class LaunchpadRegisterItem : LaunchpadItem
	{
		public override string Name { 
			get { return Catalog.GetString ("Register Project"); }
		}
		
		public override string Description { 
			get { return Catalog.GetString ("Register a new project at Launchpad"); }
		}

		public override string Icon { 
			get { return "LaunchpadRegister.png@" + GetType ().Assembly.FullName; }
		}

		public bool SupportsItems(Item[] items)
		{
			return true;
		}

		public override void Perform (IEnumerable<ITextItem> items)
		{
			Util.Environment.Open("https://launchpad.net/projects/+new");
		}
	}
}
