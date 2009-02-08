/* YouTubeFavoriteItemSource.cs
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
using Mono.Unix;
using Gtk;
using Do.Universe;
using Do.Platform.Linux;
using System.Threading;

namespace YouTube
{
	public class YouTubeFavoriteItemSource : ItemSource, IConfigurable
	{
		public YouTubeFavoriteItemSource()
		{
		}
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get { yield return typeof (YoutubeVideoItem);}
		}
		
		public override string Name { get { return "Youtube Favorites"; } }
		public override string Description { get { return "Videos on your Youtube favorites list."; } }
		public override string Icon {get { return "youtube_logo.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Do.Universe.Item> Items 
		{
			get { return Youtube.favorites; }
		}
		
		public override void UpdateItems ()
		{
			Thread t = new Thread((ThreadStart) Youtube.updateFavorites);
			t.IsBackground = true;
			t.Start();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new YouTubeConfig ();
		}
		
	}
}
