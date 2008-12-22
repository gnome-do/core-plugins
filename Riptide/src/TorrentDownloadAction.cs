// TorrentDownloadAction.cs
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
using System.IO;
using System.Linq;
using System.Net;
using MonoTorrent.Common;
using MonoTorrent.Client;


using Do.Universe;

namespace Do.Riptide
{
	
	
	public class TorrentDownloadAction : Act, IConfigurable
	{
		
		public override string Name {
			get { return "Download Torrent"; }
		}
		
		public override string Description {
			get { return "Download a Torrent Right in Gnome Do!"; }
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
			
			WebResponse res = null;	
			WebRequest req;
			Stream remoteStream, localStream;
			
			//We need a place to store our torrents
			torrentFolder = Paths.Combine (Paths.UserData, "torrents/");
			if (!System.IO.Directory.Exists (torrentFolder))
				System.IO.Directory.CreateDirectory (torrentFolder);
			
			item = items.First () as TorrentResultItem;
			
			string[] temp = item.URL.Split (new char[] {'/'});
			filename = temp[temp.Length - 1];
			
			req = WebRequest.Create (item.URL);
			
			req.Timeout = 10000;
			
			try {
				res = req.GetResponse ();
			} catch {
				NotificationBridge.ShowMessage ("Riptide Error", "Torrent file download timed out");
				return null;
			}
			
			remoteStream = res.GetResponseStream ();
			localStream = System.IO.File.Create (Paths.Combine (torrentFolder, filename));
			
			byte[] buffer = new byte[1024];
			int bytesRead;
			
			do {
				bytesRead = remoteStream.Read (buffer, 0, buffer.Length);
				localStream.Write (buffer, 0, bytesRead);
			} while (bytesRead > 0);
			
			res.Close ();
			remoteStream.Close ();
			localStream.Close ();
			
			Torrent torrent = Torrent.Load (Paths.Combine (torrentFolder, filename));
			TorrentManager manager = new TorrentManager(torrent, TorrentClientManager.DownloadDir, new TorrentSettings ());
			
			manager.TorrentStateChanged += OnTorrentStateChanged;
			
			TorrentClientManager.NewTorrent (manager);
			
			return null;
		}
		
		/*private void OnFileDownloaded (object o, System.ComponentModel.AsyncCompletedEventArgs args)
		{
			string torrentFolder;
			string filename = args.UserState as string;
			
			Console.WriteLine ("File Downloaded");
			
			torrentFolder = Paths.Combine (Paths.UserData, "torrents/");
			if (!System.IO.Directory.Exists (torrentFolder))
				System.IO.Directory.CreateDirectory (torrentFolder);
			
			Torrent torrent = Torrent.Load (Paths.Combine (torrentFolder, filename));
			TorrentManager manager = new TorrentManager(torrent, TorrentClientManager.DownloadDir, new TorrentSettings ());
			
			manager.TorrentStateChanged += OnTorrentStateChanged;
			
			TorrentClientManager.NewTorrent (manager);
		}*/

		private void OnTorrentStateChanged (object o, TorrentStateChangedEventArgs args)
		{
			if (!TorrentClientManager.TorrentAlert || args.NewState == TorrentState.Hashing) return;
			string body = args.TorrentManager.Torrent.Name + " is " + args.NewState.ToString ();
			NotificationBridge.ShowMessage ("Torrent Information:", body);
		}

		public Gtk.Bin GetConfiguration ()
		{
			Gtk.Bin conf = new Configuration ();
			return conf;
		}

	}
}













