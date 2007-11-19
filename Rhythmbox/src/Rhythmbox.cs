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
			XmlDocument db;
			Dictionary<string, AlbumMusicItem> albums;
			Dictionary<string, ArtistMusicItem> artists;
			
			albums_out = new List<AlbumMusicItem> ();
			artists_out = new List<ArtistMusicItem> ();
			
			db = new XmlDocument ();
			albums = new Dictionary<string, AlbumMusicItem> ();
			artists = new Dictionary<string, ArtistMusicItem> ();
			try {
				db.Load (kMusicLibraryFile);
				foreach (XmlNode entry in db.GetElementsByTagName ("entry")) {
					string song_name, album_name, artist_name, year, cover;
					
					song_name = album_name = artist_name = year = cover = null;
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
							// track_file = song_attr.InnerText;
							break;
						}
					}
					if ((song_name ?? album_name) == null) continue;
					
					cover = string.Format ("{0} - {1}.jpg", artist_name, album_name);
					cover = Path.Combine (kCoverArtDirectory, cover);
					if (!File.Exists (cover)) cover = null;

					albums[album_name] = new AlbumMusicItem (album_name, artist_name, year, cover);
					artists[artist_name] = new ArtistMusicItem (artist_name, cover);
				}
				albums_out.AddRange (albums.Values);
				artists_out.AddRange (artists.Values);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Rhythmbox database file: " + e.Message);
			}
		}
		
		public static IList<TrackMusicItem> TracksFor (MusicItem item)
		{
			XmlDocument db;
			SortedList<string, TrackMusicItem> tracks;
			
			if (item is TrackMusicItem) {
				List<TrackMusicItem> single = new List<TrackMusicItem> ();
				single.Add (item as TrackMusicItem);
				return single;
			}
			
			db = new XmlDocument ();
			tracks = new SortedList<string, TrackMusicItem> ();
			try {
				db.Load (kMusicLibraryFile);
				foreach (XmlNode entry in db.GetElementsByTagName ("entry")) {
					string track_file, song_name, album_name, artist_name, year, cover;
					
					track_file = song_name = album_name = artist_name = year = cover = null;
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
							track_file = song_attr.InnerText;
							break;
						}
					}
					if (song_name == null) continue;
					
					cover = string.Format ("{0} - {1}.jpg", artist_name, album_name);
					cover = Path.Combine (kCoverArtDirectory, cover);
					if (!File.Exists (cover)) cover = null;

					if ((item is AlbumMusicItem && (item as AlbumMusicItem).Name == album_name) ||
					    (item is ArtistMusicItem && (item as ArtistMusicItem).Artist == artist_name)) {
						TrackMusicItem track;
						
						track = new TrackMusicItem (song_name, artist_name, album_name, year, cover, track_file);
						try {
							tracks.Add (track_file, track);
						} catch { }
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Rhythmbox database file: " + e.Message);
			}
			return tracks.Values;
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
