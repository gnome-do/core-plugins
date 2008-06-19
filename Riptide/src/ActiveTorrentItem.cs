// TorrentActionItem.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using MonoTorrent.Client;

using Do.Universe;

namespace Do.Riptide
{
	
	
	public class ActiveTorrentItem : ITorrentItem
	{
		private string name, description;
		private TorrentManager torrent;
		
		public string Name {
			get {
				return name;
			}
		}

		public string Description {
			get {
				return description;
			}
		}

		public string Icon {
			get {
				return "stock_internet";
			}
		}

		public TorrentManager ActiveTorrent {
			get {
				return torrent;
			}
		}

		
		public ActiveTorrentItem(TorrentManager torrent)
		{
			this.torrent = torrent;
			this.name = torrent.Torrent.Name;
			
			if (torrent.Complete) {
				description = "Seeding";
			} else {
				description = torrent.Progress.ToString () + "% Downloaded at " + 
					torrent.Monitor.DownloadSpeed / 1024 + " KB/s";
			}
		}
	}
}
