//  SkypeChatAction.cs
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
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Skype
{

	public class SkypeChatAction : Act
	{

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Chat"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Chat with a buddy using Skype"); }
		}
		
		public override string Icon {
			get { return  string.Format ("{0}@{1}", "Message.png", typeof (Skype).Assembly.FullName); }
		}
		
		public override bool SupportsItem (Item item) {
			if (item is ContactItem)
				return null != (item as ContactItem) ["handle.skype"];
			return true;
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ContactItem);
				yield return typeof (SkypeContactDetailItem);
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
			Item user = items.First ();
			string message = "";
			
			if (modItems.Any ())
				message = (modItems.First() as ITextItem).Text;
			
			if (user is ContactItem) {
				Skype.ChatWith ((user as ContactItem) ["handle.skype"], message);
				yield break;
			} else if (user is SkypeContactDetailItem) {
				Skype.ChatWith ((user as SkypeContactDetailItem).Handle, message);
				yield break;
			}
			yield break;
		}
	}
}
