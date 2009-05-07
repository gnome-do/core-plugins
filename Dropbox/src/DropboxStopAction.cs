
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
 
using Do.Universe;
using Do.Platform;


namespace Dropbox
{
	
	
	public class DropboxStopAction : Act
	{
		
		public override string Name {
			get { return "Stop";  }
		}
		
		public override string Description {
			get { return "Stops the dropbox daemon."; }
		}
		
		public override string Icon {
			get { return "dropbox"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IApplicationItem); }
		}
		
		public override bool SupportsItem (Item item) 
		{
			return item.Name == "Dropbox";
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			try {
				Process client = Process.Start ("dropbox stop");
				client.WaitForExit ();
			} catch {
			}
			
			return null;
		}

	}
}
