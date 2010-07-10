
using System;
using System.Collections.Generic;

using Do.Universe;

namespace Transmission {

	public class TorrentItem: Item {

		private string _name;
		private string _comment;
		private string _hash_string;
		private long _size;
		private TransmissionAPI.TorrentStatus _status;
		private int _download_speed_limit, _upload_speed_limit;
		private TorrentDirectoryItem _root;

		public TorrentItem(TransmissionAPI.TorrentInfo info) {
			_hash_string = info.HashString;
			_name = info.Name;
			_comment = info.Comment;
			_status = info.Status;
			_size = info.TotalSize;
			_download_speed_limit = info.DownloadLimit;
			_upload_speed_limit = info.UploadLimit;
			_root = new TorrentDirectoryItem(this, null, info.DownloadDir);
		}

		public override string Name {
			get { return _name; }
		}

		public override string Description {
			get {
				string status_text = "";
				switch (_status) {
				case TransmissionAPI.TorrentStatus.CheckWait: status_text = "Waiting for check"; break;
				case TransmissionAPI.TorrentStatus.Check:     status_text = "Checking"; break;
				case TransmissionAPI.TorrentStatus.Download:  status_text = "Downloading"; break;
				case TransmissionAPI.TorrentStatus.Seed:      status_text = "Seeding"; break;
				case TransmissionAPI.TorrentStatus.Stopped:   status_text = "Stopped"; break;
				}

				return string.Format("{0}, {1}", Utils.FormatSize(_size), status_text);
			}
		}

		public override string Icon {
			get { return "transmission"; }
		}

		public string HashString {
			get { return _hash_string; }
		}

		public TorrentDirectoryItem Root {
			get { return _root; }
		}

		public int DownloadSpeedLimit {
			get { return _download_speed_limit; }
			set { _download_speed_limit = value; }
		}

		public int UploadSpeedLimit {
			get { return _upload_speed_limit; }
			set { _upload_speed_limit = value; }
		}
	}

}
