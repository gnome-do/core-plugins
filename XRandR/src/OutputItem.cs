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
		string name;
		int id;
		public OutputItem(int id,XRROutputInfo output)
		{
			this.name = output.name;
			this.id = id;
		}
		
		public override string Name {
			get { return name; }
		}
		public int Id {
			get { return id; }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Set your resolution"); }
		}
		
		public override string Icon {
			get { return "video-display"; }
		}
	}
}
