/* RssFeedLinkItem
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
	public class RssFeedLinkItem : Item, IUrlItem
	{
		protected string name, url, description;

		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return description; }
		}

		public override string Icon {
			get { return "www"; }
		}

		public string Url {
			get { return url; }
		}

		/// <summary>
		/// Create a new RssFeedLinkItem with a given name, URL and description.
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/> name for the feed.
		/// </param>
		/// <param name="url">
		/// A <see cref="System.String"/> url for the feed.
		/// </param>
		/// <param name="description">
		/// A <see cref="System.String"/> url for the description.
		/// </param>
		public RssFeedLinkItem (string name, string url, string description)
		{
			this.name = name;
			this.url = url;
			this.description = description;
		}


	}
}
