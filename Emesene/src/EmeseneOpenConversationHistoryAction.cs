using System;
using System.Collections.Generic;
using Do.Universe;
using System.Linq;

namespace Do.Universe
{
	public class EmeseneOpenConversationHistoryAction : Act
	{
		
		public EmeseneOpenConversationHistoryAction()
		{
		}
		
		public override string Name
		{
			get { return "Open conversation history"; }
		}
		
		public override string Description
		{
			get { return "Opens the emesene conversation log."; }
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
			//Emesene.get_conversation_history((items.First() as ContactItem)["email"]);
			yield break;
		}
	}
}
