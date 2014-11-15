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
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Do.Platform;

namespace Do.Rhythmbox
{

	public static class Rhythmbox
	{

		static readonly string MusicLibraryFile;
		static readonly string CoverArtDirectory;

		static ICollection<SongMusicItem> songs;

		static Timer clear_songs_timer;
		const int SecondsSongsCached = 45;

		static Rhythmbox ()
		{
			string xdgDbFile;
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			
			// this is for version compatibility, the new version of RB uses XDG dirs
			xdgDbFile = Path.Combine (ReadXdgUserDir ("XDG_DATA_HOME", ".local/share"), "rhythmbox/rhythmdb.xml");
			MusicLibraryFile = File.Exists (xdgDbFile) 
				? xdgDbFile
				: Path.Combine (home, ".gnome2/rhythmbox/rhythmdb.xml");

			CoverArtDirectory = Path.Combine (ReadXdgUserDir ("XDG_CACHE_HOME", ".cache"), "rhythmbox/covers");

			clear_songs_timer = new Timer (state =>
				Gtk.Application.Invoke ((sender, args) => songs.Clear ())
			);
			songs = new List<SongMusicItem> ();
		}

		public static void LoadAlbumsAndArtists (out List<AlbumMusicItem> albums_out, out List<ArtistMusicItem> artists_out)
		{
			Dictionary<Tuple<string, string>, AlbumMusicItem> albums;
			Dictionary<string, ArtistMusicItem> artists;

			albums_out = new List<AlbumMusicItem> ();
			artists_out = new List<ArtistMusicItem> ();

			albums = new Dictionary<Tuple<string, string>, AlbumMusicItem> ();
			artists = new Dictionary<string, ArtistMusicItem> ();
			foreach (SongMusicItem song in LoadAllSongs ()) {
				Tuple<string, string> album = new Tuple<string, string> (song.Artist, song.Album);
				// Don't let null covers replace non-null covers.
				if (!artists.ContainsKey (song.Artist) || artists[song.Artist].Cover == null) {
					artists[song.Artist] = new ArtistMusicItem (song.Artist, song.Cover);
				}
				if (!albums.ContainsKey (album) || albums[album].Cover == null) {
					albums[album] = new AlbumMusicItem (song.Album, song.Artist, song.Year, song.Cover);
				}
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
					.Where (song => song.Album == item.Name && song.Artist == item.Artist)
					.OrderBy (song => song.Disc)
					.ThenBy (song => song.Track);
			else
				return Enumerable.Empty<SongMusicItem> ();
		}

		static string ReadXdgUserDir (string key, string fallback)
		{
			string home_dir, config_dir, env_path, user_dirs_path;

			home_dir = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			config_dir = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);

			env_path = Environment.GetEnvironmentVariable (key);
			if (!String.IsNullOrEmpty (env_path)) {
				return env_path;
			}

			user_dirs_path = Path.Combine (config_dir, "user-dirs.dirs");
			if (!File.Exists (user_dirs_path)) {
				return Path.Combine (home_dir, fallback);
			}

			try {
				using (StreamReader reader = new StreamReader (user_dirs_path)) {
					string line;
					while ((line = reader.ReadLine ()) != null) {
						line = line.Trim ();
						int delim_index = line.IndexOf ('=');
						if (delim_index > 8 && line.Substring (0, delim_index) == key) {
							string path = line.Substring (delim_index + 1).Trim ('"');
							bool relative = false;

							if (path.StartsWith ("$HOME/")) {
								relative = true;
								path = path.Substring (6);
							} else if (path.StartsWith ("~")) {
								relative = true;
								path = path.Substring (1);
							} else if (!path.StartsWith ("/")) {
								relative = true;
							}
							return relative ? Path.Combine (home_dir, path) : path;
						}
					}
				}
			} catch (FileNotFoundException) {
			}
			return Path.Combine (home_dir, fallback);
		}

		public static IEnumerable<SongMusicItem> LoadAllSongs ()
		{
			// Begin a new timer to clear the songs SecondsSongsCached seconds from now.
			clear_songs_timer.Change (SecondsSongsCached*1000, Timeout.Infinite);
			
			if (songs.Any ()) return songs;
			
			// Song list is not cached. Load songs from database.
			try {
				XmlDocument xml = new XmlDocument ();
				using (XmlReader reader = XmlReader.Create (MusicLibraryFile)) {
					xml.Load (reader);
					XmlNodeList nodeList = xml.SelectNodes ("//entry");
					for (int i = 0; i < nodeList.Count; i++) {
						XmlNode node = nodeList.Item (i);

						if (!GetNodeText (node.Attributes ["type"]).Equals ("song"))
							continue;

						string song_name = GetNodeText (node.SelectSingleNode ("title"));
						string artist_name = GetNodeText (node.SelectSingleNode ("artist"));
						string album_name = GetNodeText (node.SelectSingleNode ("album"));
						string song_file = GetNodeText (node.SelectSingleNode ("location"));
						string year = GetNodeText (node.SelectSingleNode ("date"));

						int song_track = 0;
						Int32.TryParse (GetNodeText (node.SelectSingleNode ("track-number")), out song_track);

						int song_disc = 0;
						Int32.TryParse (GetNodeText (node.SelectSingleNode ("disc-number")), out song_disc);

						string cover = Path.Combine (CoverArtDirectory, string.Format ("{0} - {1}.jpg", artist_name, album_name)); 
						if (!File.Exists (cover))
							cover = null; 

						SongMusicItem song = new SongMusicItem (song_name, artist_name, album_name, year, cover, song_file, song_track, song_disc);
						songs.Add (song);
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Rhythmbox database file: " + e.Message);
			}
			return songs;
		}

		static string GetNodeText (XmlNode node)
		{
			if (node == null)
				return "";
			return node.InnerText;
		}

		public static void StartIfNeccessary ()
		{
			if (!InstanceIsRunning) {
				Process.Start ("rhythmbox-client", "--no-present");
				//System.Threading.Thread.Sleep (3 * 1000);
			}
		}

		public static bool InstanceIsRunning
		{
			get {
				try {
					// Use pidof command to look for Rhythmbox process. Exit
					// status is 0 if at least one matching process is found.
					// If there's any error, just assume some it's running.
					ProcessStartInfo pidof = new ProcessStartInfo ("pidof", "rhythmbox");
					pidof.UseShellExecute = false;
					pidof.RedirectStandardOutput = true;
					Process run = new Process ();
					run.StartInfo = pidof;
					run.Start ();
					run.WaitForExit ();
					return run.ExitCode == 0;
				} catch (Exception e) {
					Log.Error ("Could not determine if Rhythmbox is running: {0}", e.Message);
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
				Process client = Process.Start ("rhythmbox-client", command);
				if (wait) client.WaitForExit ();
			} catch {
			}
		}
	}
}
