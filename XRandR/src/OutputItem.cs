// OutputItem.cs created with MonoDevelop
// User: johannes at 4:44 PM 2/4/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;
using Mono.Unix;

namespace XRandR
{
	public class OutputItem : Item
	{
		String name;
		public OutputItem(String name)
		{
			this.name = name;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Set your resolution"); }
		}
		
		public override string Icon {
			get { return "system-config-display"; }
		}
	}
}
