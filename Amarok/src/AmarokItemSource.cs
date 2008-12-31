//  AmarokItemSource.cs
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
using System.Collections.Generic;

using Do.Plugins;
using Do.Universe;
using Mono.Unix;

namespace Do.Plugins.Amarok
{

	public class AmarokMusicItemSource : ItemSource
	{
		List<Item> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;

		public AmarokMusicItemSource ()
		{
			items = new List<Item> ();
		}

		public override string Name { get { return Catalog.GetString ("Amarok Music"); } }
		public override string Description { 
			get { return Catalog.GetString ("Provides access to artists and albums from Amarok."); } 
		}
		public override string Icon { get { return "amarok"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (MusicItem);
				yield return typeof (BrowseMusicItem);
				yield return typeof (IApplicationItem);
			}
		}

		public override IEnumerable<Item> Items { get { return items; } }

		public override IEnumerable<Item> ChildrenOfItem (Item parent) {
			List<Item> children;

			children = new List<Item> ();
			if (parent is IApplicationItem && parent.Name.Contains ("Amarok")) {
				children.Add (new BrowseAlbumsMusicItem ());
				children.Add (new BrowseArtistsMusicItem ());
				children.AddRange (AmarokRunnableItem.DefaultItems);
			}
			else if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in AllAlbumsBy (parent as ArtistMusicItem))
					children.Add (album);
			}
			else if (parent is AlbumMusicItem) {
				foreach (SongMusicItem song in Amarok.LoadSongsFor (parent as AlbumMusicItem))
					children.Add (song);
			}
			else if (parent is BrowseAlbumsMusicItem) {
				foreach (AlbumMusicItem album in albums)
					children.Add (album);
			}
			else if (parent is BrowseArtistsMusicItem) {
				foreach (ArtistMusicItem album in artists)
					children.Add (album);
			}
			return children;
		}

		public void UpdateItems ()
		{
			items.Clear ();

			// Add play, pause, etc. controls.
			items.AddRange (AmarokRunnableItem.DefaultItems);

			// Add browse features.
			items.Add (new BrowseAlbumsMusicItem ());
			items.Add (new BrowseArtistsMusicItem ());

			// Add albums and artists.
			Amarok.LoadAlbumsAndArtists (out albums, out artists);
			foreach (Item album in albums) items.Add (album);
			foreach (Item artist in artists) items.Add (artist);
		}

		protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
		{
			return albums.FindAll (delegate (AlbumMusicItem album) {
				return album.Artist == artist.Name;
			});
		}
	}
}
