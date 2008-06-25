// BansheeItemSource.cs
//
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//

using System;
using System.Collections.Generic;

using Do.Universe;
using Do.Addins;

namespace Do.Addins.Banshee {

        public class BansheeItemSource : IItemSource {
                List<IItem> items;
                List<AlbumMusicItem> albums;
                List<ArtistMusicItem> artists;

                public BansheeItemSource()
                {
                        items = new List<IItem> ();
                        //UpdateItems();
                }

                public string Name {
                        get { return "Banshee Music"; }
                }
                public string Description {
                        get { return "Provides access to artists and albums from Banshee."; }
                }
                public string Icon {
                        get { return "music-player-banshee"; }
                }

                public Type[] SupportedItemTypes {
                        get {
                                return new Type[] {
                                        typeof (MusicItem),
                                        typeof (BrowseMusicItem),
                                        typeof (ApplicationItem),
                                    };
                        }
                }

                public ICollection<IItem> Items {
                    get { return items; }
                }

                public ICollection<IItem> ChildrenOfItem (IItem parent)
                {
                        List<IItem> children;

                        children = new List<IItem> ();
                        if (parent is ApplicationItem && parent.Name == "Banshee") {
                                children.Add (new BrowseAlbumsMusicItem ());
                                children.Add (new BrowseArtistsMusicItem ());
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

                protected List<AlbumMusicItem> AllAlbumsBy (ArtistMusicItem artist)
                {
                        return albums.FindAll (delegate (AlbumMusicItem album) {
                                return album.Artist == artist.Name;
                        });
                }

                public void UpdateItems ()
                {
                    items.Clear ();

                    // Add browse features
                    items.Add (new BrowseAlbumsMusicItem ());
                    items.Add (new BrowseArtistsMusicItem ());

                    // Add albums and artists
                    Banshee.LoadAlbumsAndArtists (out albums, out artists);
                    foreach (IItem album in albums) items.Add (album);
                    foreach (IItem artist in artists) items.Add (artist);
                }
        }
}
