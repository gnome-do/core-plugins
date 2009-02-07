using System;
using Do.Universe;
using System.Collections.Generic;
using System.Linq;

namespace Do.Universe
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
			get { yield return typeof (EmeseneStatusItem);}
		}

		public override bool SupportsItem (Item item)
		{
			return (item is EmeseneStatusItem); 
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{ 
			Emesene.set_status((items.First () as EmeseneStatusItem).GetAbbreviation());
			yield break;
		}
	}
}
