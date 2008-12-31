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
 */

using System;
using System.Linq;
using System.Collections.Generic;

namespace Banshee
{
	public enum PlaybackShuffleMode
    {
        Linear,
        Song,
        Artist,
        Album
    }
	
	public static class Banshee
	{
		static BansheeDbus bus;
		static BansheeIndexer indexer;
		
		static Banshee()
		{
			bus = new BansheeDbus ();
			indexer = new BansheeIndexer ();
		}

		public static IEnumerable<T> LoadMedia (MediaItem item)
		{
			return Enumerable.Empty<MediaItem> ();
		}
		
		public void LoadVideos (out List<VideoItem> videosOut)
		{
			videosOut = new List<VideoItem> ();
			lock (indexer_mutex) {
				foreach (VideoItem video in videos) {
					videosOut.Add (video);
				}
			}
		}
		
		public void LoadPodcasts (out List<PodcastItem> podcastsOut)
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
		
		public void LoadAlbumsAndArtists (out List<AlbumMusicItem> albumsOut,
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
		
		public List<PodcastPodcastItem> LoadPodcastsFor (PodcastItem item)
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
		
		public List<SongMusicItem> LoadSongsFor (MusicItem item)
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
		
		public List<IItem> SearchMedia (string pattern)
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

		bool ContainsMatch (IItem item, string pattern)
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
