/* EmeseneOpenAvatarHistoryAction.cs
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
using System.Collections.Generic;
using Do.Universe;

namespace Emesene
{	
	public class EmeseneOpenAvatarHistoryAction : Act
	{
		
		public EmeseneOpenAvatarHistoryAction()
		{
		}
		
		public override string Name
		{
			get { return "Open avatar history"; }
		}
		
		public override string Description
		{
			get { return "Opens the avatar history for the contact."; }
		}
		
		public override string Icon
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (ContactItem);}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem) 
			{
				foreach (string detail in (item as ContactItem).Details)
				{
					if (detail.StartsWith ("prpl-")) return false;
				}
				return true;
			} return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Emesene.get_avatar_history((items.First() as ContactItem)["email"]);	
			yield break;
		}
	}
}
