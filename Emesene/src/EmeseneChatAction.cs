/* EmeseneChatAction.cs
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
using System.Threading;
using Do.Universe;
#if USE_DBUS_SHARP
using DBus;
#else
using NDesk.DBus;
#endif
using org.freedesktop.DBus;

namespace Emesene
{
	public class EmeseneChatAction : Act
	{
		public EmeseneChatAction ()
		{
		}
		
		public override string Name
		{
			get { return "Chat"; }
		}
		
		public override string Description
		{
			get { return "Send an instant message to a friend."; }
		}
		
		public override string Icon
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {yield return typeof (ContactItem);}
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
			Emesene.openChatWith((items.First() as ContactItem)["email"]);
			yield break;
		}
	}
}
