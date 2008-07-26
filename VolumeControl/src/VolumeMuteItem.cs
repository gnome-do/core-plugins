/* VolumeMuteItem.cs
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
using Do.Addins;
using Do.Universe;
using Mono.Unix;

namespace VolumeControl
{
	public class VolumeMuteItem : IRunnableItem
	{
		public string Name {
			get { return Catalog.GetString ("Mute Volume"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Mute system volume"); }
		}
		
		public string Icon {
			get { return "audio-volume-muted"; }
		}
		
		public void Run ()
		{
			System.Diagnostics.Process.Start ("amixer set Master 0% > /dev/null");
		}
	}
}