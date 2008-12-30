/* TSClientItem.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
 *
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;

using Do.Universe;

namespace Simulacra
{
	public class TSClientItem : Item, IOpenableItem
	{
		string name;
		string path;

		public TSClientItem (string hostname, string filepath) {
			name = hostname;
			path = filepath;
		}

		public override string Name { get { return name; } }
		public string Path { get { return path; } }
		public override string Description { get { return "Remote Desktop host"; } }
		public override string Icon { get { return "tsclient"; } }

		public string Text { get { return name; } }

		public void Open () {
			Process.Start ("tsclient", "-x " + path);
		}
	}
}
