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

namespace Do.Plugins.Amarok
{

	public class AmarokMusicItemSource : IItemSource
	{
		List<IItem> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;

		public AmarokMusicItemSource ()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}

		public string Name { get { return "Amarok Music"; } }
		public string Description { get { return "Provides access to artists and albums from Amarok."; } }
		public string Icon { get { return "amarok"; } }

		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (MusicItem),
					typeof (BrowseMusicItem),
					typeof (ApplicationItem),
				};
			}
		}

		public ICollection<IItem> Items { get { return items; } }

		public ICollection<IItem> ChildrenOfItem (IItem parent) {
			List<IItem> children;

			children = new List<IItem> ();
			if (parent is ApplicationItem && parent.Name == "Amarok Music Player") {
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
			foreach (IItem album in albums) items.Add (album);
			foreach (IItem artist in artists) items.Add (artist);
		}

		protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
		{
			return albums.FindAll (delegate (AlbumMusicItem album) {
				return album.Artist == artist.Name;
			});
		}
	}
}