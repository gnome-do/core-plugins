//  Exaile.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed
//  with this source distribution.
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
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Data.Sqlite;

using Do.Platform;

namespace Exaile
{

	public static class Exaile
	{

		static readonly string MusicLibraryFile;
		static readonly string CoverArtDirectory;

		static ICollection<SongMusicItem> songs;

		static Timer clear_songs_timer;
		const int SecondsSongsCached = 45;

		static Exaile ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

			MusicLibraryFile = Path.Combine (home, ".exaile/music.db");
			CoverArtDirectory = Path.Combine (home, ".exaile/covers");

			clear_songs_timer = new Timer (state => Gtk.Application.Invoke ((sender, args) => songs.Clear ()));

			songs = new List<SongMusicItem> ();
		}

		public static void LoadAlbumsAndArtists (
			out List<AlbumMusicItem> albums_out,
			out List<ArtistMusicItem> artists_out)
		{
			albums_out = new List<AlbumMusicItem> ();
			artists_out = new List<ArtistMusicItem> ();

			Dictionary<string, AlbumMusicItem> albums = new Dictionary<string, AlbumMusicItem> ();
			Dictionary<string, ArtistMusicItem> artists = new Dictionary<string, ArtistMusicItem> ();

			foreach (SongMusicItem song in LoadAllSongs ()) {
				if (!artists.ContainsKey (song.Artist) || artists[song.Artist].Cover == null)
					artists[song.Artist] = new ArtistMusicItem (song.Artist, song.Cover);

				if (!albums.ContainsKey (song.Album) || albums[song.Album].Cover == null)
					albums[song.Album] = new AlbumMusicItem (song.Album, song.Artist, song.Year, song.Cover);
			}
			albums_out.AddRange (albums.Values);
			artists_out.AddRange (artists.Values);
		}

		public static IEnumerable<SongMusicItem> LoadSongsFor (MusicItem item)
		{
			if (item is SongMusicItem)
				return new SongMusicItem[] { item as SongMusicItem };

			else if (item is ArtistMusicItem)
				return LoadAllSongs ()
					.Where (song => song.Artist.Contains (item.Name))
					.OrderBy (song => song.Album).ThenBy (song => song.Track);

			else if (item is AlbumMusicItem)
				return LoadAllSongs ()
					.Where (song => song.Album == item.Name)
					.OrderBy (song => song.Track);

			else
				return Enumerable.Empty<SongMusicItem> ();
		}

		public static IEnumerable<SongMusicItem> LoadAllSongs ()
		{
			// Begin a new timer to clear the songs.
			clear_songs_timer.Change (SecondsSongsCached*1000, Timeout.Infinite);

			if (songs.Any ()) return songs;

			// If the database doesn't exist then just return.
			// This prevents the code below from creating an empty database
			// which causes Exaile to crash.
			if (!File.Exists (MusicLibraryFile)) {
				Log.Error ("Could not find Exaile database file: " + MusicLibraryFile);
				return songs;
			}
				
			// Song list is not cached. Load songs from database.
			try {
				string connectionString = "URI=file:" + MusicLibraryFile;
				using (IDbConnection dbcon = new SqliteConnection(connectionString)) {
					using (IDbCommand dbcmd = dbcon.CreateCommand()) {
						dbcon.Open();
						dbcmd.CommandText =
							"SELECT tracks.title, artists.name, albums.name, tracks.year, albums.image, paths.name, tracks.track " +
							"FROM tracks " +
							"JOIN artists ON tracks.artist = artists.id " +
							"JOIN albums ON tracks.album = albums.id " +
							"JOIN paths ON tracks.path = paths.id";

						using (IDataReader reader = dbcmd.ExecuteReader()) {
							while(reader.Read()) {
								string name = reader.GetString(0);
								string artist = reader.GetString(1);
								string album = reader.GetString(2);
								string year = reader.GetString(3);
								object image_value = reader.GetValue(4);
								string cover = null;
								string file = reader.GetString(5);
								int track = reader.GetInt32(6);

								if (image_value is string)
									cover = Path.Combine(CoverArtDirectory, image_value as string);

								songs.Add(new SongMusicItem(name, artist, album, year, cover, file, track));
							}
						}
					}
				}
			} catch (Exception e) {
				Log.Error ("Could not read Exaile database file: " + e.Message);
			}
			return songs;
		}

		public static bool InstanceIsRunning
		{
			get {
				try {
					ProcessStartInfo pinfo =
						new ProcessStartInfo ("pgrep", "exaile");
					pinfo.UseShellExecute = false;
					pinfo.RedirectStandardOutput = true;

					Process pgrep = Process.Start (pinfo);
					pgrep.WaitForExit ();
					return pgrep.ExitCode == 0;
				} catch (Exception e) {
					Log.Error ("Could not determine if Exaile is running: {0}", e.Message);
					Log.Debug (e.StackTrace);
				}
				return true;
			}
		}

		public static void Client (string command)
		{
			Client (command, false);
		}

		public static void Client (string command, bool wait)
		{
			try {
				Process client = Process.Start ("exaile", command);
				if (wait) client.WaitForExit ();
			} catch {
			}
		}
	}
}
