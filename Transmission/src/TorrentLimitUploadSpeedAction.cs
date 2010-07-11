
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Transmission {

	public class TorrentLimitUploadSpeedAction: TorrentAbstractLimitSpeedAction {

	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Limit upload speed"); }
	    }

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Set upload speed limit"); }
		}

		public override string Icon {
			get { return "top"; }
		}

		protected override PredefinedSpeed GetCurrentSpeedItem(TorrentItem torrent) {
			int currentSpeed = torrent.UploadSpeedLimit;
			return new PredefinedSpeed(
				currentSpeed,
				string.Format("Saved: {0}", Utils.FormatSpeed(currentSpeed)),
				"Use limit from torrent settings"
			);
		}

		protected override void SetSpeedLimit(TransmissionAPI api, TorrentItem torrent, int speed) {
			bool limit_speed = (speed != 0);
			int? limit = (speed == 0 ? (int?)null : speed);
			api.SetTorrents(new string[] {torrent.HashString}, null, null, null, limit_speed, limit);
			torrent.UploadSpeedLimit = speed;
		}

	}
}
