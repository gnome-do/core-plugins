//  AmarokItems.cs
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
using System.Threading;
using System.Diagnostics;

using Do.Plugins;
using Do.Universe;

namespace Do.Plugins.Amarok
{

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
			base ("Browse Artists", "Browse Amarok Music by Artist")
		{
		}
	}

	class BrowseAlbumsMusicItem : BrowseMusicItem
	{
		public BrowseAlbumsMusicItem ():
			base ("Browse Albums", "Browse Amarok Music by Album")
		{
		}
	}

	public class AmarokRunnableItem : Item, IRunnableItem
	{
		public static readonly AmarokRunnableItem[] DefaultItems =
			new AmarokRunnableItem[] {

				new AmarokRunnableItem ("Play",
						"Play Current Track in Amarok",
						"player_play",
						"--play"),

				new AmarokRunnableItem ("Pause",
						"Pause Amarok Playback",
						"player_pause",
						"--pause"),

				new AmarokRunnableItem ("Next",
						"Play Next Track in Amarok",
						"player_end",
						"--next"),

				new AmarokRunnableItem ("Previous",
						"Play Previous Track in Amarok",
						"player_start",
						"--previous"),
			};

		string name, description, icon, command;

		public AmarokRunnableItem (string name, string description, string icon, string command)
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
					Amarok.StartIfNeccessary ();
					Amarok.Client (command);
					}).Start ();
		}

	}
}
