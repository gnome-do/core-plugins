/* OutputItem.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
 *
 * OutputItem represents one output whose resolution may be changed.
 */

using System;
using Do.Universe;
using Mono.Unix;

namespace XRandR
{
	public class OutputItem : Item
	{
		string name;
		int id;
		bool connected;
		public OutputItem(int id, XRROutputInfo output, bool connected)
		{
			this.name = output.name;
			this.id = id;
			this.connected = connected;
		}
		
		public override string Name {
			get { return name; }
		}
		public int Id {
			get { return id; }
		}
		
		public override string Description {
			get { return Catalog.GetString (connected ? "Set your resolution" : "Not connected"); }
		}
		
		public override string Icon {
			get { return "video-display"; }
		}
	}
}
