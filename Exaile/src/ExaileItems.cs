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

using Mono.Addins;

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
			base (AddinManager.CurrentLocalizer.GetString ("Browse Artists"), AddinManager.CurrentLocalizer.GetString ("Browse Exaile Music by Artist"))
		{
		}
	}

	class BrowseAlbumsMusicItem : BrowseMusicItem
	{
		public BrowseAlbumsMusicItem ():
			base (AddinManager.CurrentLocalizer.GetString ("Browse Albums"), AddinManager.CurrentLocalizer.GetString ("Browse Exaile Music by Album"))
		{
		}
	}

	public class ExaileRunnableItem : Item, IRunnableItem
	{
		public static readonly IEnumerable<ExaileRunnableItem> Items = new [] {
			new ExaileRunnableItem (
				AddinManager.CurrentLocalizer.GetString ("Show Current Track"),
				AddinManager.CurrentLocalizer.GetString ("Show Notification of Current Track in Exaile"),
				"gnome-mime-audio",
				"--gui-query"),

			new ExaileRunnableItem (
				AddinManager.CurrentLocalizer.GetString ("Volume Up"),
				AddinManager.CurrentLocalizer.GetString ("Increase Exaile Playback Volume"),
				"audio-volume-high",
				"--increase_vol=10"),

			new ExaileRunnableItem (
				AddinManager.CurrentLocalizer.GetString ("Volume Down"),
				AddinManager.CurrentLocalizer.GetString ("Decrease Exaile Playback Volume"),
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
