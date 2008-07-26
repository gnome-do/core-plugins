/* ScreenshotDelayItem.cs
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
using Mono.Unix;
using Do.Universe;

namespace GNOME {

	class ScreenshotDelayItem : IItem {

		int seconds;

		public ScreenshotDelayItem (int seconds) {
			this.seconds = seconds;
		}

		public string Name {
			get { return string.Format (Catalog.GetString ("{0}-second delay"), seconds); }
		}

		public string Description {
			get {
				return Catalog.GetPluralString ("Wait {0} second before taking the screenshot.",
					"Wait {0} seconds before taking the screenshot.", seconds);
			}
		}

		public string Icon {
			get { return "gnome-panel-clock"; }
		}

		public int Seconds {
			get { return seconds; }
		}
	}
}
