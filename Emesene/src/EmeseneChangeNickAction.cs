// EmeseneChangeNickAction.cs created with MonoDevelop
// User: luis at 08:28 a 19/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;
using System.Collections.Generic;

namespace Emesene
{	
	public class EmeseneChangeNickAction : Act
	{
		
		public EmeseneChangeNickAction()
		{
		}
		
		public override string Name
		{
			get { return "Change emesene nickname"; }
		}
		
		public override string Description
		{
			get { return "Change your emesene nickname."; }
		}
		
		public override string Icon
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) 
				return true;
			return false;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach(Item nick in items){
				if (nick is ITextItem) 
				{
					Emesene.set_nick((nick as ITextItem).Text);
				}
			}
			return null;
		}
	}
}
