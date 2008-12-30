//  xmms2ItemSource.cs
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
using System.Collections.Generic;


using Do.Universe;

namespace Do.Addins.xmms2
{

	public class xmms2MusicItemSource : ItemSource
	{
		List<Item> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;

		public xmms2MusicItemSource ()
		{
			items = new List<Item> ();
			//UpdateItems ();
		}

		public override string Name { get { return "xmms2 Music"; } }
		public override string Description { get { return "Provides access to artists and albums from xmms2."; } }
		public override string Icon { get { return "xmms2"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (MusicItem),
					typeof (BrowseMusicItem),
					typeof (IApplicationItem),
					typeof (PlaylistItem),
					typeof (BrowsePlaylistItem),
				};
			}
		}

		public override IEnumerable<Item> Items { get { return items; } }

		public override IEnumerable<Item> ChildrenOfItem (Item parent) {
			List<Item> children;
						
			children = new List<Item> ();
			if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in AllAlbumsBy (parent as ArtistMusicItem))
					children.Add (album);
				children.Add(new BrowseAllMusicItem (parent as ArtistMusicItem));
			}
			else if (parent is AlbumMusicItem) {
				foreach (SongMusicItem song in xmms2.LoadSongsFor (parent as AlbumMusicItem))
					children.Add (song);
			}
			else if (parent is PlaylistItem) {
				foreach (SongMusicItem song in xmms2.LoadSongsFor (parent as PlaylistItem))
					children.Add (song);
			}
			else if (parent is BrowseAlbumsMusicItem) {
				foreach (AlbumMusicItem album in albums)
					children.Add (album);
			}
			else if (parent is BrowseArtistsMusicItem) {
				foreach (ArtistMusicItem artist in artists)
					children.Add (artist);
			}
			else if (parent is BrowsePlaylistItem){
				foreach(Item playlist in xmms2.playlists){
					children.Add(playlist);
				}
			}
			else if(parent is BrowseAllMusicItem){
				foreach (SongMusicItem song in xmms2.LoadSongsFor ((parent as BrowseAllMusicItem).Artist))
					children.Add (song);
			}
			return children;
		}

		public override void UpdateItems ()
		{
			items.Clear ();

			// Add play, pause, etc. controls.
			items.AddRange (xmms2RunnableItem.DefaultItems);

			// Add browse features.
			items.Add (new BrowseAlbumsMusicItem ());
			items.Add (new BrowseArtistsMusicItem ());
			items.Add (new BrowsePlaylistItem());
			
			// Add albums and artists.
			xmms2.LoadAlbumsAndArtists (out albums, out artists);
			xmms2.LoadAllPlaylists();
			if (xmms2.playlists != null) {
				foreach (Item playlist in xmms2.playlists) items.Add(playlist);
			}
			if (albums != null) {
				foreach (Item album in albums) items.Add (album);
			}
			if (artists != null) {
				foreach (Item artist in artists) items.Add (artist);
			}
		}

		protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
		{
			return albums.FindAll (delegate (AlbumMusicItem album) {
				return album.Artist == artist.Name;
			});
		}
	}
}
