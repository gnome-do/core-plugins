//  RhythmboxDBus.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;

using DBus;

namespace Do.Rhythmbox
{
	public struct Playlist
	{
		public ObjectPath Id;
		public string Name;
		public string Icon;
	}

	[Interface ("org.mpris.MediaPlayer2.Playlists")]
	public interface IPlaylists
	{
		void ActivatePlaylist (ObjectPath playlist_id);
		Playlist [] GetPlaylists (uint index, uint max_count, string order, bool reverse_order);
	}

	public class RhythmboxDBus
	{
		private static IPlaylists mprisPlaylists = null;

		public RhythmboxDBus ()
		{
		}

		private static IPlaylists MPRISPlaylists {
			get { 
				if (mprisPlaylists == null) {
					try {
						mprisPlaylists = Bus.Session.GetObject<IPlaylists> ("org.mpris.MediaPlayer2.rhythmbox", new ObjectPath ("/org/mpris/MediaPlayer2"));
					} catch (Exception e) {
						Console.Error.WriteLine ("[Rhythmbox] Bus.Session.GetObject failed: " + e);
					}
				}
				return mprisPlaylists;
			}
		}

		public static IEnumerable<Playlist> Playlists {
			get {
				Playlist[] playlists = null;
				try {
					playlists = MPRISPlaylists.GetPlaylists(0, 4096, "Ascending", false);
				}
				catch (Exception e) {
					Console.Error.WriteLine ("[Rhythmbox] GetPlaylists via MPRIS (D-Bus) failed: " + e);
					if (e.Message.StartsWith("org.freedesktop.DBus.Error.ServiceUnknown"))
						Console.Error.WriteLine("[Rhythmbox] If Rhythmbox is running, please ensure that the MPRIS plugin is enabled.");
				}

				return playlists != null ? new List<Playlist> (playlists) : Enumerable.Empty<Playlist> ();
			}
		}

		public static void PlayPlaylist(PlaylistMusicItem playlist) {
			if (MPRISPlaylists != null) {
				// We know the playlist's name (playlist.Name), but not its ID. The reason it is not stored in the MusicItem
				// is that if Rhythmbox is restarted while Do is running, the ID changes, and the Play action will fail.
				// In other words, Rhythmbox's MPRIS/DBus Playlist IDs are non-persistent.
				// To avoid problems, we only store the playlist name, and figure out the ID as required, so that it is always
				// up-to-date. This way, the Play action on playlists should always work, even when Rhythmbox is not running.

				Rhythmbox.StartIfNeccessary ();

				foreach (Playlist pl in Playlists) {
					if (pl.Name == playlist.Name) {
						MPRISPlaylists.ActivatePlaylist (pl.Id);
						break;
					}
				}
			}
		}
	}
}
