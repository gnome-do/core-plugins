//  MusicItems.cs
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
using System.Collections.Generic;


using Do.Universe;

namespace Do.Addins.xmms2
{

	public abstract class MusicItem : Item
	{
		protected string name, artist, cover;
		
		public MusicItem ()
		{
		}

		public MusicItem (string name, string artist,string cover):
			this ()
		{
			this.name = name;
			this.artist = artist;
			this.cover = cover;
		}

		public override string Name { get { return name; } }
		public override string Description { get { return artist; } }
		public override string Icon { get { return Cover ?? "gtk-cdrom"; } }

		public virtual string Artist { get { return artist; } }
		public virtual string Cover { get { return cover; } }

	}

	public class AlbumMusicItem : MusicItem
	{
		public AlbumMusicItem (string name, string artist, string cover):
			base (name, artist, cover)
		{
		}
	}

	public class ArtistMusicItem : MusicItem
	{
		public ArtistMusicItem (string artist, string cover):
			base ()
		{
			this.artist = this.name = artist;
			this.cover = cover;
		}

		public override string Description
		{
			get {
				return string.Format ("All music by {0}", artist);
			}
		}
	}

	public class SongMusicItem : MusicItem, IComparable
	{
		string album; 
		int id;

		public SongMusicItem (int id,  string artist, string album, string name):
			base (name, artist, null)
		{
			this.id = id;
			this.album = album;
		}

		public override string Icon { get { return "gnome-mime-audio"; } }
		public override string Description
		{
			get {
				return string.Format ("{0} - {1}", artist, album);
			}
		}

		public virtual string Album { get { return album; } }
		public virtual int Id { get { return id; } }
		
		public int CompareTo (object o)
		{
			SongMusicItem other = o as SongMusicItem;
			if (album.CompareTo (other.Album) == 0)
				return id - other.Id;
			return album.CompareTo (other.Album);
		}
	}
}
