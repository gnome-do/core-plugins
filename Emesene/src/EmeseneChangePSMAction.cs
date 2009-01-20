using System;
using Do.Universe;
using System.Collections.Generic;

namespace Emesene
{	
	public class EmeseneChangePSMAction : Act
	{
		public EmeseneChangePSMAction()
		{
		}
		
		public override string Name
		{
			get { return "Change emesene personal message."; }
		}
		
		public override string Description
		{
			get { return "Change your emesene personal message."; }
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
			foreach(Item ps in items)
			{
				if (ps is ITextItem) 
					Emesene.set_psm((ps as ITextItem).Text);
			}
			return null;
		}
	}
}
