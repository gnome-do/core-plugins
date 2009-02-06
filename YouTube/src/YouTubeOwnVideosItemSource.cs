/* YouTubeOwnVideosItemSource.cs
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
using System.Threading;
using Do.Universe;

namespace Do.Universe
{
	
	
	public class YouTubeOwnVideosItemSource : ItemSource
	{
		public YouTubeOwnVideosItemSource()
		{
		}
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get { yield return typeof (YoutubeVideoItem);}
		}
		
		public override string Name { get { return "YouTube Videos"; } }
		public override string Description { get { return "Your own YouTube videos"; } }
		public override string Icon {get { return "youtube_logo.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Item> Items 
		{
			get { return Youtube.own; }
		}
		
		public override void UpdateItems ()
		{
			Thread t = new Thread((ThreadStart) Youtube.updateOwn);
			t.IsBackground = true;
			t.Start();
		}
		
	}
}
