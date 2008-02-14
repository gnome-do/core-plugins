// BansheeItems.cs
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
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Do.Addins;
using Do.Universe;

namespace Do.Addins.Banshee {

        public abstract class MusicItem : IItem {
                protected string name, artist, year, cover;

                public MusicItem ()
                {
                }

                public MusicItem (string name, string artist, string year, string cover): this ()
                {
                        this.name = name;
                        this.artist = artist;
                        this.year = year;
                        this.cover = cover;
                }

                public virtual string Name {
                        get { return name; }
                }
                public virtual string Description {
                        get { return artist; }
                }
                public virtual string Icon {
                        get { return Cover ?? "media-optical"; }
                }
                public virtual string Artist {
                        get { return artist; }
                }
                public virtual string Year {
                        get { return year; }
                }
                public virtual string Cover {
                        get { return cover; }
                }
        }

        public class AlbumMusicItem : MusicItem {

                public AlbumMusicItem (string name, string artist, string year, string cover):
                    base (name, artist, year, cover)
                {
                }
        }

        public class ArtistMusicItem : MusicItem {

                public ArtistMusicItem (string artist, string cover):
                    base ()
                {
                        this.artist = this.name = artist;
                        this.cover = cover;
                }

                public override string Description {
                        get { return string.Format ("All music by {0}", artist); }
                }
                public override string Icon {
                        get { return Cover ?? "audio-input-microphone"; }
                }
        }

        public class SongMusicItem : MusicItem {

                string file, album;

                public SongMusicItem (string name, string artist, string album,
                                      string year, string cover, string file):
                                      base (name, artist, year, cover)
                {
                        this.file = file;
                        this.album = album;
                        if (this.cover == null)
                                FetchCover();
                }

                private void FetchCover()
                {
                        // Fetch the cover if present
                        Uri u;
                        string coverpath = null;
                        string[] coverpattern = {"cover.png", "cover.jpg", "cover.jpeg",
                                                 "Cover.png", "Cover.jpg", "Cover.jpeg" };
                        // Check if the file is local
                        if (this.file.StartsWith("file:///")) {
                            foreach (string pattern in coverpattern) {
                                    u = (new Uri(this.file));
                                    coverpath = Path.Combine (Path.GetDirectoryName(u.ToString().Replace("file://", "")), 
                                                                                    pattern);
                                    if (!System.IO.File.Exists (coverpath))
                                            continue;
                                    this.cover = coverpath;
                             }
                        }
                }

                public override string Icon {
                        get { return "audio-x-generic"; }
                }
                public override string Description {
                        get { return string.Format ("{0} - {1}", artist, album); }
                }
                public virtual string File {
                        get { return file; }
                }
                public virtual string Album {
                        get { return album; }
                }
        }

        public class BrowseMusicItem: IItem {
                string name, description;

                public BrowseMusicItem (string name, string description)
                {
                        this.name = name;
                        this.description = description;
                }

                public string Name {
                        get { return name; }
                }
                public string Description {
                        get { return description; }
                }
                public virtual string Icon {
                        get { return "media-optical"; }
                }
        }

        public class BrowseArtistsMusicItem : BrowseMusicItem
        {
                public BrowseArtistsMusicItem ():
                    base ("Browse Artists", "Browse Banshee Music by Artist")
                {
                }

                public override string Icon {
                            get { return "audio-input-microphone"; }
                }
        }

        public class BrowseAlbumsMusicItem : BrowseMusicItem
        {
                public BrowseAlbumsMusicItem ():
                    base ("Browse Albums", "Browse Banshee Music by Album")
                {
                }
        }

        public class BansheeRunnableItem : IRunnableItem
        {
                public static readonly BansheeRunnableItem[] DefaultItems =
                  new BansheeRunnableItem[] {

                    new BansheeRunnableItem ("Play",
                                               "Play Current Track in Banshee",
                                               "media-playback-start",
                                               "play-pause"),

                    new BansheeRunnableItem ("Pause",
                                               "Toggle Banshee Playback",
                                               "media-playback-pause",
                                               "play-pause"),

                    new BansheeRunnableItem ("Next",
                                               "Play Next Track in Banshee",
                                               "media-skip-forward",
                                               "next"),

                    new BansheeRunnableItem ("Previous",
                                               "Play Previous Track in Banshee",
                                               "media-skip-backward",
                                               "previous"),

                };

                string name, description, icon, command;

                public BansheeRunnableItem (string name, string description,
                                            string icon, string command)
                {
                        this.name = name;
                        this.description = description;
                        this.icon = icon;
                        this.command = command;
                }

                public string Name {
                        get { return name; }
                }
                public string Description {
                        get { return description; }
                }
                public string Icon {
                        get { return icon; }
                }

                public void Run ()
                {
                        new Thread ((ThreadStart) delegate {
                                BansheeDBus b = new BansheeDBus();
                                switch (command) {
                                case "play-pause":
                                    b.TogglePlaying();
                                    break;
                                case "next":
                                    b.Next();
                                    break;
                                case "previous":
                                    b.Previous();
                                    break;
                                }
                        }).Start ();
                }
        }
}
