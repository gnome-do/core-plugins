//  RhythmboxItems.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
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
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Do.Rhythmbox
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
			base (Catalog.GetString ("Browse Artists"), 
				Catalog.GetString ("Browse Rhythmbox Music by Artist"))
		{
		}
	}

	class BrowseAlbumsMusicItem : BrowseMusicItem
	{
		public BrowseAlbumsMusicItem ():
			base (Catalog.GetString ("Browse Albums"), 
				Catalog.GetString ("Browse Rhythmbox Music by Album"))
		{
		}
	}

	public class RhythmboxRunnableItem : Item, IRunnableItem
	{
		public static readonly IEnumerable<RhythmboxRunnableItem> Items = new [] {
			new RhythmboxRunnableItem (
				Catalog.GetString ("Show Current Track"),
				Catalog.GetString ("Show Notification of Current Track in Rhythmbox"),
				"gnome-mime-audio",
				"--notify"),

			new RhythmboxRunnableItem (
				Catalog.GetString ("Mute"),
				Catalog.GetString ("Mute Rhythmbox Playback"),
				"audio-volume-muted",
				"--mute"),

			new RhythmboxRunnableItem (
				Catalog.GetString ("Unmute"),
				Catalog.GetString ("Unmute Rhythmbox Playback"),
				"audio-volume-high",
				"--unmute"),

			new RhythmboxRunnableItem (
				Catalog.GetString ("Volume Up"),
				Catalog.GetString ("Increase Rhythmbox Playback Volume"),
				"audio-volume-high",
				"--volume-up"),

			new RhythmboxRunnableItem (
				Catalog.GetString ("Volume Down"),
				Catalog.GetString ("Decrease Rhythmbox Playback Volume"),
				"audio-volume-low",
				"--volume-down"),
			};

		string name, description, icon;

		public RhythmboxRunnableItem (string name, string description, string icon, string command)
		{
			this.name = name;
			this.description = description;
			this.icon = icon;
			Command = command;
		}

		string Command { get; set; }

		public override string Name { get { return name; } }
		public override string Description { get { return description; } }
		public override string Icon { get { return icon; } }

		public void Run ()
		{
			Services.Application.RunOnThread (() => {
				Rhythmbox.StartIfNeccessary ();
				Rhythmbox.Client (Command);
			});
		}

	}
}
