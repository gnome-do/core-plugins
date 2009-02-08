/* EmeseneChangeStatusAction.cs
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
using Do.Universe;
using System.Collections.Generic;
using System.Linq;

namespace Emesene
{	
	public class EmeseneChangeStatusAction : Act
	{
		
		public EmeseneChangeStatusAction()
		{
		}
		
		public override string Name
		{
			get { return "Change emesene status."; }
		}
		
		public override string Description
		{
			get { return "Change your emesene status."; }
		}
		
		public override string Icon
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (EmeseneStatusItem);}
		}

		public override bool SupportsItem (Item item)
		{
			return (item is EmeseneStatusItem); 
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{ 
			Emesene.set_status((items.First () as EmeseneStatusItem).GetAbbreviation());
			yield break;
		}
	}
}
