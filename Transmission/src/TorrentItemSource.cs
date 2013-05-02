
using System;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;
using Do.Platform.Linux;

namespace Transmission {

	public class TorrentItemSource: ItemSource, IConfigurable {

		private List<Item> _torrents = new List<Item>();

		public TorrentItemSource() {
		}

		public override string Name {
			get { return "Torrents"; }
		}

		public override string Description {
			get { return "Transmission torrent client downloads"; }
		}

		public override string Icon {
			get { return "transmission"; }
		}

		public override void UpdateItems () {
			Log<TorrentItemSource>.Debug("Updating torrents list");

			// Clear current torrents list.
			_torrents.Clear();

			TransmissionAPI api = TransmissionPlugin.getTransmission();

			foreach (TransmissionAPI.TorrentInfo t in api.GetAllTorrents()) {
				Log<TorrentItemSource>.Debug("Torrent: {0}", t.Name);

				TorrentItem torrent = new TorrentItem(t);

				// Transmission returns files as flat list with full names, this map
				// is used to organize files into hierarchy.
				// It maps directory path to directory item.
				Dictionary<string, TorrentDirectoryItem> dirs = new Dictionary<string, TorrentDirectoryItem>();
				dirs.Add("", torrent.Root);

				int index = 0; // File index within list.
				foreach (TransmissionAPI.TorrentFileInfo f in t.Files) {
					// Split path and name.
					int sep_pos = f.Name.LastIndexOf('/');

					string name = f.Name.Substring(sep_pos+1);
					string path = f.Name.Substring(0, sep_pos == -1 ? 0 : sep_pos);
					Log<TorrentItemSource>.Debug("File {0} in dir {1}", name, path);
					TorrentDirectoryItem dir = FindOrCreateDirectory(path, dirs);

					dir.Files.Add(new TorrentFileItem(torrent, index, dir, name, f));

					++index;
				}

				_torrents.Add(torrent);
			}
		}

		private TorrentDirectoryItem FindOrCreateDirectory(
			string path,
			Dictionary<string, TorrentDirectoryItem> dirs
		) {
			TorrentDirectoryItem dir;
			dirs.TryGetValue(path, out dir);

			if (dir != null) {
				// Found already added directory.
				return dir;

			} else {
				// Directory doesn't exist, find or add parent one, then add this one.
				int sep_pos = path.LastIndexOf('/');

				string parent_path = path.Substring(0, sep_pos == -1 ? 0 : sep_pos);
				TorrentDirectoryItem parent = FindOrCreateDirectory(parent_path, dirs);

				string name = path.Substring(sep_pos+1);
				dir = new TorrentDirectoryItem(parent.Torrent, parent, name);

				parent.Files.Add(dir);
				dirs.Add(path, dir);

				return dir;
			}
		}

		public override IEnumerable<Item> Items {
			get { return _torrents; }
		}

		public override IEnumerable<Item> ChildrenOfItem(Item item) {
			if (item is TorrentItem) {
				foreach (Item entry in ((TorrentItem)item).Root.Files)
					yield return entry;

			} else if (item is TorrentDirectoryItem) {
				foreach (Item entry in ((TorrentDirectoryItem)item).Files)
					yield return entry;

			} else {
				yield break;

			}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof(TorrentItem);
				yield return typeof(TorrentDirectoryItem);
				yield return typeof(TorrentFileItem);
			}
		}

		public Gtk.Bin GetConfiguration() {
			return new TransmissionConfig();
		}

	}
}
