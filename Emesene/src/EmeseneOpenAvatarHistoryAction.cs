using System;
using System.Linq;
using System.Collections.Generic;
using Do.Universe;

namespace Do.Universe
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
