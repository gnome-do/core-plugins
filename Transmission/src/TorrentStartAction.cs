
using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Transmission {
	
	public class TorrentStartAction: Act {
		
		public TorrentStartAction() {
		}
		
	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Start"); }
	    }
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Start downloading torrent"); }
		}
		
		public override string Icon {
			get { return "gtk-media-play"; }
		}
			
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (TorrentItem); }
		}
    
		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems) {
			TorrentItem item = items.First() as TorrentItem;
			
			TransmissionAPI api = TransmissionPlugin.getTransmission();
			api.StartTorrents(new string[] {item.HashString});
			
			return null;
		}
	}
}
