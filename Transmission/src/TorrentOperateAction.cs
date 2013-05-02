using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace Transmission {

	public class TorrentOperateAction: Act {

	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Operate on files"); }
	    }

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Operate on downloaded file"); }
		}

		public override string Icon {
			get { return "file"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITorrentEntry); }
		}

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems) {
			foreach (Item item in items) {
				ITorrentEntry entry = (ITorrentEntry)item;
				yield return Services.UniverseFactory.NewFileItem(entry.Path) as Item;
			}
		}
	}
}
