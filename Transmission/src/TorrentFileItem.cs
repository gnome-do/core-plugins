
using System;
using System.Collections.Generic;

using Do.Universe;

namespace Transmission {

	public class TorrentFileItem: Item, ITorrentEntry {

		// Owner torrent.
		private TorrentItem _torrent;
		// Position of this file within torrents file list.
		private int _index;

		private TorrentDirectoryItem _parent;
		private string _name;
		private long _size, _downloaded;
		private TransmissionAPI.FilePriority _priority;

		public TorrentFileItem(TorrentItem torrent, int index, TorrentDirectoryItem parent, TransmissionAPI.TorrentFileInfo info) {
			_torrent = torrent;
			_index = index;

			_parent = parent;
			_name = info.Name;
			_size = info.Length;
			_downloaded = info.BytesCompleted;

			_priority = info.Priority;
		}

		public TorrentItem Torrent {
			get { return _torrent; }
		}

		public int Index {
			get { return _index; }
		}

		public TransmissionAPI.FilePriority Priority {
			get { return _priority; }
		}

		public IEnumerable<TorrentFileItem> GetFiles() {
			yield return this;
		}

		public override string Name {
			get { return _name; }
		}

		public override string Description {
			get { return string.Format("{0} of {1}", Utils.FormatSize(_downloaded), Utils.FormatSize(_size)); }
		}

		public override string Icon {
			get { return "document"; }
		}

		public string Path {
			get { return _parent.Path + '/' + _name; }
		}

		public string Uri {
			get { return "file://" + Path; }
		}
	}

}
