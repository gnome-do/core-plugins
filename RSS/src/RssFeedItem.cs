/* RssFeedItem
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
using System.Collections.Generic;

using Do.Universe;

namespace Do.Plugins.Rss
{
	/// <summary>
	/// Simple base class for representing bookmarks.
	/// A bookmark is any item with a name and URL.
	/// </summary>
	public class RssFeedItem : Item
	{
		protected string name, url;

		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return name; }
		}

		public override string Icon {
			get { return "www"; }
		}

		public string URL {
			get { return url; }
		}

		/// <summary>
		/// Create a new RssFeedItem with a given name and URL.
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/> name for the bookmark.
		/// </param>
		/// <param name="url">
		/// A <see cref="System.String"/> url for the bookmark.
		/// </param>
		public RssFeedItem (string name, string url)
		{
			this.name = name;
			this.url = url;
		}

	}
}
