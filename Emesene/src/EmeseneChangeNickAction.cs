using System;
using System.Collections.Generic;
using System.Linq;

namespace Do.Universe
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
			get { yield return typeof (ITextItem); }
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ITextItem);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Emesene.set_nick((items.First () as ITextItem).Text);
			yield break;
		}
	}
}
