
using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Transmission {

	public class TorrentUnmarkForDownloadAction: Act {

		public TorrentUnmarkForDownloadAction() {
		}

	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Unmark for download"); }
	    }

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Unmark file as needed to be downloaded"); }
		}

		public override string Icon {
			get { return "remove"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITorrentEntry);
			}
		}

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems) {
			TransmissionAPI api = TransmissionPlugin.getTransmission();

			// Operation is common for all files.
			TransmissionAPI.FileOperation operation = new TransmissionAPI.FileOperation(false, null);

			// Group selected items by owner torrent.
			var files_by_torrent = items
				.Cast<ITorrentEntry>()
				.GroupBy(
					item => item.Torrent,
					(torrent, entries) => new {
						Torrent = torrent,
						Files = entries.SelectMany(entry => entry.GetFiles())
					}
				);

			// Perform action for each torrent separately.
			foreach (var group in files_by_torrent) {
				var operations = group.Files.ToDictionary(f => f.Index, f => operation);
				api.SetTorrent(group.Torrent.HashString, null, null, null, null, null, operations);
			}

			yield break;
		}
	}
}
