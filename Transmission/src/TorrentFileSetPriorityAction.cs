
using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Transmission {

	public class PriorityItem: Item {
		private TransmissionAPI.FilePriority _value;
		private string _name;
		private string _icon;

		public PriorityItem(TransmissionAPI.FilePriority value, string name, string icon) {
			_value = value;
			_name = name;
			_icon = icon;
		}

		public override string Name {
			get { return _name; }
		}

		public override string Description {
			get { return ""; }
		}

		public override string Icon {
			get { return _icon; }
		}

		public TransmissionAPI.FilePriority Value {
			get { return _value; }
		}
	}

	public class TorrentFileSetPriorityAction: Act {

		public TorrentFileSetPriorityAction() {
		}

	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Set priority"); }
	    }

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Set download priority"); }
		}

		public override string Icon {
			get { return "object-flip-vertical"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITorrentEntry);
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				yield return typeof (PriorityItem);
			}
		}

		public override bool ModifierItemsOptional {
			get { return false; }
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem(Item item) {
			yield return new PriorityItem(TransmissionAPI.FilePriority.Low, "Low", "down");
			yield return new PriorityItem(TransmissionAPI.FilePriority.Normal, "Normal", "forward");
			yield return new PriorityItem(TransmissionAPI.FilePriority.High, "High", "up");
		}

		public override IEnumerable<Item> Perform(IEnumerable<Item> items, IEnumerable<Item> modItems) {
			TransmissionAPI.FilePriority priority = (modItems.First() as PriorityItem).Value;

			TransmissionAPI.FileOperation operation = new TransmissionAPI.FileOperation(null, priority);

			// Group torrent entries by torrent.
			var files_by_torrent = items
				.Cast<ITorrentEntry>()
				.GroupBy(
					item => item.Torrent,
					(torrent, entries) => new {
						Torrent = torrent,
						Files = entries.SelectMany(entry => entry.GetFiles())
					}
				);

			TransmissionAPI api = TransmissionPlugin.getTransmission();

			// Expand entries for each torrent into set of torrent file entries.
			// Perform action for each torrent separately.
			foreach (var group in files_by_torrent) {
				var operations = group.Files.ToDictionary(f => f.Index, f => operation);
				api.SetTorrent(group.Torrent.HashString, null, null, null, null, null, operations);
			}

			yield break;
		}
	}
}
