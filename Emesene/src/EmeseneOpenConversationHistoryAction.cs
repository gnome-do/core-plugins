// EmeseneOpenConversationHistoryAction.cs created with MonoDevelop
// User: luis at 06:09 p 21/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
using Do.Universe;

namespace Emesene
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
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem) {
				foreach (string detail in (item as ContactItem).Details) {
					if (detail.StartsWith ("prpl-")) return false;
				}
				return true;
			} return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach(Item buddy in items){
				ContactItem contact = buddy as ContactItem;
				Emesene.get_conversation_history(contact["email"]);
			}
			return null;
		}
	}
}
