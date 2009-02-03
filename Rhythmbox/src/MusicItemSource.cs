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

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Do.Rhythmbox
{

	public class MusicItemSource : ItemSource
	{
		List<Item> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;

		public MusicItemSource ()
		{
			items = new List<Item> ();
		}

		public override string Name {
			get { return Catalog.GetString ("Rhythmbox Music"); }
		}
		
		public override string Description { 
			get { return Catalog.GetString ("Provides access to artists and albums from Rhythmbox."); }
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
				foreach (Item item in RhythmboxRunnableItem.Items)
					yield return item;
			}
			else if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in albums.Where (album => album.Artist.Contains (parent.Name)))
					yield return album;
			}
			else if (parent is AlbumMusicItem) {
				foreach (SongMusicItem song in Rhythmbox.LoadSongsFor (parent as AlbumMusicItem))
					yield return song;
			}
			else if (parent is BrowseAlbumsMusicItem) {
				foreach (AlbumMusicItem album in albums)
					yield return album;
			}
			else if (parent is BrowseArtistsMusicItem) {
				foreach (ArtistMusicItem artist in artists)
					yield return artist;
			}
		}

		public override void UpdateItems ()
		{
			items.Clear ();

			// Add volume and display controls.
			foreach (Item item in RhythmboxRunnableItem.Items)
				items.Add (item);

			// Add browse features.
			items.Add (new BrowseAlbumsMusicItem ());
			items.Add (new BrowseArtistsMusicItem ());

			// Add albums and artists.
			Rhythmbox.LoadAlbumsAndArtists (out albums, out artists);
			foreach (Item album in albums) items.Add (album);
			foreach (Item artist in artists) items.Add (artist);
		}
	}
}
