using System;
using System.Linq;
using Do.Universe;
using System.Collections.Generic;

namespace Do.Universe
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
			get { yield return typeof (ITextItem); }
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ITextItem);
		}
		
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{				
			Emesene.set_psm((items.First () as ITextItem).Text);
			yield break;
		}
	}
}
