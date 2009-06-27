//  RecentConversationsActions.cs
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
// 

using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace GMail
{
	
	public class RecentConversationsActions : Act
	{

		const string url = "https://mail.google.com/mail/?shva=1#search/from:({0})+OR+to:({0})";
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("View recent conversations"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("View recent emails and chat logs with a friend"); }
		}

		public override string Icon {
			get { return "internet-group-chat"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ContactItem);
				yield return typeof (IContactDetailItem);
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem)
				return !string.IsNullOrEmpty ((item as ContactItem).AnEmailAddress);
			else if (item is IContactDetailItem)
				return (item as IContactDetailItem).Key.Contains ("email");

			return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Do.Universe.Item> items, IEnumerable<Do.Universe.Item> modItems)
		{
			foreach (Item item in items) {
				string email = "";
				
				if (item is ContactItem)
					email = ((ContactItem) item).AnEmailAddress;
				else if (item is IContactDetailItem)
					email = ((IContactDetailItem) item).Value;

				if (!string.IsNullOrEmpty (email))
					Services.Environment.OpenUrl (string.Format (url, email));

				yield break;
			}
		}
	}
}
