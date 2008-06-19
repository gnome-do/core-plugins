// TorrentClientManager.cs
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

using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Common;

using Do.Addins;
using Do.Universe;

namespace Do.Riptide
{
	public class TorrentClientManager
	{
		private static List<TorrentManager> managers;
		private static ClientEngine client;
		private static IPreferences prefs;
		private static WindowTorrentView torrentWindow;
		
		internal static bool ProgressWindow {
			get {
				return prefs.Get<bool> ("Progress_Window", true);
			}
			set {
				prefs.Set<bool> ("Progress_Window", value);
				if (value && managers.Count > 0)
					torrentWindow.Show ();
				else
					torrentWindow.Hide ();
			}
		}
		
		internal static bool TorrentAlert {
			get {
				return prefs.Get<bool> ("Torrent_Alerts", true);
			}
			set {
				prefs.Set<bool> ("Torrent_Alerts", value);
			}
		}
		
		internal static bool Encryption {
			get {
				return prefs.Get<bool> ("Encrypted_Headers", false);
			}
			set {
				prefs.Set<bool> ("Encrypted_Headers", value);
				if (value)
					client.Settings.MinEncryptionLevel = EncryptionType.RC4Header;
				else
					client.Settings.MinEncryptionLevel = EncryptionType.None;
			}
		}
		
		internal static int MaxDown {
			get {
				return prefs.Get<int> ("Maximum_Download_Speed", 900);
			}
			set {
				prefs.Set<int> ("Maximum_Download_Speed", value);
				client.Settings.GlobalMaxDownloadSpeed = value;
			}
		}
		
		internal static int MaxUp {
			get {
				return prefs.Get<int> ("Maximum_Upload_Speed", 10);
			}
			set {
				prefs.Set<int> ("Maximum_Upload_Speed", value);
				client.Settings.GlobalMaxUploadSpeed = value;
			}
		}
		
		internal static int Port {
			get {
				return prefs.Get<int> ("Listen_Port", 3500);
			}
			set {
				prefs.Set<int> ("Listen_Port", value);
				client.Settings.ListenPort = value;
			}
		}
		
		internal static string DownloadDir {
			get {
				return prefs.Get<string> ("Download_Directory", System.Environment.SpecialFolder.Desktop.ToString ());
			}
			set {
				prefs.Set<string> ("Download_Directory", value);
				client.Settings.SavePath = value;
			}
		}
		
		private static EncryptionType EncryptionLevel {
			get {
				if (Encryption)
					return EncryptionType.RC4Header;
				else
					return EncryptionType.None;
			}
		}

		public static TorrentManager[] Managers {
			get {
				return managers.ToArray ();
			}
		}
		
		static TorrentClientManager ()
		{
			prefs = Do.Addins.Util.GetPreferences ("Riptide");
			torrentWindow = new WindowTorrentView ();
			torrentWindow.Hide ();
			managers = new List<TorrentManager> ();
			
			EngineSettings settings = new EngineSettings ();
			settings.GlobalMaxDownloadSpeed = MaxDown;
			settings.GlobalMaxUploadSpeed   = MaxUp;
			settings.ListenPort             = Port;
			settings.MinEncryptionLevel     = EncryptionLevel;
			
			client = new ClientEngine (settings);
			
			AppDomain.CurrentDomain.ProcessExit += delegate {
				client.StopAll ();
				client.Dispose ();
				client = null;
			};
			
		}
		
		internal static void NewTorrent (TorrentManager manager)
		{
			managers.Add (manager);
			client.Register (manager);
			manager.TorrentStateChanged += OnTorrentStateChanged;
			
			if (ProgressWindow)
				torrentWindow.Show ();
			torrentWindow.AddTorrent (manager);
			torrentWindow.Stick ();
			
			Gtk.Application.Invoke (delegate { manager.Start (); });
		}
		
		protected static void OnTorrentStateChanged (object o, TorrentStateChangedEventArgs args)
		{
			if (args.NewState != TorrentState.Stopped) return;
			
			managers.Remove (args.TorrentManager);
			client.Unregister (args.TorrentManager);
			
			if (args.OldState == TorrentState.Downloading) {
				foreach (TorrentFile file in args.TorrentManager.FileManager.Files) {
					System.IO.File.Delete (file.Path);
				}
			}
			
			args.TorrentManager.Dispose ();
			
			if (managers.Count <= 0)
				torrentWindow.Hide ();
		}
		
		protected static void Resize ()
		{
			torrentWindow.HeightRequest = 0;
			torrentWindow.WidthRequest  = 0;
		}
	}
}
