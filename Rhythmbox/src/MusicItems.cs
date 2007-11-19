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

using Do.Addins;
using Do.Universe;

namespace Do.Addins.Rhythmbox
{
	
	public abstract class MusicItem : IItem
	{
		protected string name, artist, year, cover;
		
		public MusicItem ()
		{
		}
		
		public MusicItem (string name, string artist, string year, string cover):
			this ()
		{
			this.name = name;
			this.artist = artist;
			this.year = year;
			this.cover = cover;
		}
		
		public virtual string Name { get { return name; } }
		public virtual string Description { get { return artist; } }
		public virtual string Icon { get { return Cover ?? "gtk-cdrom"; } }
		
		public string Artist {
			get { return artist; }
		}
		
		public string Cover {
			get { return cover; }
			set { cover = value; }
		}
	
	}
	
	public class AlbumMusicItem : MusicItem
	{
		public AlbumMusicItem (string name, string artist, string year, string cover):
			base (name, artist, year, cover)
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
		
		public override string Name { get { return artist; } }
		public override string Description { get { return string.Format ("All music by {0}", artist); } }
	}
	
	public class TrackMusicItem : MusicItem
	{
		string file, album;
		
		public TrackMusicItem (string name, string artist, string album, string year, string cover, string file):
			base (name, artist, year, cover)
		{
			this.file = file;
			this.album = album;
			this.cover = "gnome-mime-audio";
		}
		
		public string File { get { return file; } }
		public override string Description { get { return string.Format ("{0} - {1}", artist, album); } }
	}
}
