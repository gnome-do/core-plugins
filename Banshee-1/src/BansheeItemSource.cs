/* BansheeItemSource.cs
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
using System.Collections.Generic;

using Do.Addins;
using Do.Universe;
using Mono.Unix;

namespace Banshee1
{	
	public class BansheeItemSource : IItemSource
	{
		List<IItem> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;
		List<VideoItem> videos;
		List<PodcastItem> publishers;
		
		public BansheeItemSource()
		{
			items = new List<IItem> ();
			Banshee indexer = new Banshee ();
			indexer.Start ();
		}
		
		public string Name {
			get { return "Banshee Media"; }
		}
		
		public string Description {
			get { return "Indexes Media from Banshee Media Player"; }
		}
		
		public string Icon {
			get { return "music-player-banshee"; }
		}
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (MediaItem),
					typeof (BrowseMediaItem),
					typeof (ApplicationItem),
				};
			}
		}
		
		public IEnumerable<IItem> Items {
			get { return items; }
		}
		
		public IEnumerable<IItem> ChildrenOfItem (IItem parent)
		{
			List<IItem> children;
			
			children = new List<IItem> ();

			if (parent is ApplicationItem && parent.Name.ToLower () == Catalog.GetString ("banshee media player")) {
				if (albums != null && albums.Count > 0)
					children.Add (new BrowseAlbumsMusicItem ());
				if (artists != null && artists.Count > 0)
					children.Add (new BrowseArtistMusicItem ());
				if (videos != null && videos.Count > 0)
					children.Add (new BrowseVideoItem ());
				if (publishers != null && publishers.Count > 0)
					children.Add (new BrowsePublisherPodcastItem ());
				
				children.AddRange (BansheeRunnableItem.DefaultItems);
			}
			else if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in AllAlbumsBy (parent as ArtistMusicItem))
					children.Add (album);
			}
			else if (parent is AlbumMusicItem) {
				foreach (SongMusicItem song in Banshee.LoadSongsFor (parent as AlbumMusicItem))
					children.Add (song);
			}
			else if (parent is PodcastPublisherItem) {
				foreach (PodcastPodcastItem pc in Banshee.LoadPodcastsFor (parent as PodcastPublisherItem))
					children.Add (pc);
			}
			else if (parent is BrowsePublisherPodcastItem) {
				foreach (PodcastItem podcast in publishers)
					children.Add (podcast);
			}
			else if (parent is BrowseVideoItem) {
				foreach (VideoItem video in videos)
					children.Add (video);
			}
			else if (parent is BrowseAlbumsMusicItem) {
				foreach (AlbumMusicItem album in albums)
					children.Add (album);
			}
			else if (parent is BrowseArtistMusicItem) {
				foreach (ArtistMusicItem artist in artists)
					children.Add (artist);
			}
			
			return children;
		}
		
		public void UpdateItems ()
		{
			//if (!Banshee.HasCollectionChanged) return;
			
			items.Clear ();
			
			//Add browser features
			items.Add (new BrowseAlbumsMusicItem ());
			items.Add (new BrowseArtistMusicItem ());
			items.Add (new BrowsePublisherPodcastItem ());
			items.Add (new BrowseVideoItem ());
			items.AddRange (BansheeRunnableItem.DefaultItems);
			
			//Add albums and artists to the universe
			Banshee.LoadAlbumsAndArtists  (out albums, out artists);
			Banshee.LoadVideos (out videos);
		 	Banshee.LoadPodcasts (out publishers);
		
			foreach (IItem album in albums) items.Add (album);
			foreach (IItem artist in artists) items.Add (artist);
			foreach (IItem video in videos) items.Add (video);
			foreach (IItem podcast in publishers) items.Add (podcast);
			
			Banshee.HasCollectionChanged = false;
		}
		
		protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
		{
			return albums.FindAll (delegate (AlbumMusicItem album) {
				return album.Artist == artist.Name;
			});
		}
	}
}
