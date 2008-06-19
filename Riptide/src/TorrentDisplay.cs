// TorrentDisplay.cs
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
using MonoTorrent.Common;

namespace Do.Riptide
{
	
	
	public partial class TorrentDisplay : Gtk.Bin
	{
		private TorrentManager torrent;
		
		public TorrentDisplay(TorrentManager torrent)
		{
			this.Build();
			
			this.torrent = torrent;
			torrent.TorrentStateChanged += OnTorrentStateChanged;
			
			GLib.Timeout.Add (2000, OnTorrentUpdate);
			
			label.Text = torrent.Torrent.Name;
			label.ModifyFg (Gtk.StateType.Normal,
				new Gdk.Color (byte.MaxValue, byte.MaxValue, byte.MaxValue));
			ShowAll ();
		}

		protected virtual void OnPauseButtonClicked (object sender, System.EventArgs e)
		{
			TorrentPauseToggle (this, torrent);
		}

		protected virtual void OnCancelButtonClicked (object sender, System.EventArgs e)
		{
			TorrentStopped (this, torrent);
		}
		
		private bool OnTorrentUpdate ()
		{
			progressbar.Fraction = torrent.Progress / 100;
			progressbar.Text = (torrent.Monitor.DownloadSpeed / 1024).ToString () + "KB/s";
			
			return (torrent.Progress < 100);
		}
		
		private void OnTorrentStateChanged (object o, TorrentStateChangedEventArgs args)
		{
			if (args.NewState == TorrentState.Paused)
				pauseplay.Pixbuf = Stetic.IconLoader.LoadIcon
					(this, "gtk-media-play", Gtk.IconSize.Menu, 16);
			else if (args.NewState == TorrentState.Downloading)
				pauseplay.Pixbuf = Stetic.IconLoader.LoadIcon
					(this, "gtk-media-pause", Gtk.IconSize.Menu, 16);
			else if (args.NewState == TorrentState.Seeding)
				progressbar.Text = "Seeding at " + (torrent.Monitor.UploadSpeed / 1024).ToString () + " KB/s";
		}
		
		public event TorrentDisplayChangedHandler TorrentStopped;
		public event TorrentDisplayChangedHandler TorrentPauseToggle;
			
		public delegate void TorrentDisplayChangedHandler (TorrentDisplay display, TorrentManager manager);
	}
}







