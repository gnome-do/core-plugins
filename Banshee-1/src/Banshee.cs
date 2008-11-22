/* Banshee.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * THE PHILLIES WON TONIGHT! WE'RE GOING TO THE WORLD SERIES!!!!!
 */


using System;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using Banshee.Collection.Indexer.RemoteHelper;

using Do.Addins;
using Do.Universe;
using Mono.Unix;

namespace Banshee1
{
	public class Banshee : SimpleIndexerClient
	{
		static List<IItem> songs;
		static List<IItem> videos;
		static List<IItem> podcasts;
		static object indexer_mutex;
		
		static DateTime last_index;
		static string artwork_directory;
		static bool reindex;
		
		readonly string[] export_fields = new string[] {"name", "artist", "year", "album", "local-path", "URI",
			"media-attributes", "artwork-id", "track-number"};
		
		static Banshee ()
		{
			songs = new List<IItem> ();
			videos = new List<IItem> ();
			podcasts = new List<IItem> ();
			indexer_mutex = new object ();
			last_index = DateTime.MinValue;
			reindex = true;
			
			artwork_directory = Do.Paths.Combine (
				Do.Paths.ReadXdgUserDir ("XDG_CACHE_DIR", ".cache"), "album-art");
		}
		
		public Banshee ()
		{
			AddExportField (export_fields);
		}
		
		public static bool HasCollectionChanged {
			get { return reindex; }
			set { reindex = value; }
		}
		protected override void IndexResult (IDictionary<string, object> result)
		{
			try {
				MediaItem item;
				object path;
				object artwork;
				string artPath;
				object year;
				object trackNum;
								
				//we should always use local-path first, if that does not exist,
				//then we should use the URI value.
				if (!result.TryGetValue ("local-path", out path))
					path = result["URI"];

				if (result.TryGetValue ("year", out year))
					year = year.ToString ();
					
				if (result.TryGetValue ("track-number", out trackNum))
					trackNum = trackNum.ToString (); 
				
				//Videos do not have artwork ids, so we need to handle them specially.
				artPath = null;
				if (result.TryGetValue ("artwork-id", out artwork))
					artPath = Do.Paths.Combine (artwork_directory, result["artwork-id"].ToString () + ".jpg");
				
				//We create a type for each type of media and stick it into our
				//list of media items. The list shouldn't be so big that this makes
				//all other actions slow
				
				//Handle videos in the collection
				if (result["media-attributes"].ToString ().Contains ("VideoStream")) {		
					item = new VideoItem (result["name"].ToString (), result["artist"].ToString (),
						((string) year ?? null), artPath, path.ToString ());

					lock (indexer_mutex)
						videos.Add (item);
					
				//Handle the podcasts in collection
				} else if (result["media-attributes"].ToString ().Contains ("Podcast")) {
					item = new PodcastPodcastItem (result["name"].ToString (),
						result["album"].ToString (), ((string) year ?? null), artPath, path.ToString ());
					
					lock (indexer_mutex)
						podcasts.Add (item);
				
				//everything else should be Music, so we index it!
				} else {
					item = new SongMusicItem (result["name"].ToString (),
						result["artist"].ToString (), result["album"].ToString (),
						((string) year ?? null), artPath, ((string) trackNum ?? null), path.ToString ());
					
					lock (indexer_mutex)
						songs.Add (item);
				}
			} catch (KeyNotFoundException e) {
				Log.Error ("{0}", e.Message);
			}
		}
		
		protected override int CollectionCount {
			get { 
				lock (indexer_mutex) {
					return songs.Count + videos.Count + podcasts.Count;
				}
			}
		}
		
		protected override DateTime CollectionLastModified {
			get { return last_index; }
		}
		
		protected override void OnStarted ()
		{
			lock (indexer_mutex) {
				songs.Clear ();
				podcasts.Clear ();
				videos.Clear ();
			}
			
			reindex = true;
		}

		protected override void OnShutdownWhileIndexing ()
		{
			Log.Info ("Banshee requested a shutdown. Stopping indexing");
		}
		
		public static void LoadVideos (out List<VideoItem> videosOut)
		{
			videosOut = new List<VideoItem> ();
			lock (indexer_mutex) {
				foreach (VideoItem video in videos) {
					videosOut.Add (video);
				}
			}
		}
		
