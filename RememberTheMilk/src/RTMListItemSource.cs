/* RTMListItemSource.cs
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
using System.Collections.Generic;
using System.Threading;

using Do.Platform.Linux;
using Do.Universe;

namespace RememberTheMilk
{	
	public class RTMListItemSource : ItemSource, IConfigurable
	{		
		public override string Name {
			get { return "Remember The Milk Lists"; }
		}
		
		public override string Description {
			get { return "Task Lists in your Remember The Milk account."; }
		}
		
		public override string Icon {
			get { return "rtm.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (RTMListItem),
				};
			}
		}
		
		public override IEnumerable<Item> Items
		{
			get { return RTM.Lists; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			return RTM.TasksForList ((parent as RTMListItem).Id);
		}

		public override void UpdateItems ()
		{
			Thread updateLists = new Thread (new ThreadStart (RTM.UpdateLists));
			updateLists.IsBackground = true;
			updateLists.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}
