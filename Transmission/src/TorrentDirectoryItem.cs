
using System;
using System.Collections.Generic;

using Do.Universe;

namespace Transmission {

	public class TorrentDirectoryItem: Item, ITorrentEntry {

		private TorrentItem _torrent;
		private TorrentDirectoryItem _parent;
		private string _name;
		private IList<Item> _files;

		public TorrentDirectoryItem(TorrentItem torrent, TorrentDirectoryItem parent, string name) {
			_torrent = torrent;
			_parent = parent;
			_name = name;
			_files = new List<Item>();
		}

		public TorrentItem Torrent {
			get { return _torrent; }
		}

		public IEnumerable<TorrentFileItem> GetFiles() {
			foreach (ITorrentEntry entry in _files)
				foreach (TorrentFileItem file in entry.GetFiles())
					yield return file;
		}

		public override string Name {
			get { return _name; }
		}

		public override string Description {
			get { return string.Empty; }
		}

		public override string Icon {
			get { return "folder"; }
		}

		public IList<Item> Files {
			get { return _files; }
		}

		public string Path {
			get {
				if (_parent != null) {
					return _parent.Path + '/' + _name;
				} else {
					return _name;
				}
			}
		}

		public string Uri {
			get { return "file://" + Path; }
		}
	}

}
