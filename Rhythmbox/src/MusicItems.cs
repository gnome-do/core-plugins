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

namespace Do.Universe
{
	
	public class MusicAlbumItem : IItem
	{
		
		SortedList<string, string> tracks;
		string name, artist, year, cover;
		
		public MusicAlbumItem () {
			tracks = new SortedList<string, string> ();
		}
		
		public MusicAlbumItem (string name, string artist, string year, string cover) : this () {
			this.name = name;
			this.artist = artist;
			this.year = year;
			this.cover = cover;
		}
		
		public virtual string Name { get { return name; } }
		public string Description { get { return string.Format ("{0}", artist, year); } }
		public string Icon { get { return Cover ?? "gtk-cdrom"; } }
		
		public string Cover {
			get { return cover; }
			set { cover = value; }
		}
		
		public ICollection<string> Tracks {
			get { return tracks.Values; }
		}
		
		public void AddTrack (string track)
		{
			try {
				tracks.Add (track, track);
			} catch {}
		}
	}

}
