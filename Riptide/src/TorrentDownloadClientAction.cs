// TorrentDownloadClientAction.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;


using Do.Universe;
using Do.Platform;

namespace Do.Riptide
{
	
	
	public class TorrentDownloadClientAction : Act
	{
		
		public override string Name {
			get { return "Open Torrent"; }
		}
		
		public override string Description {
			get { return "Download a torrent with your favorite torrent client"; }
		}

		public override string Icon {
			get { return "stock_internet"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] { typeof (TorrentResultItem) }; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string torrentFolder;
			string filename;
			TorrentResultItem item;
			WebClient client;
			
			//We need a place to store our torrents
			torrentFolder = Path.Combine (Services.Paths.UserDataDirectory, "torrents/");
			if (!System.IO.Directory.Exists (torrentFolder))
				System.IO.Directory.CreateDirectory (torrentFolder);
			
			item = items.First () as TorrentResultItem;
			
			string[] temp = item.URL.Split (new char[] {'/'});
			filename = temp[temp.Length - 1];
			
			client = new WebClient ();
			//client.DownloadFile (item.URL, Paths.Combine (torrentFolder, filename));
			client.DownloadFileCompleted += OnFileDownloaded;
			client.DownloadFileAsync (new System.Uri (item.URL), Path.Combine (torrentFolder, filename), filename);
			
			return null;
		}
		
		private void OnFileDownloaded (object o, System.ComponentModel.AsyncCompletedEventArgs args)
		{
			string torrentFolder;
			string filename = args.UserState as string;
			
			torrentFolder = Path.Combine (Services.Paths.UserDataDirectory, "torrents/");
			if (!System.IO.Directory.Exists (torrentFolder))
				System.IO.Directory.CreateDirectory (torrentFolder);
			
			System.Diagnostics.Process proc = new System.Diagnostics.Process ();
			proc.StartInfo.FileName = "xdg-open";
			proc.StartInfo.Arguments = Path.Combine (torrentFolder, filename);
			
			proc.Start ();
		}
	}
}














