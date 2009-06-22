/* LaunchpadAction.cs
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

using Do.Universe;
using Do.Platform;

using Mono.Addins;

namespace Launchpad
{

	class LaunchpadAction : Act
	{

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Search Launchpad"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Search Launchpad properties."); }
		}

		public override string Icon { 
			get { return "Launchpad.png@" + GetType ().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (LaunchpadItem);}
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			return LaunchpadItems.Items.OfType<Item> ();
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (LaunchpadItem lp in modItems)
				lp.Perform (items.OfType<ITextItem> ());
			
			yield break;
		}
	}

}
