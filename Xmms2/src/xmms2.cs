//  xmms2.cs
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
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

using Do.Universe;

namespace Do.Addins.xmms2 {

	public static class xmms2 {
		//coverart to be added later
		//static readonly string MusicLibraryFile;
		//static readonly string CoverArtDirectory;

		public static List<SongMusicItem> songs;
		public static List<PlaylistItem> playlists;
		
		static Timer clearSongsTimer;
		static Timer clearPlaylistsTimer;
		const int SecondsSongsCached = 45;

		static xmms2 (){
			//string home;

			//home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			//not needed and not implemented, respectively
			//MusicLibraryFile = "~/.gnome2/rhythmbox/rhythmdb.xml".Replace("~", home);
			//CoverArtDirectory = "~/.gnome2/rhythmbox/covers".Replace("~", home);

			clearSongsTimer = new Timer (ClearSongs);
			clearPlaylistsTimer = new Timer (ClearPlaylists);
			playlists = new List<PlaylistItem>();
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
			foreach (SongMusicItem song in LoadAllSongs ()) {	//this is genius, Mr. Siegel.
				// Don't let null covers replace non-null covers.
				if (!artists.ContainsKey (song.Artist) || artists[song.Artist].Cover == null) {
					artists[song.Artist] = new ArtistMusicItem (song.Artist, song.Cover);
				}
				if (!albums.ContainsKey (song.Album) || albums[song.Album].Cover == null) {
					albums[song.Album] = new AlbumMusicItem (song.Album, song.Artist, song.Cover);	
				}
			}
			albums_out.AddRange (albums.Values);
			artists_out.AddRange (artists.Values);
		}

		public static List<SongMusicItem> LoadSongsFor (MusicItem item)
		{
			if(item is SongMusicItem) {
				List<SongMusicItem> single = new List<SongMusicItem> ();
				single.Add (item as SongMusicItem);
				return single;
			}else if(item is AlbumMusicItem){
				List<SongMusicItem> songs = search(string.Format("album:\"{0}\"*", item.Name)); //cuts down on extraneous search results, more efficient
				songs.Sort();
				return songs;
			}else if(item is ArtistMusicItem){
				List<SongMusicItem> songs = search(string.Format("artist:\"{0}\"*", item.Name));
				songs.Sort();
				return songs;
			}
			return new List<SongMusicItem>();

//			songs = new SortedList<SongMusicItem, SongMusicItem> ();
//			foreach (SongMusicItem song in LoadAllSongs ()) { //this is slowish, and causes O(songs) complexity, instead of O(albumlength) or O(SongsbyArtist) 
//				switch (item.GetType ().Name) {
//					case "AlbumMusicItem":
//						if (item.Name != song.Album) continue;
//					break;
//					case "ArtistMusicItem":
//						if (item.Name != song.Artist) continue;
//					break;
//				}
//				try {
//					songs.Add (song, song);
//				} catch { }
//			}
//			return new List<SongMusicItem> (songs.Values);
		}
		
		public static List<SongMusicItem> LoadSongsFor (PlaylistItem list){
			string line;
			string[] delimarr = new string[] {" - ",};
			string[] data;
			int id;
			SongMusicItem entry;
			List<SongMusicItem> entries = new List<SongMusicItem>();
			
			System.Diagnostics.Process getList;
			getList = new System.Diagnostics.Process();
			getList.StartInfo.FileName = "xmms2";
			getList.StartInfo.Arguments = string.Format("list {0}", list.Name); //Warning: does not guard against improperly formatted queries
			getList.StartInfo.RedirectStandardOutput = true;
			getList.StartInfo.UseShellExecute = false;
			getList.Start();
			while(null != (line = getList.StandardOutput.ReadLine())){
				try{
					if(line.Substring(2,1) == "["){
						line = Regex.Replace(line, "..\\[.+?\\/", ""); //clears out beginning of line format
						line = Regex.Replace(line, "\\]", " -"); //clears the other ]
						data = line.Split(delimarr, StringSplitOptions.None);
						id = int.Parse(data[0]);
						data[2] = Regex.Replace(data[2], "[^\\w\\s\\d].+$", "");
						entry = new SongMusicItem(id, data[1], "", data[2]); //blank album to match default output.  phooey.
					 entries.Add(entry);
					}
				}catch(Exception e){
					Console.Error.WriteLine("failed to load playlist: " + e.Message + " at " + line);
				}
			}
			return entries;
			
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
			
			lock(songs){
				// Begin a new timer to clear the songs SecondsSongsCached seconds from now.
				clearSongsTimer.Change (SecondsSongsCached*1000, Timeout.Infinite);
				
				if (songs.Count == 0) {
					// Song list is not cached. Load songs from output of the search
					songs = search("title:*");
				}
				songsCopy = new List<SongMusicItem> (songs);
			}
			return songsCopy;
		}
		
