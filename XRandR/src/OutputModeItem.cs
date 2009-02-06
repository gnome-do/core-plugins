// OutputModeItem.cs created with MonoDevelop
// User: johannes at 6:13 PM 2/5/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;
using Mono.Unix;

namespace XRandR
{
	public class OutputModeItem:Item,IRunnableItem
	{
		int output_id;
		string name;
		int mode_id;
		public OutputModeItem(int output_id,XRRModeInfo mode)
		{
			this.output_id = output_id;
			this.name = mode.name + " "+mode.dotClock/mode.vTotal/mode.hTotal + "Hz";
			this.mode_id = mode.id;
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
		
		public void Run () {
			foreach(ScreenResources res in External.ScreenResources())
				res.setMode(output_id,mode_id);
		}
	}
}
