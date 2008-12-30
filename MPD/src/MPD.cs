//  MPD.cs
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
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace MPD
{
	public static class MPD
	{
		static readonly string CoverArtDirectory;

		static List<SongMusicItem> songs;

		static Timer clearSongsTimer;
		const int SecondsSongsCached = 90;

		static MPD ()
		{
			/*
			 * MPD doesn't by itself collect cover art.  However, most of MPD's clients
			 * do.  GMPC stores them in ~/.covers.  We should add more logic to
			 * check other locations as well.  This works perfectly for me.
			 */
			
			String home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			CoverArtDirectory = "~/.covers".Replace("~", home);
			clearSongsTimer = new Timer (ClearSongs);
			songs = new List<SongMusicItem> ();
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
				if (!artists.ContainsKey (song.Artist.ToLower()) || artists[song.Artist.ToLower()].Cover == null) {
					artists[song.Artist.ToLower()] = new ArtistMusicItem (song.Artist, song.Cover);
				}
				if (!albums.ContainsKey (song.Album.ToLower()) || albums[song.Album.ToLower()].Cover == null) {
					albums[song.Album.ToLower()] = new AlbumMusicItem (song.Album, song.Artist, song.Year, song.Cover);	
				}
			}
			albums_out.AddRange (albums.Values);
			artists_out.AddRange (artists.Values);
		}

		public static List<SongMusicItem> LoadSongsFor (MusicItem item)
		{
			SortedList<string, SongMusicItem> songs;		
			
			//case where we're loading all the songs for a given song
			//is trivially just that one song
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
					songs.Add (song.Name, song);	
				} catch { }
			}
			List<SongMusicItem> newlist = new List<SongMusicItem>();
			foreach(SongMusicItem song in songs.Values){
				newlist.Add(song);
			}
			return newlist;
		}

		private static void ClearSongs (object state)
		{
			lock (songs) {
				songs.Clear ();
			}
		}

		public static List<SongMusicItem> LoadAllSongs ()
		{
			List<SongMusicItem> songsCopy;
			lock (songs) {
				// Begin a new timer to clear the songs SecondsSongsCached seconds from now.
				clearSongsTimer.Change (SecondsSongsCached*1000, Timeout.Infinite);
				if (songs.Count == 0) {
					// Song list is not cached. Load songs from database.
					try { 
						Process proc = new Process();
						proc.StartInfo.FileName = "/usr/bin/mpc"; 
						proc.StartInfo.Arguments =@"playlist --format "":%title%:%artist%:%album%:%file%""";
						proc.StartInfo.UseShellExecute=false;
						proc.StartInfo.RedirectStandardOutput = true;
						proc.Start();
						String line = proc.StandardOutput.ReadLine();
						while(line != null){
							string[] info = line.Split(':'); 							
							SongMusicItem song;
							string song_file, song_name, album_name, artist_name,cover;							
							int number = Convert.ToInt32(info[0].Substring(1,info[0].IndexOf(')') -1 ));
							song_name = info[1];													
							artist_name = info[2];						
							album_name = info[3];
							song_file = info[4];		
							string cover_name_artist = artist_name;
							string cover_name_album = album_name;
							//Try the album art first, then the artist art
							cover_name_artist = cover_name_artist.Replace(" ","%20");
							cover_name_album = cover_name_album.Replace(" ","%20");
							cover = string.Format ("{0}-{1}.jpg", cover_name_artist, cover_name_album);
							cover = Path.Combine (CoverArtDirectory, cover);
							if(!File.Exists (cover)){
								cover = string.Format ("{0}.jpg", cover_name_artist);
								cover = Path.Combine (CoverArtDirectory, cover);
								if (!File.Exists (cover)) cover = null;
							}
							song = new SongMusicItem (song_name, artist_name, album_name, cover,song_file,  number);
							songs.Add (song);
							line = proc.StandardOutput.ReadLine();
						}				
					} catch (Exception e) {
						Console.Error.WriteLine ("Could not read MPD database file: " + e.Message);
					}
				}
				songsCopy = new List<SongMusicItem> (songs);
			}
			return songsCopy;
		}

		public static void Client (string command)
		{	
			try {
				Process.Start ("mpc", command);
			} catch {
			}
		}
	}
}
