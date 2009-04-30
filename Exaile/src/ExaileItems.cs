//  ExaileItems.cs
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

namespace Exaile
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
			base (Catalog.GetString ("Browse Artists"), Catalog.GetString ("Browse Exaile Music by Artist"))
		{
		}
	}

	class BrowseAlbumsMusicItem : BrowseMusicItem
	{
		public BrowseAlbumsMusicItem ():
			base (Catalog.GetString ("Browse Albums"), Catalog.GetString ("Browse Exaile Music by Album"))
		{
		}
	}

	public class ExaileRunnableItem : Item, IRunnableItem
	{
		public static readonly IEnumerable<ExaileRunnableItem> Items = new [] {
			new ExaileRunnableItem (
				Catalog.GetString ("Show Current Track"),
				Catalog.GetString ("Show Notification of Current Track in Exaile"),
				"gnome-mime-audio",
				"--gui-query"),

			new ExaileRunnableItem (
				Catalog.GetString ("Volume Up"),
				Catalog.GetString ("Increase Exaile Playback Volume"),
				"audio-volume-high",
				"--increase_vol=10"),

			new ExaileRunnableItem (
				Catalog.GetString ("Volume Down"),
				Catalog.GetString ("Decrease Exaile Playback Volume"),
				"audio-volume-low",
				"--decrease_vol=10"),
			};

		string name, description, icon;

		public ExaileRunnableItem (string name, string description, string icon, string command)
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
				Exaile.Client (Command);
			});
		}

	}
}
