/* EmeseneChangePSMAction.cs
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
using System.Linq;
using Do.Universe;
using System.Collections.Generic;

namespace Emesene
{	
	public class EmeseneChangePSMAction : Act
	{
		public EmeseneChangePSMAction()
		{
		}
		
		public override string Name
		{
			get { return "Change emesene personal message."; }
		}
		
		public override string Description
		{
			get { return "Change your emesene personal message."; }
		}
		
		public override string Icon
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (ITextItem); }
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ITextItem);
		}
		
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{				
			Emesene.set_psm((items.First () as ITextItem).Text);
			yield break;
		}
	}
}
