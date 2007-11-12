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
		static readonly string kMusicLibraryFile;
		static readonly string kCoverArtDirectory;
		
		static RhythmboxMusicItemSource () {
			string home;
			
			home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			kMusicLibraryFile = "~/.gnome2/rhythmbox/rhythmdb.xml".Replace("~", home);
			kCoverArtDirectory = "~/.gnome2/rhythmbox/covers".Replace("~", home);
		}
		
		List<IItem> items;
		
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
					typeof (MusicAlbumItem),
				};
			}
		}
		
		public ICollection<IItem> Items { get { return items; } }
		public ICollection<IItem> ChildrenOfItem (IItem item) { return null; }
		
		public void UpdateItems ()
		{
			XmlDocument db;
			Dictionary<string, IItem> albums;
			Dictionary<string, IItem> artists;
			
			items.Clear ();
			db = new XmlDocument ();
			albums = new Dictionary<string, IItem> ();
			artists = new Dictionary<string, IItem> ();
			try {
				db.Load (kMusicLibraryFile);

				foreach (XmlNode entry in db.GetElementsByTagName ("entry")) {
					MusicAlbumItem album, artist_album;
					string song_title, album_title, artist, year, cover;
					string track_file;
					
					song_title = album_title = artist = year = cover = track_file = null;
					if (entry.Attributes.GetNamedItem ("type").Value != "song") continue;
					foreach (XmlNode song_attr in entry.ChildNodes) {
						switch (song_attr.Name) {
						case "title":
							song_title = song_attr.InnerText;
							break;
						case "album":
							album_title = song_attr.InnerText;
							break;
						case "artist":
							artist = song_attr.InnerText;
							break;
						case "year":
							year = song_attr.InnerText;
							break;
						case "location":
							track_file = song_attr.InnerText;
							break;
						}
					}
					if (song_title == null || album_title == null) continue;
					
					cover = string.Format ("{0} - {1}.jpg", artist, album_title);
					cover = Path.Combine (kCoverArtDirectory, cover);
					if (!File.Exists (cover)) cover = null;
					
					if (!albums.ContainsKey (album_title)) {
						albums[album_title] = new MusicAlbumItem (album_title, artist, year, cover);
					}
					if (!artists.ContainsKey (artist)) {
						artists[artist] = new MusicAlbumItem (artist, "All music by " + artist, year, cover);
					}
					album = albums[album_title] as MusicAlbumItem;
					artist_album = artists[artist] as MusicAlbumItem;
					album.AddTrack (track_file);
					artist_album.AddTrack (track_file);
				}
				items.AddRange (albums.Values);
				items.AddRange (artists.Values);
				
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Rhythmbox database file: " + e.Message);
			}
		}
		
		/*
		ContactItem ContactItemFromBuddyXmlNode (XmlNode buddy_node)
		{
			ContactItem buddy;
			string proto, name, alias, icon;
			
			try {
				name = alias = icon = null;
				// The messaging protocol (e.g. "prpl-jabber" for Jabber).
				proto = buddy_node.Attributes.GetNamedItem ("proto").Value;
				foreach (XmlNode attr in buddy_node.ChildNodes) {
					switch (attr.Name) {
					// The screen name.
					case "name":
						name = attr.InnerText;
						break;
					// The alias, or real name.
					case "alias":
						alias = attr.InnerText;
						break;
					// Buddy icon image file.
					case "setting":
						if (attr.Attributes.GetNamedItem ("name").Value == "buddy_icon") {
							icon = Path.Combine (kBuddyIconDirectory, attr.InnerText);
						}
						break;
					}
				}
			} catch {
				// Bad buddy.
				return null;
			}
			// If crucial details are missing, we can't make a buddy.
			if (name == null || proto == null) return null;
			
			// Create a new buddy, add the details we have.
			buddy = new ContactItem ();
			if (alias != null)
					buddy.Name = alias;
			if (icon != null)
					buddy.Photo = icon;
			AddScreenNameToContact (buddy, proto, name);
			return buddy;
		}
		*/
	}
}
