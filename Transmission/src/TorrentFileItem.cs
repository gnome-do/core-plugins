
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
		private bool _wanted;
		private TransmissionAPI.FilePriority _priority;

		public TorrentFileItem(
			TorrentItem torrent, int index, TorrentDirectoryItem parent,
			string name, TransmissionAPI.TorrentFileInfo info
		) {
			_torrent = torrent;
			_index = index;

			_parent = parent;
			_name = name;
			_size = info.Length;
			_downloaded = info.BytesCompleted;

			_wanted = info.Wanted;
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
			get {
				// I don't use special percentage format string, because it rounds
				// value and I don't want to get "100%" until file is really downloaded.
				// High precision isn't needed, because info is mostly out-of-date.

				if (_downloaded == _size)
					return string.Format("Complete, {0}", Utils.FormatSize(_size));

				else if (_wanted)
					return string.Format("{0} of {1} complete ({2:0}%)",
						Utils.FormatSize(_downloaded), Utils.FormatSize(_size),
						Math.Floor(100.0 * _downloaded / _size)
					);

				else if (_downloaded != 0)
					return string.Format("Skipped, {0} of {1} complete ({2:0}%)",
						Utils.FormatSize(_downloaded),Utils.FormatSize(_size),
						Math.Floor(100.0 * _downloaded / _size)
					);

				else
					return string.Format("Skipped, {0}", Utils.FormatSize(_size));
			}
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
