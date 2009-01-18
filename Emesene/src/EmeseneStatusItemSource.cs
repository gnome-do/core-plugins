// EmeseneStatusItemSource.cs created with MonoDevelop
// User: luis at 12:33 p 21/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;
using System.Collections.Generic;

namespace Emesene
{
	
	
	public class EmeseneStatusItemSource : ItemSource
	{
		
		public EmeseneStatusItemSource()
		{
		}

		public override void UpdateItems ()
		{
			return;
		}
		
		public override string Name { get { return "Emesene Status"; } }
		public override string Description { get { return "Avaliable emesene status."; }}
		public override string Icon {get { return "emesene"; } }
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get {
				return new Type[] {
					typeof (EmeseneStatusItem)
				};
			}
		}
		
		public override IEnumerable<Item> Items 
		{
			get { return Emesene.status; }
		}
	}
}
