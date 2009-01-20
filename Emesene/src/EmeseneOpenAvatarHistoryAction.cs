using System;
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
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem) 
			{
				foreach (string detail in (item as ContactItem).Details) \
				{
					if (detail.StartsWith ("prpl-")) return false;
				}
				return true;
			} return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach(Item buddy in items)
			{
				ContactItem contact = buddy as ContactItem;
				Emesene.get_avatar_history(contact["email"]);
			}
			return null;
		}
	}
}
