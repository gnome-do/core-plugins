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


namespace Do.Addins.Banshee {

        public class BansheeItemSource : ItemSource {
                List<Item> items;
                List<AlbumMusicItem> albums;
                List<ArtistMusicItem> artists;

                public BansheeItemSource()
                {
                        items = new List<Item> ();
                        //UpdateItems();
                }

                public override string Name {
                        get { return "Banshee Music"; }
                }
                public override string Description {
                        get { return "Provides access to artists and albums from Banshee."; }
                }
                public override string Icon {
                        get { return "music-player-banshee"; }
                }

                public override IEnumerable<Type> SupportedItemTypes {
                        get {
                                return new Type[] {
                                        typeof (MusicItem),
                                        typeof (BrowseMusicItem),
                                        typeof (ApplicationItem),
                                    };
                        }
                }

                public override IEnumerable<Item> Items {
                    get { return items; }
                }

                public override IEnumerable<Item> ChildrenOfItem (Item parent)
                {
                        List<Item> children;

                        children = new List<Item> ();
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

                public override void UpdateItems ()
                {
                    items.Clear ();

                    // Add browse features
                    items.Add (new BrowseAlbumsMusicItem ());
                    items.Add (new BrowseArtistsMusicItem ());

                    // Add albums and artists
                    Banshee.LoadAlbumsAndArtists (out albums, out artists);
                    foreach (Item album in albums) items.Add (album);
                    foreach (Item artist in artists) items.Add (artist);
                }
        }
}