		public static void LoadPodcasts (out List<PodcastItem> podcastsOut)
		{
			Dictionary<string, PodcastItem> publishers;
			
			podcastsOut = new List<PodcastItem> ();
			publishers = new Dictionary<string, PodcastItem> ();
			
			lock (indexer_mutex) {
				foreach (PodcastPodcastItem podcast in podcasts) {
					if (!publishers.ContainsKey (podcast.Artist))
						publishers[podcast.Artist] = new PodcastPublisherItem (
							podcast.Artist, podcast.Year, podcast.Cover);
				}
			}
			
			podcastsOut.AddRange (publishers.Values);
		}
		
		public static void LoadAlbumsAndArtists (out List<AlbumMusicItem> albumsOut,
			out List<ArtistMusicItem> artistsOut)
		{
			Dictionary<string, AlbumMusicItem>  albums;
			Dictionary<string, ArtistMusicItem> artists;
			
			albumsOut = new List<AlbumMusicItem> ();
			artistsOut = new List<ArtistMusicItem> ();
			
			albums = new Dictionary<string, AlbumMusicItem> ();
			artists = new Dictionary<string,ArtistMusicItem> ();
			
			lock (indexer_mutex) {
				foreach (SongMusicItem song in songs) {
					if (!artists.ContainsKey (song.Artist))
						artists[song.Artist] = new ArtistMusicItem (song.Artist, song.Cover);
						
					if (!albums.ContainsKey (song.Album))
						albums[song.Album] = new AlbumMusicItem (song.Album, song.Artist, song.Year,
							song.Cover);
				}
			}
			
			albumsOut.AddRange (albums.Values);
			artistsOut.AddRange (artists.Values);
		}
		
		public static List<PodcastPodcastItem> LoadPodcastsFor (PodcastItem item)
		{
			List<PodcastPodcastItem> feedItems;
			
			if (item is PodcastPodcastItem) {
				List<PodcastPodcastItem> podcast = new List<PodcastPodcastItem> ();
				podcast.Add (item as PodcastPodcastItem);
				return podcast;
			}
			
			feedItems = new List<PodcastPodcastItem> ();
			feedItems.Clear ();
			lock (indexer_mutex) {
				foreach (PodcastPodcastItem pc in podcasts) {
					if ((item as PodcastPublisherItem).Name != pc.Artist) continue;
					
					try {
						feedItems.Add (pc);
					} catch (Exception e) {
						Log.Error ("{0}", e.Message);
					}
				}
			}
			
			return feedItems;
		}
		
		public static List<SongMusicItem> LoadSongsFor (MusicItem item)
		{
			SortedList<string, SongMusicItem> albumSongs;
			string key;
			
			if (item is SongMusicItem) {
				List<SongMusicItem> single = new List<SongMusicItem> ();
				single.Add (item as SongMusicItem);
				return single;
			}
			
			albumSongs = new SortedList<string,SongMusicItem> ();
			lock (indexer_mutex) {
				foreach (SongMusicItem song in songs) {
					switch (item.GetType ().Name) {
					case "AlbumMusicItem":
						if (item.Name != song.Album) continue;
						break;
					case "ArtistMusicItem":
						if (item.Name != song.Artist) continue;
						break;
					}
					
					key = string.Format ("{0}-{1}-{2}", song.Album, song.Track, song.File);
					try {
						if (albumSongs.ContainsKey (key)) continue;
						albumSongs.Add (key, song);
					} catch (Exception e) {
						Log.Error ("{0} : {1}", key, e.Message);
					}
				}
			}
			
			return new List<SongMusicItem> (albumSongs.Values);
		}
		
		public static List<IItem> SearchMedia (string pattern)
		{
			Console.Error.WriteLine ("Into SM");
			List<IItem> results = new List<IItem> ();
			
			lock (indexer_mutex) {
				if (songs != null) {
					results.AddRange (songs.FindAll (delegate (IItem item) {
						return ContainsMatch (item, pattern);
					}));
				}
			
				if (videos != null) {
					results.AddRange (videos.FindAll (delegate (IItem item) {
						return ContainsMatch (item, pattern);
					}));
				}
			
				if (podcasts != null) {
					results.AddRange (podcasts.FindAll (delegate (IItem item) {
						return ContainsMatch (item, pattern);
					}));
				}
			}
			
			Console.Error.WriteLine ("OUT");
			return results;
		}
		
		static bool ContainsMatch (IItem item, string pattern)
		{
			foreach (PropertyInfo p in item.GetType ().GetProperties ()) {
				try {
					if (p.Name != "File" && (p.GetValue (item, null).ToString ().Contains (pattern)))
						return true;
				} catch { }
			}
			
			return false;
		}
	}
}