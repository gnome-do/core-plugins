//  EmpathyChatAction.cs
//
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//
//  Copyright © 2010
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Mono.Addins;
using Do.Platform;
using Do.Universe;

namespace EmpathyPlugin
{
	public class EmpathyChatAction : Act
	{
		public EmpathyChatAction ()
		{
		}

		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Chat"); }
		}

		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Send an instant message to a friend."); }
		}

		public override string Icon
		{
			get { return EmpathyPlugin.ChatIcon; }
		}

		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				yield return typeof (ContactItem);
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem) {
				ContactItem contact = item as ContactItem;

				return contact.Details.Contains("is-empathy");
			}
			return true;
		}

		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get { yield return typeof (ITextItem); }
		}

		public override bool ModifierItemsOptional
		{
			get { return true; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string message = "";

			if (modItems.Any ())
				message = (modItems.First () as ITextItem).Text;

			foreach (Item item in items) {
				if (item is ContactItem) {
					ContactItem contactItem = item as ContactItem;
					string contactId = contactItem["email"];

					if (!string.IsNullOrEmpty (message))
						EmpathyPlugin.OpenConversationWithBuddy (contactId, message);
					else
						EmpathyPlugin.OpenConversationWithBuddy (contactId);
				}
			}
			yield break;
		}
	}
}
