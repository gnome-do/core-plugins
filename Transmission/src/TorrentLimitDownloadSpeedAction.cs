
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Transmission {

	public class TorrentLimitDownloadSpeedAction: TorrentAbstractLimitSpeedAction {

	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString("Limit download speed"); }
	    }

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString("Set download speed limit"); }
		}

		public override string Icon {
			get { return "top"; }
		}

		protected override PredefinedSpeed GetCurrentSpeedItem(TorrentItem torrent) {
			int currentSpeed = torrent.DownloadSpeedLimit;
			return new PredefinedSpeed(
				currentSpeed,
				string.Format("Saved: {0}", Utils.FormatSpeed(currentSpeed)),
				"Use limit from torrent settings"
			);
		}

		protected override void SetSpeedLimit(TransmissionAPI api, IEnumerable<TorrentItem> torrents, int speed) {
			bool limit_speed = (speed != 0);
			int? limit = (speed == 0 ? (int?)null : speed);
			api.SetTorrents(torrents.Select(t => t.HashString), null, limit_speed, limit, null, null);

			foreach (TorrentItem torrent in torrents)
				torrent.DownloadSpeedLimit = speed;
		}

	}
}
