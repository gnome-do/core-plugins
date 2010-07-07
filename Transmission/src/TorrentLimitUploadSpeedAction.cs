
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Transmission {
	
	public class TorrentLimitUploadSpeedAction: Act {

		public TorrentLimitUploadSpeedAction() {
		}

	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Limit upload speed"); }
	    }

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Set upload speed limit"); }
		}

		public override string Icon {
			get { return "top"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (TorrentItem); }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (PredefinedSpeed);
			}
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem(Item item) {
			TorrentItem torrent = (TorrentItem)item;
			int current = torrent.UploadSpeedLimit;

			yield return new PredefinedSpeed(0, "Unlimited", "Turn upload speed limit off");
			yield return new PredefinedSpeed(current, string.Format("Saved: {0}", Utils.FormatSpeed(current)), "Use limit from torrent settings");
			foreach (PredefinedSpeed speed in Utils.PredefinedSpeedItems)
				yield return speed;
		}

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems) {
			TorrentItem item = items.First() as TorrentItem;
			string speed_str = (modItems.First() as ITextItem).Text;

			int? speed = null;
			try {
				// Try to parse entered speed value.
				speed = Utils.ParseSpeed(speed_str);

			} catch (ArgumentException) {
				Log<TorrentLimitUploadSpeedAction>.Debug("Invalid speed string: {0}", speed_str);

				// Show notification about invalid speed value with some hints on
				// accepted formats.
				string message = AddinManager.CurrentLocalizer.GetString(
					"Can't recognize \"{0}\" as speed\nUse values like: 100k, 50 kb, 20m, 10 mib"
				);
				Services.Notifications.Notify("Transmission", string.Format(message, speed_str), "transmission");
			}

			if (speed.HasValue) {
				TransmissionAPI api = TransmissionPlugin.getTransmission();
				api.SetTorrents(new string[] {item.HashString}, null, true, null, null, speed.Value);
				item.UploadSpeedLimit = speed.Value;
			}

			yield break;
		}
	}
}
