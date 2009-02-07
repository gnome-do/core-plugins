using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Do.Universe;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace Do.Universe
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
