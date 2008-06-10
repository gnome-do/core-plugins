/* RssFeedAction.cs
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

using Gtk;
using System;
using System.IO;
using System.Reflection;

namespace Do.Plugins.Rss
{
	public partial class Configuration : Gtk.Bin
	{
		public Configuration ()
		{
			this.Build();
			Timeout.Value = RssItemSource.Timeout;
			CacheDuration.Value = RssItemSource.CacheDuration;
			OpmlChooser.SetUri (RssItemSource.OpmlFile);
		}

		protected virtual void OnTimeoutValueChanged (object o, EventArgs e)
		{
			RssItemSource.Timeout = Timeout.ValueAsInt;
		}

		protected virtual void OnOpmlChooserSelectionChanged (object o, EventArgs e)
		{
			RssItemSource.OpmlFile = OpmlChooser.Uri;
		}

		protected virtual void OnCacheDurationValueChanged (object o, EventArgs e)
		{
			RssItemSource.CacheDuration = CacheDuration.ValueAsInt;
		}
	}
}
