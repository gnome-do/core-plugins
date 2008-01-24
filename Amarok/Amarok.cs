//  Amarok.cs
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
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Data.Sqlite;

namespace Do.Plugins.Amarok
{

	public static class Amarok
	{
		static readonly string MusicLibraryFile;
		static readonly string CoverArtDirectory;

		static Amarok ()
		{
			string home;
			home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			MusicLibraryFile = "~/.kde/share/apps/amarok/collection.db".Replace("~", home);
			CoverArtDirectory = "~/.gnome2/amarok/covers".Replace("~", home);
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
			List<SongMusicItem> songs;

			songs = new List<SongMusicItem> ();
			try {
				using (IDbConnection db = new SqliteConnection ("URI=file:" + MusicLibraryFile)) {
					IDbCommand query;

					db.Open ();
					query = db.CreateCommand ();
					query.CommandText = "SELECT a.name, t.title, t.url, i.path, album.name FROM tags t, artist a, album LEFT JOIN statistics s ON (t.url = s.url) LEFT JOIN images i ON (a.name = i.artist AND album.name = i.album) WHERE t.album = album.id AND t.artist = a.id";
					using (IDataReader reader = query.ExecuteReader ()) {
						while (reader.Read ()) {

							SongMusicItem song;
							string song_file, song_name, album_name, artist_name, year, cover;
							
							year = null;
							song_name = reader[1] as string;
							artist_name = reader[0] as string;
							song_file = reader[2] as string;
							cover = reader[3] as string;
							album_name = reader[4] as string;
							
							if (song_file[0] == '.')
								song_file = song_file.Substring (1);

							if (string.IsNullOrEmpty (cover) || !File.Exists (cover))
								cover = null;

							song = new SongMusicItem (song_name, artist_name, album_name, year, cover, song_file);
							songs.Add (song);
						}
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Amarok database file: " + e.ToString ());
			}
			return songs;
		}

		public static void StartIfNeccessary ()
		{
			if (!InstanceIsRunning) {
				Process.Start ("amarok");
				System.Threading.Thread.Sleep (4 * 1000);
			}
		}

		public static bool InstanceIsRunning
		{
			get {
				Process pidof;

				try {
					// Use pidof command to look for Amarok process. Exit
					// status is 0 if at least one matching process is found.
					// If there's any error, just assume some it's running.
					pidof = Process.Start ("pidof", "amarokapp");
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
			Console.WriteLine (command);
			try {
				client = Process.Start ("amarok", command);
				if (wait)
					client.WaitForExit ();
			} catch {
			}
		}
	}
}
