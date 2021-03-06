// Banshee.cs 
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using Do.Platform;

namespace Banshee
{

	public enum PlaybackShuffleMode
	{
		Linear,
		Song,
		Artist,
		Album
	}

	public class Banshee
	{
		static BansheeDBus bus;
		static Thread index_mutex;
		static BansheeIndexer indexer;
		
		static Banshee()
		{
			bus = new BansheeDBus ();
			indexer = new BansheeIndexer ();
			index_mutex = MakeIndexerThread ();
		}

		public static void Index ()
		{
			if (index_mutex.ThreadState == ThreadState.Running)
				return;
			else if (index_mutex.ThreadState == ThreadState.Unstarted)
				index_mutex.Start ();
			else {
				index_mutex = MakeIndexerThread ();
			}
		}

		public static bool IsPlaying {
			get { return bus.IsPlaying (); }
		}

		public static void Enqueue (MediaItem item)
		{
			Services.Application.RunOnThread (() => bus.Enqueue (LoadMedia (item)));
		}

		public static void Play ()
		{
			Services.Application.RunOnThread (bus.Play);
		}
		
		public static void Play (MediaItem item)
		{
			Services.Application.RunOnThread (() => bus.Play (LoadMedia (item)));
		}

		public static void Pause ()
		{
			Services.Application.RunOnThread (bus.Pause);
		}

		public static void Next ()
		{
			Services.Application.RunOnThread (bus.Next);
		}

		public static void Previous ()
		{
			Services.Application.RunOnThread (bus.Previous);
		}

		public static PlaybackShuffleMode ShuffleMode {
			set { bus.ShuffleMode = value; }
		}

		public static IEnumerable<IMediaFile> LoadMedia (MediaItem item)
		{
			if (item is MusicItem)
				return LoadSongsFor (item as MusicItem);
			else if (item is PodcastItem)
				return LoadPodcastsFor (item as PodcastItem);
			else if (item is VideoItem)
				return indexer.Videos.Cast<IMediaFile> ();
			else
				return Enumerable.Empty<IMediaFile> ();
		}

		public static void LoadVideos (out List<VideoItem> videos)
		{
			videos = new List<VideoItem> (indexer.Videos);
		}
		
		public static List<IMediaFile> SearchMedia (string pattern)
		{
			List<IMediaFile> results = new List<IMediaFile> ();

			results.AddRange (indexer.Songs.Where (item => ContainsMatch (item, pattern)).Cast<IMediaFile> ());
			results.AddRange (indexer.Videos.Where (item => ContainsMatch (item, pattern)).Cast<IMediaFile> ());
			results.AddRange (indexer.Podcasts.Where (item => ContainsMatch (item, pattern)).Cast<IMediaFile> ());

			return results;
		}

		public static void LoadPodcasts (out List<PodcastItem> podcastsOut)
		{
			Dictionary<string, PodcastItem> publishers;
			
			publishers = new Dictionary<string, PodcastItem> ();
		
			foreach (PodcastPodcastItem podcast in indexer.Podcasts) {
				if (!publishers.ContainsKey (podcast.Artist))
					publishers[podcast.Artist] = new PodcastPublisherItem (
						podcast.Artist, podcast.Year, podcast.Cover);
			}
			
			podcastsOut = new List<PodcastItem> (publishers.Values);
		}
		
		public static void LoadAlbumsAndArtists (out List<AlbumMusicItem> albumsOut, out List<ArtistMusicItem> artistsOut)
		{
			Dictionary<string, AlbumMusicItem>  albums;
			Dictionary<string, ArtistMusicItem> artists;
			
			albums = new Dictionary<string, AlbumMusicItem> ();
			artists = new Dictionary<string,ArtistMusicItem> ();
			
			foreach (SongMusicItem song in indexer.Songs) {
				if (!artists.ContainsKey (song.Artist))
					artists[song.Artist] = new ArtistMusicItem (song.Artist, song.Cover);
					
				if (!albums.ContainsKey (song.Album))
					albums[song.Album] = new AlbumMusicItem (song.Album, song.Artist, song.Year,
						song.Cover);
			}
			
			albumsOut = new List<AlbumMusicItem> (albums.Values);
			artistsOut = new List<ArtistMusicItem> (artists.Values);
		}

		public static IEnumerable<AlbumMusicItem> LoadAlbumsFor (ArtistMusicItem artist, IEnumerable<AlbumMusicItem> albums)
		{
			return albums.Where (album => album.Artist.Contains (artist.Name));
		}

		static List<IMediaFile> LoadSongsFor (MusicItem item)
		{
			SortedList<string, IMediaFile> albumSongs;
			string key;
			
			if (item is SongMusicItem) {
				List<IMediaFile> single = new List<IMediaFile> ();
				single.Add (item as SongMusicItem);
				return single;
			}
			
			albumSongs = new SortedList<string, IMediaFile> ();
			foreach (SongMusicItem song in indexer.Songs) {
				switch (item.GetType ().Name) {
				case "AlbumMusicItem":
					if (item.Name != song.Album) continue;
					break;
				case "ArtistMusicItem":
					if (item.Name != song.Artist) continue;
					break;
				}
				
				key = string.Format ("{0}-{1}-{2}", song.Album, song.Track, song.Path);
				try {
					if (albumSongs.ContainsKey (key)) continue;
					albumSongs.Add (key, song);
				} catch (Exception e) {
					Log.Error ("{0} : {1}", key, e.Message);
				}
			}
			
			return new List<IMediaFile> (albumSongs.Values);
		}

		static IEnumerable<IMediaFile> LoadPodcastsFor (PodcastItem item)
		{			
			if (item is PodcastPodcastItem) {
				yield return item as IMediaFile;
			}
			
			foreach (PodcastPodcastItem pc in indexer.Podcasts) {
				if ((item as PodcastPublisherItem).Name != pc.Artist) continue;
				
				yield return pc;
			}
		}


		static bool ContainsMatch (MediaItem item, string pattern)
		{
			return ContainsMatch (item, p => PropertyInfoMatchesPattern (item, p, pattern));
		}

		static bool ContainsMatch (MediaItem item, Func<PropertyInfo, bool> predicate)
		{
			return item.GetType ().GetProperties ().Any (predicate);
		}

		static bool PropertyInfoMatchesPattern (MediaItem item, PropertyInfo info, string pattern)
		{
			return (info.Name != "File" && (info.GetValue (item, null).ToString ().Contains (pattern)));
		}

		static Thread MakeIndexerThread ()
		{
			return new Thread ((ThreadStart) indexer.Start);
		}
	}
}