		private static void ClearPlaylists (object state)
		{
			lock(playlists){
				playlists.Clear();
			}
		}
		
		public static List<PlaylistItem> LoadAllPlaylists(){
			string line;
			List<PlaylistItem> playlistsCopy;
			PlaylistItem item;
			System.Diagnostics.Process getPla;
			getPla = new System.Diagnostics.Process();
			getPla.StartInfo.FileName = "xmms2";
			getPla.StartInfo.Arguments = "playlist list";
			getPla.StartInfo.RedirectStandardOutput = true;
			getPla.StartInfo.UseShellExecute = false;
			getPla.Start();
			lock(playlists){
				if (playlists.Count == 0) {
					clearPlaylistsTimer.Change (SecondsSongsCached*1000, Timeout.Infinite);
					while(null != (line = getPla.StandardOutput.ReadLine())){
						item = new PlaylistItem(line.Substring(2));
						if(line.Substring(0,2) == "->"){
							item.current = true;
						}
						playlists.Add(item);
					}
				}
				playlistsCopy = new List<PlaylistItem>(playlists);
			}
			return playlistsCopy;
		}
		
		public static List<SongMusicItem> search(string query){ //queries should be in the form <field>:\"<name>\"
			
			List<SongMusicItem> songs = new List<SongMusicItem>();
			
			string line;
			string[] data;
			string[] delimarr = new string[] {"| ",};
			SongMusicItem song;
		
			try{

				System.Diagnostics.Process getLib;
				getLib = new System.Diagnostics.Process();
				getLib.StartInfo.FileName = "xmms2";
				getLib.StartInfo.Arguments = string.Format("mlib search {0}", query);
				getLib.StartInfo.RedirectStandardOutput = true;
				getLib.StartInfo.UseShellExecute = false;
				getLib.Start();
				getLib.StandardOutput.ReadLine(); //get rid of extraneous
				getLib.StandardOutput.ReadLine(); //lines at the top
				while(null != (line = getLib.StandardOutput.ReadLine())){
					if(line.Substring(0,1) != "-"){
						data = line.Split(delimarr, StringSplitOptions.RemoveEmptyEntries);
						int id = int.Parse(data[0]);
						for(int i = 1; i <= 3; i++){
							data[i] = Regex.Replace(data[i], " +$", "");//keeps formatting right, also ensures searches actually work!
						}
						song = new SongMusicItem(id, data[1], data[2], data[3]);
						songs.Add(song);
					}
				}
			
			} catch (Exception e) {
				Console.Error.WriteLine ("xmms2 mlib search failed: " + e.Message + " query was " + query);
			}
			return songs;
		}
		
		public static void StartIfNeccessary ()
		{
			if (!InstanceIsRunning) {
				Process.Start ("xmms2d");
				System.Threading.Thread.Sleep (3 * 1000);
			}
		}

		public static bool InstanceIsRunning
		{
			get {
				Process pidof;

				try {
					// Use pidof command to look for xmms2d process. Exit
					// status is 0 if at least one matching process is found.
					// If there's any error, just assume some it's running.
					pidof = Process.Start ("pidof", "xmms2d");
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
				client = Process.Start ("xmms2", command);
				if (wait)
					client.WaitForExit ();
			} catch {
			}
		}
	}
}
