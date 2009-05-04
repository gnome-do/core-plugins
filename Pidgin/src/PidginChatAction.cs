//  PidginChatAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace PidginPlugin
{

	public class PidginChatAction : Act
	{

		public PidginChatAction ()
		{
		}
		
		public override string Name {
			get { return Catalog.GetString ("Chat"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Send an instant message to a friend."); }
		}
		
		public override string Icon {
			get { return Pidgin.ChatIcon; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ContactItem);
				yield return typeof (PidginHandleContactDetailItem);
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem) {
				ContactItem contact = item as ContactItem;
				//return (item as ContactItem).Details.Any (d => d.StartsWith ("prpl-"));
				return contact.Details.Where (d => Pidgin.BuddyIsOnline (contact[d])).Any ();
			}
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			List<string> names = new List<string> ();
			string message = "";
			
			if (modItems.Any ())
				message = (modItems.First () as ITextItem).Text;
			
			foreach (Item item in items) {
				if (item is ContactItem) {
					// Just grab the first protocol we see.
					ContactItem contact = item as ContactItem;
					foreach (string detail in contact.Details) {
						if (!detail.StartsWith ("prpl-"))
							continue;
						//if this buddy is online, add and break
						if (Pidgin.BuddyIsOnline (contact[detail])) {
							names.Add (contact[detail]);
							break;
						}
					}
				} else if (item is PidginHandleContactDetailItem) {
					names.Add ((item as PidginHandleContactDetailItem).Value);
				}
			}
	
			
			if (names.Count > 0) {
				Services.Application.RunOnThread (() => {
					Pidgin.StartIfNeccessary ();
					Services.Application.RunOnMainThread (() => {
						foreach (string name in names) {
							if (!string.IsNullOrEmpty (message))
								Pidgin.OpenConversationWithBuddy (name, message);
							else
								Pidgin.OpenConversationWithBuddy (name);
						}
					});
				});
			}
			yield break;
		}
		
	}
}
