/* LaunchpadItemSource.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
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
using System.IO;
using System.Collections.Generic;
using Do.Universe;

namespace Do.Launchpad
{
	public class LaunchpadItemSource : IItemSource
	{
		private List<IItem> lpsections;

		public LaunchpadItemSource ()
		{
			lpsections = new List<IItem> ();
			UpdateItems ();
		}

		public Type[] SupportedItemTypes
		{
			get
			{
				return new Type[] {
					typeof(LaunchpadBlueprintsRegisterItem),
					typeof(LaunchpadBugReportItem),
					typeof(LaunchpadRegisterItem)
				};
			}
		}

		public string Name
		{
			get { return "Launchpad"; }
		}

		public string Description
		{
			get { return "Launchpad Shortcuts"; }
		}

		public string Icon
		{
			get { return "/dev/null"; } //TODO: get icon
		}

		public void UpdateItems ()
		{
			lpsections.Clear();
			lpsections.Add(new LaunchpadBlueprintsRegisterItem());
			lpsections.Add(new LaunchpadBugReportItem());
			lpsections.Add(new LaunchpadRegisterItem());
		}

		public ICollection<IItem> Items {
			get { return (lpsections as ICollection<IItem>); }
		}

		public ICollection<IItem> ChildrenOfItem (IItem item) {
			return null;
		}
	}
}

