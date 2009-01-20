// BansheeRunnableItem.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Banshee
{

	public class BansheeRunnableItem : Item, IRunnableItem
	{	
		Action action;
		string name, description, icon;

		public static readonly IEnumerable<BansheeRunnableItem> DefaultItems =
			new [] {
				new BansheeRunnableItem (Catalog.GetString ("Play"),
						Catalog.GetString ("Start or resume Banshee playback."),
						"media-playback-play",
						Banshee.Play),

				new BansheeRunnableItem (Catalog.GetString ("Pause"),
						Catalog.GetString ("Pause or resume Banshee playback."),
						"media-playback-pause",
						Banshee.Pause),

				new BansheeRunnableItem (Catalog.GetString ("Next"),
						Catalog.GetString ("Play Next Track in Banshee"),
						"media-skip-forward",
						Banshee.Next),

				new BansheeRunnableItem (Catalog.GetString ("Previous"),
						Catalog.GetString ("Play Previous Track in Banshee"),
						"media-skip-backward",
						Banshee.Previous),
			};

		public BansheeRunnableItem (string name, string description, string icon, Action action)
		{
			this.name = name;
			this.description = description;
			this.icon = icon;
			this.action = action;
		}

		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return description; }
		}

		public override string Icon {
			get { return icon; }
		}

		public void Run ()
		{
			action ();
		}
	}

}
