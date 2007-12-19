//  Rhythmbox.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
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
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Do.Addins.Rhythmbox
{

	public class Rhythmbox
	{
		static readonly string kMusicLibraryFile;
		static readonly string kCoverArtDirectory;

		static Rhythmbox ()
		{
			string home;

			home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			kMusicLibraryFile = "~/.gnome2/rhythmbox/rhythmdb.xml".Replace("~", home);
			kCoverArtDirectory = "~/.gnome2/rhythmbox/covers".Replace("~", home);
		}

		public static void LoadAlbumsAndArtists (out List<AlbumMusicItem> albums_out, out List<ArtistMusicItem> artists_out)
		{
			Dictionary<string, AlbumMusicItem> albums;
			Dictionary<string, ArtistMusicItem> artists;

			albums_out = new List<AlbumMusicItem> ();
			artists_out = new List<ArtistMusicItem> ();

			albums = new Dictionary<string, AlbumMusicItem> ();
			artists = new Dictionary<string, ArtistMusicItem> ();
			foreach (SongMusicItem song in LoadAllSongs ()) {	
				// Don't let null covers replace non-null covers.
				if (!artists.ContainsKey (song.Artist) || artists[song.Artist].Cover == null) {
					artists[song.Artist] = new ArtistMusicItem (song.Artist, song.Cover);
				}
				if (!albums.ContainsKey (song.Album) || albums[song.Album].Cover == null) {
					albums[song.Album] = new AlbumMusicItem (song.Album, song.Artist, song.Year, song.Cover);	
				}
			}
			albums_out.AddRange (albums.Values);
			artists_out.AddRange (artists.Values);
		}

		public static List<SongMusicItem> LoadSongsFor (MusicItem item)
		{
			SortedList<string, SongMusicItem> songs;

			if (item is SongMusicItem) {
				List<SongMusicItem> single = new List<SongMusicItem> ();
				single.Add (item as SongMusicItem);
				return single;
			}

			songs = new SortedList<string, SongMusicItem> ();
			foreach (SongMusicItem song in LoadAllSongs ()) {
				switch (item.GetType ().Name) {
					case "AlbumMusicItem":
						if (item.Name != song.Album) continue;
					break;
					case "ArtistMusicItem":
						if (item.Name != song.Artist) continue;
					break;
				}
				try {
					songs.Add (song.File, song);
				} catch { }
			}
			return new List<SongMusicItem> (songs.Values);
		}

		public static List<SongMusicItem> LoadAllSongs ()
		{
			XmlDocument db;
			List<SongMusicItem> songs;

			db = new XmlDocument ();
			songs = new List<SongMusicItem> ();
			try {
				db.Load (kMusicLibraryFile);
				foreach (XmlNode entry in db.GetElementsByTagName ("entry")) {
					SongMusicItem song;
					string song_file, song_name, album_name, artist_name, year, cover;

					song_file = song_name = album_name = artist_name = year = cover = null;
					if (entry.Attributes.GetNamedItem ("type").Value != "song") continue;
					foreach (XmlNode song_attr in entry.ChildNodes) {
						switch (song_attr.Name) {
							case "title":
								song_name = song_attr.InnerText;
							break;
							case "album":
								album_name = song_attr.InnerText;
							break;
							case "artist":
								artist_name = song_attr.InnerText;
							break;
							case "year":
								year = song_attr.InnerText;
							break;
							case "location":
								song_file = song_attr.InnerText;
							break;
						}
					}
					if (song_name == null) continue;

					cover = string.Format ("{0} - {1}.jpg", artist_name, album_name);
					cover = Path.Combine (kCoverArtDirectory, cover);
					if (!File.Exists (cover)) cover = null;

					song = new SongMusicItem (song_name, artist_name, album_name, year, cover, song_file);
					songs.Add (song);
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Rhythmbox database file: " + e.Message);
			}
			return songs;
		}

		public static void StartIfNeccessary ()
		{
			if (!InstanceIsRunning)
			{
				Process.Start ("rhythmbox-client", "--no-present");
				System.Threading.Thread.Sleep (3 * 1000);
			}
		}

		public static bool InstanceIsRunning
		{
			get {
				Process pidof;

				try {
					// Use pidof command to look for Rhythmbox process. Exit
					// status is 0 if at least one matching process is found.
					// If there's any error, just assume some it's running.
					pidof = Process.Start ("pidof", "rhythmbox");
					pidof.WaitForExit ();
					return pidof.ExitCode == 0;
				} catch {
					return true;
				}
			}
		}

		public static void Client (string command)
		{
			Client (command, false);
		}

		public static void Client (string command, bool wait)
		{
			Process client;
			try {
				client = Process.Start ("rhythmbox-client", command);
				if (wait)
					client.WaitForExit ();
			} catch {
			}
		}
	}
}
