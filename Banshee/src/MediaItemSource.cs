/* MediaItemSource.cs
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

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Banshee
{	
	public class MediaItemSource : ItemSource
	{
		List<Item> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;
		List<VideoItem> videos;
		List<PodcastItem> publishers;
		
		public MediaItemSource()
		{
			if (!Util.VersionSupportsIndexing ()) {
				Log<Banshee>.Error (Util.UnsupportedVersionMessage);
				throw new Exception (Util.UnsupportedVersionMessage);
			}
				
			items = new List<Item> ();
		}
		
		public override string Name {
			get { return Catalog.GetString ("Banshee Media"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Indexes Media from Banshee Media Player"); }
		}
		
		public override string Icon {
			get { return "music-player-banshee"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (MediaItem);
				yield return typeof (BrowseMediaItem);
				yield return typeof (IApplicationItem);
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return items; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			List<Item> children;
			
			children = new List<Item> ();

			if (parent is IApplicationItem && (parent as IApplicationItem).Exec.Contains ("banshee-1")) {
				if (albums != null && albums.Count > 0)
					children.Add (new BrowseAlbumsMusicItem ());
				if (artists != null && artists.Count > 0)
					children.Add (new BrowseArtistMusicItem ());
				if (videos != null && videos.Count > 0)
					children.Add (new BrowseVideoItem ());
				if (publishers != null && publishers.Count > 0)
					children.Add (new BrowsePublisherPodcastItem ());
			}
			else if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in AllAlbumsBy (parent as ArtistMusicItem))
					children.Add (album);
			}
			else if (parent is AlbumMusicItem) {
				foreach (SongMusicItem song in Banshee.LoadMedia (parent as AlbumMusicItem))
					children.Add (song);
			}
			else if (parent is PodcastPublisherItem) {
				foreach (PodcastPodcastItem pc in Banshee.LoadMedia (parent as PodcastPublisherItem))
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
		
		public override void UpdateItems ()
		{
			items.Clear ();
			Banshee.Index ();

			Banshee.LoadVideos (out videos);
			Banshee.LoadPodcasts (out publishers);
			Banshee.LoadAlbumsAndArtists (out albums, out artists);
			
			foreach (Item video in videos) items.Add (video);
			foreach (Item album in albums) items.Add (album);
			foreach (Item artist in artists) items.Add (artist);
			foreach (Item podcast in publishers) items.Add (podcast);
		}
		
		protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
		{
			return albums.FindAll (delegate (AlbumMusicItem album) {
				return album.Artist == artist.Name;
			});
		}
	}
}
