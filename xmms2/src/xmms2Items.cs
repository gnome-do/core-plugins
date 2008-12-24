//  RhythmboxItems.cs
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
using System.Threading;
using System.Diagnostics;


using Do.Universe;

namespace Do.Addins.xmms2{

	class BrowseMusicItem: Item
	{
		string name, description;

		public BrowseMusicItem (string name, string description)
		{
			this.name = name;
			this.description = description;
		}
		
		public override string Name { get { return name; } }
		public override string Description { get { return description; } }
		public override string Icon { get { return "gtk-cdrom"; } }
	}

	class BrowseArtistsMusicItem : BrowseMusicItem
	{
		public BrowseArtistsMusicItem ():
			base ("Browse Artists", "Browse xmms2 Music by Artist")
		{
		}
	}

	class BrowseAlbumsMusicItem : BrowseMusicItem
	{
		public BrowseAlbumsMusicItem ():
			base ("Browse Albums", "Browse xmms2 Music by Album")
		{
		}
	}

	class BrowseAllMusicItem : BrowseMusicItem
	{
		ArtistMusicItem artist;
		public BrowseAllMusicItem (ArtistMusicItem artist):
			base ("Songs", "All songs by " + artist.Artist)
		{
			this.artist = artist;
		}
		public ArtistMusicItem Artist { get { return artist; } }
	}

	public class BrowsePlaylistItem : Item{

		public BrowsePlaylistItem(){
		}
		
		public override string Name { get { return "Browse Playlists"; } }
		public override string Description { get { return "View all xmms2 playlists"; } }
		public override string Icon { get { return "gtk-cdrom"; } }
	}
	
	public class PlaylistItem: Item{
		public bool current;
		string name;
		string desc; //separated for (current) tag
		//public List<SongMusicItem> entries;
		
		public PlaylistItem(string name){
			this.name = name;
		}
	
		public override string Name { get { return name; } } 
		public override string Description { get { 			
				if(this.current){
				this.desc = string.Format("{0} Playlist (Current)", name);
			}else{
				this.desc = string.Format("{0} Playlist", name);
			}return desc; } }
		public override string Icon { get { return "format-justify-fill"; } }
	}
	
	class xmms2RunnableItem : Item, IRunnableItem
	{
		public static readonly xmms2RunnableItem[] DefaultItems =
			new xmms2RunnableItem[] {
			
				new xmms2RunnableItem ("Queue All",
						"Add library to xmms2 playlist",
						"gtk-add",
						"mlib loadall"),
			
				new xmms2RunnableItem ("Play",
						"Play Current Track in xmms2",
						"player_play",
						"play"),

				new xmms2RunnableItem ("Pause",
						"Pause xmms2 Playback",
						"player_pause",
						"toggleplay"),

				new xmms2RunnableItem ("Next",
						"Play Next Track in xmms2",
						"player_end",
						"next"),

				new xmms2RunnableItem ("Previous",
						"Play Previous Track in xmms2",
						"player_start",
						"prev"),

				new xmms2RunnableItem ("Stop",
						"Stop Current Track in xmms2",
						"player_stop",
						"stop"),

				new xmms2RunnableItem ("Mute",
						"Mute xmms2 Playback",
						"audio-volume-muted",
						"volume 0"),

				new xmms2RunnableItem ("Volume Up",
						"Increase xmms2 Playback Volume",
						"audio-volume-high",
						"volume +10"),

				new xmms2RunnableItem ("Volume Down",
						"Decrease xmms2 Playback Volume",
						"audio-volume-low",
						"volume -10"),
			};

		string name, description, icon, command;

		public xmms2RunnableItem (string name, string description, string icon, string command)
		{
			this.name = name;
			this.description = description;
			this.icon = icon;
			this.command = command;
		}

		public override string Name { get { return name; } }
		public override string Description { get { return description; } }
		public override string Icon { get { return icon; } }

		public void Run ()
		{
			new Thread ((ThreadStart) delegate {
					xmms2.StartIfNeccessary ();
					xmms2.Client (command);
					}).Start ();
		}

	}
}
