
using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Skype
{

	public class SkypeSetStatusAction : Act
	{
		
	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Set Status"); }
	    }
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Set your Skype status"); }
		}
		
		public override string Icon {
			get { return string.Format ("{0}@{1}", "CallStart_128x128.png", typeof (Skype).Assembly.FullName); }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (StatusItem);
			}
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) 
		{
			StatusItem status = (items.First () as StatusItem);
			
			if (Skype.InstanceIsRunning)
				Skype.SetStatus (status);
			yield break;
		}		
	}
}
