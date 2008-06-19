// Configuration.cs
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

namespace Do.Riptide
{
	
	
	public partial class Configuration : Gtk.Bin
	{
		public Configuration()
		{
			this.Build();
		
			check_force_encryption.Active = TorrentClientManager.Encryption;
			check_progress_window.Active = TorrentClientManager.ProgressWindow;
			check_torrent_alerts.Active = TorrentClientManager.TorrentAlert;
			
			if (TorrentClientManager.MaxDown >= 900) {
				check_max_download.Active = spin_download_speed.Sensitive = false;
			} else {
				check_max_download.Active = spin_download_speed.Sensitive = true;
				spin_download_speed.Value = TorrentClientManager.MaxDown / 1024;
			}
			
			if (TorrentClientManager.MaxUp >= 900) {
				check_max_upload.Active = spin_upload_speed.Sensitive = false;
			} else {
				check_max_upload.Active = spin_upload_speed.Sensitive = true;
				spin_upload_speed.Value = TorrentClientManager.MaxUp / 1024;
			}
			
			spin_download_port.Value = TorrentClientManager.Port;
			file_directory.SetCurrentFolder (TorrentClientManager.DownloadDir);
		}

		protected virtual void OnCheckMaxDownloadClicked (object sender, System.EventArgs e)
		{
			spin_download_speed.Sensitive = check_max_download.Active;
			if (check_max_download.Active) {
				spin_download_speed.Value = 30;
			} else {
				spin_download_speed.Value = 0;
				TorrentClientManager.MaxDown = 0;
			}
		}

		protected virtual void OnCheckMaxUploadClicked (object sender, System.EventArgs e)
		{
			spin_upload_speed.Sensitive = check_max_upload.Active;
			if (check_max_upload.Active) {
				spin_upload_speed.Value = 10;
			} else {
				spin_upload_speed.Value = 0;
				TorrentClientManager.MaxUp = 0;
			}
		}
		protected virtual void OnCheckForceEncryptionClicked (object sender, System.EventArgs e)
		{
			TorrentClientManager.Encryption = check_force_encryption.Active;
		}

		protected virtual void OnCheckProgressWindowClicked (object sender, System.EventArgs e)
		{
			TorrentClientManager.ProgressWindow = check_progress_window.Active;
		}

		protected virtual void OnCheckTorrentAlertsClicked (object sender, System.EventArgs e)
		{
			TorrentClientManager.TorrentAlert = check_torrent_alerts.Active;
		}

		protected virtual void on_download_directory_changed (object sender, System.EventArgs e)
		{
			TorrentClientManager.DownloadDir = file_directory.CurrentFolder;
		}
		protected virtual void OnSpinDownloadSpeedValueChanged (object sender, System.EventArgs e)
		{
			if (check_max_download.Active)
				TorrentClientManager.MaxDown = (int) spin_download_speed.Value * 1024;
		}
		protected virtual void OnSpinUploadSpeedValueChanged (object sender, System.EventArgs e)
		{
			if (check_max_upload.Active)
				TorrentClientManager.MaxUp = (int) spin_upload_speed.Value * 1024;
		}
		protected virtual void OnSpinDownloadPortValueChanged (object sender, System.EventArgs e)
		{
			TorrentClientManager.Port = (int) spin_download_port.Value;
		}
	}
}









