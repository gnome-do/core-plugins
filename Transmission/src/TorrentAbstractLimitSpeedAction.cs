
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Transmission {

	public abstract class TorrentAbstractLimitSpeedAction: Act {

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

		protected abstract PredefinedSpeed GetCurrentSpeedItem(TorrentItem torrent);

		public override IEnumerable<Item> DynamicModifierItemsForItem(Item item) {
			TorrentItem torrent = (TorrentItem)item;

			yield return new PredefinedSpeed(0, "Unlimited", "Turn download speed limit off");
			yield return GetCurrentSpeedItem(torrent);
			foreach (PredefinedSpeed speed in Utils.PredefinedSpeedItems)
				yield return speed;
		}

		protected abstract void SetSpeedLimit(TransmissionAPI api, TorrentItem torrent, int speed);

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems) {
			TorrentItem item = items.First() as TorrentItem;

			int? speed = null;

			// Get speed item, it can be either ITextItem or PredefinedSpeed.
			Item modItem = modItems.First();
			if (modItem is PredefinedSpeed) {
				speed = ((PredefinedSpeed)modItem).Value;
			} else {
				string speed_str = ((ITextItem)modItem).Text;

				try {
					// Try to parse entered speed value.
					speed = Utils.ParseSpeed(speed_str);

				} catch (ArgumentException) {
					Log<TransmissionPlugin>.Debug("Invalid speed string: {0}", speed_str);

					// Show notification about invalid speed value with some hints on
					// accepted formats.
					string message = AddinManager.CurrentLocalizer.GetString(
						"Can't recognize \"{0}\" as speed\nUse values like: 100k, 50 kb, 20m, 10 mib"
					);
					Services.Notifications.Notify("Transmission", string.Format(message, speed_str), "transmission");
				}
			}

			// If speed is recognized successfully, set speed limit and update item.
			if (speed.HasValue) {
				TransmissionAPI api = TransmissionPlugin.getTransmission();
				SetSpeedLimit(api, item, speed.Value);
			}

			yield break;
		}
	}
}
