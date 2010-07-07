using System.Collections.Generic;

namespace Transmission {

	public interface ITorrentEntry {
		// Owner torrent.
		TorrentItem Torrent { get; }

		// Path on FS.
		string Path { get; }

		// Get all files under this entry (recursively).
		// For files return file itself.
		IEnumerable<TorrentFileItem> GetFiles();
	}
}
