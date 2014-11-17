//  RhythmboxItemSource.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
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
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace Do.Rhythmbox
{
	public class MusicItemSource : ItemSource
	{
		List<Item> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;
		List<SongMusicItem> songs;
		List<PlaylistMusicItem> playlists;

		public MusicItemSource ()
		{
			items = new List<Item> ();
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Rhythmbox Music"); }
		}
		
		public override string Description { 
			get { return AddinManager.CurrentLocalizer.GetString ("Provides access to artists and albums from Rhythmbox."); }
		}
		
		public override string Icon {
			get { return "rhythmbox"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (MusicItem);
				yield return typeof (BrowseMusicItem);
				yield return typeof (IApplicationItem);
			}
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}

		bool IsRhythmbox (Item item)
		{
			return item is IApplicationItem && IsRhythmbox (item as IApplicationItem);
		}

		bool IsRhythmbox (IApplicationItem item)
		{
			return item.Exec.Contains ("rhythmbox");
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent) {
			if (IsRhythmbox (parent)) {
				yield return new BrowseAlbumsMusicItem ();
				yield return new BrowseArtistsMusicItem ();
				if (playlists.Count > 0)
					yield return new BrowsePlaylistsMusicItem ();
				yield return new BrowseSongsMusicItem ();
				foreach (Item item in RhythmboxRunnableItem.Items)
					yield return item;
			} else if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in albums.Where (album => album.Artist.Contains (parent.Name)))
					yield return album;
			} else if (parent is AlbumMusicItem) {
				foreach (SongMusicItem song in Rhythmbox.LoadSongsFor (parent as AlbumMusicItem))
					yield return song;
			} else if (parent is BrowseAlbumsMusicItem) {
				foreach (AlbumMusicItem album in albums)
					yield return album;
			} else if (parent is BrowseArtistsMusicItem) {
				foreach (ArtistMusicItem artist in artists)
					yield return artist;
			} else if (parent is BrowseSongsMusicItem) {
				foreach (SongMusicItem song in songs)
					yield return song;
			} else if (parent is BrowsePlaylistsMusicItem) {
				foreach (PlaylistMusicItem playlist in playlists)
					yield return playlist;
			}
		}

		public override void UpdateItems ()
		{
			items.Clear ();

			// Add volume and display controls.
			foreach (Item item in RhythmboxRunnableItem.Items)
				items.Add (item);

			// Add music data.
			Rhythmbox.LoadMusicData (out albums, out artists, out songs, out playlists);
			foreach (Item album in albums) items.Add (album);
			foreach (Item artist in artists) items.Add (artist);
			foreach (Item song in songs) items.Add (song);
			foreach (Item playlist in playlists) items.Add (playlist);

			// Add browse features.
			items.Add (new BrowseAlbumsMusicItem ());
			items.Add (new BrowseArtistsMusicItem ());
			items.Add (new BrowseSongsMusicItem ());
			if (playlists.Count > 0) {
				// If Rhythmbox isn't running, or if there are DBus issues,
				// we don't want the "Browse by playlist" item to show up and be useless.
				// Instead, we only show it when it's populated.
				// Artists, albums and songs are fetched via XML and should never fail.
				items.Add (new BrowsePlaylistsMusicItem ());
			}
		}
	}
}
