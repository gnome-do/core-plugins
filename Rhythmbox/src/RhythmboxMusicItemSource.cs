//  RhythmboxMusicItemSource.cs
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

using Do.Addins;
using Do.Universe;

namespace Do.Addins.Rhythmbox
{
	
	public class RhythmboxMusicItemSource : IItemSource
	{
		List<IItem> items;
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;
		
		public RhythmboxMusicItemSource ()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}
		
		public string Name { get { return "Rhythmbox Music"; } }
		public string Description { get { return "Provides access to artists and albums from Rhythmbox."; } }
		public string Icon { get { return "rhythmbox"; } }

		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (MusicItem),
					typeof (BrowseMusicItem),
				};
			}
		}
		
		public ICollection<IItem> Items { get { return items; } }
		
		public ICollection<IItem> ChildrenOfItem (IItem parent) {
			List<IItem> children;
			
			children = new List<IItem> ();
			if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in AllAlbumsBy (parent as ArtistMusicItem))
					children.Add (album);
			}
			else if (parent is AlbumMusicItem) {
				foreach (TrackMusicItem track in Rhythmbox.TracksFor (parent as AlbumMusicItem))
					children.Add (track);
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
			Rhythmbox.LoadAlbumsAndArtists (out albums, out artists);
			foreach (IItem album in albums) items.Add (album);
			foreach (IItem artist in artists) items.Add (artist);
		}
		
		protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
		{
			// List<AlbumMusicItem> artist_albums;
			
			// artist_albums = new List<AlbumMusicItem> ();
			return albums.FindAll (delegate (AlbumMusicItem album) {
				return album.Artist == artist.Name;
			});
		}
	}
}
