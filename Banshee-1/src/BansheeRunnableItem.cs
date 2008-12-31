/* BansheeRunnableItem.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Threading;

using Mono.Unix;

using Do.Universe;

namespace Banshee
{
	public class BansheeRunnableItem : IRunnableItem
	{	
		string name, description, icon, command;
		
		public static readonly BansheeRunnableItem[] DefaultItems =
			new BansheeRunnableItem[] {
                new BansheeRunnableItem (Catalog.GetString ("Pause/Play"),
					Catalog.GetString ("Toggle Banshee Playback"),
					"media-playback-pause",
					"play-pause"),

				new BansheeRunnableItem (Catalog.GetString ("Next"),
					Catalog.GetString ("Play Next Track in Banshee"),
					"media-skip-forward",
					"next"),

				new BansheeRunnableItem (Catalog.GetString ("Previous"),
					Catalog.GetString ("Play Previous Track in Banshee"),
					"media-skip-backward",
					"previous"),
			};
			
		public BansheeRunnableItem (string name, string description, string icon, string command)
		{
			this.name = name;
			this.description = description;
			this.icon = icon;
			this.command = command;
		}
		
		public string Name {
			get { return name; }
		}
		
		public string Description {
			get { return description; }
		}
		
		public string Icon {
			get { return icon; }
		}
		
		public void Run ()
		{
			new Thread ((ThreadStart) delegate {
				BansheeDBus bd = new BansheeDBus ();
				switch (command) {
				case "play-pause":
					bd.TogglePlaying ();
					break;
				case "next":
					bd.Next ();
					break;
				case "previous":
					bd.Previous ();
					break;
				}
			}).Start ();
		}
	}
}
