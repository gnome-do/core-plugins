// EmeseneChangeStatusAction.cs created with MonoDevelop
// User: luis at 11:54 a 21/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;
using System.Collections.Generic;

namespace Emesene
{
	
	
	public class EmeseneChangeStatusAction : Act
	{
		
		public EmeseneChangeStatusAction()
		{
		}
		
		public override string Name
		{
			get { return "Change emesene status."; }
		}
		
		public override string Description
		{
			get { return "Change your emesene status."; }
		}
		
		public override string Icon
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (EmeseneStatusItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is EmeseneStatusItem) 
				return true;
			return false;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach(Item status in items){
				if (status is EmeseneStatusItem) 
				{
					Emesene.set_status((status as EmeseneStatusItem).GetAbbreviation());
				}
			}
			return null;
		}
	}
}
