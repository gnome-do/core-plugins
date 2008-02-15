// Banshee.cs
//
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
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

using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Mono.Data.Sqlite;


namespace Do.Addins.Banshee {

        public class Banshee {

                static readonly string kMusicLibraryFile;
                static readonly string kCoverArtDirectory;
                static List<SongMusicItem> songs;
                
                static Timer clearSongsTimer;
                const int SecondsSongsCached = 45;

                static Banshee()
                {
                        string home;

                        home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
                        kMusicLibraryFile = "~/.config/banshee/banshee.db".Replace("~", home);
                        kCoverArtDirectory = "~/.config/banshee/covers".Replace("~", home);
                        
                        clearSongsTimer = new Timer (ClearSongs);
                        songs = new List<SongMusicItem> ();
                }
                
                private static void ClearSongs (object state)
                {
                        lock (songs) { songs.Clear(); }
                }

                public static void LoadAlbumsAndArtists (out List<AlbumMusicItem> albums_out,
                                                         out List<ArtistMusicItem> artists_out)
                {
                        Dictionary<string, AlbumMusicItem> albums;
                        Dictionary<string, ArtistMusicItem> artists;

                        albums_out = new List<AlbumMusicItem> ();
                        artists_out = new List<ArtistMusicItem> ();

                        albums = new Dictionary<string, AlbumMusicItem> ();
                        artists = new Dictionary<string, ArtistMusicItem> ();

                        foreach (SongMusicItem song in LoadAllSongs ()) {
                        // Don't let null covers replace non-null covers.
                                if (!artists.ContainsKey (song.Artist) || artists[song.Artist].Cover == null)
                                        artists[song.Artist] = new ArtistMusicItem (song.Artist,
                                                                                    song.Cover);

                                if (!albums.ContainsKey (song.Album) || albums[song.Album].Cover == null)
                                        albums[song.Album] = new AlbumMusicItem (song.Album,
                                                                                song.Artist,
                                                                                song.Year,
                                                                                song.Cover);
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
                                }
                                catch {
                                }
                        }
                        return new List<SongMusicItem> (songs.Values);
                }

                public static List<SongMusicItem> LoadAllSongs ()
                {
                        IDbConnection conn;
                        List<SongMusicItem> songsCopy;

                        lock (songs)
                        {
                                // Begin a new timer to clear the songs SecondsSongsCached seconds from now.
                                clearSongsTimer.Change (SecondsSongsCached*1000, Timeout.Infinite);
                                if (songs.Count == 0) {
                                        // Song list is not cached. Load songs from database.
                                        conn = new SqliteConnection("URI=file:" + kMusicLibraryFile);
                                        try {
                                                conn.Open();
                                                IDbCommand query = conn.CreateCommand();
                                                query.CommandText = "SELECT Title,AlbumTitle,Artist,Uri FROM Tracks";
                                                IDataReader reader = query.ExecuteReader();
                                                while (reader.Read()) {
                                                        try {
                                                                SongMusicItem song;
                                                                string song_name = reader.GetString(0);
                                                                string album_name = reader.GetString(1);
                                                                string artist_name = reader.GetString(2);
                                                                string song_file = reader.GetString(3);
                                                                string cover = null;
                                                                
                                                                if ((song_name == null || album_name == null) || song_file == null)
                                                                        continue;

                                                                cover = string.Format ("{0}-{1}.jpg",
                                                                                        artist_name.ToLower(),
                                                                                        album_name.ToLower());
                                                                cover = Path.Combine (kCoverArtDirectory, cover);

                                                                if (!File.Exists (cover))
                                                                        cover = null;

                                                                song = new SongMusicItem (song_name, artist_name, album_name,
                                                                                          String.Empty, cover, song_file);
                                                                songs.Add (song);
                                                        }
                                                        catch (Exception) {
                                                                //Console.Error.WriteLine (e.StackTrace);
                                                                //Console.Error.WriteLine ("Got Exception: {0}", e.Message);
                                                                continue;
                                                        }
                                                }
                                        }
                                        catch (Exception e) {
                                                //Console.Error.WriteLine (e.StackTrace);
                                                Console.Error.WriteLine ("Could not read Banshee database file: {0}", e.Message);
                                        }
                                        finally {
                                                if (conn.State == ConnectionState.Open)
                                                    conn.Close();
                                        }
                                }
                                songsCopy = new List<SongMusicItem> (songs);
                        }
                        return songsCopy;
                }
        }
}
