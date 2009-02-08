/* YouTubeSubscriptionItemSource.cs
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

namespace YouTube
{	
	public class YouTubeSubscriptionItemSource : ItemSource
	{
		public YouTubeSubscriptionItemSource()
		{
		}
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get {yield return typeof (YouTubeSubscriptionItem);}
		}
		
		public override string Name { get { return "Youtube Subscriptions"; } }
		public override string Description { get { return "Your YouTube subscriptions"; } }
		public override string Icon {get { return "youtube_user.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Item> Items 
		{
			get { return Youtube.subscriptions; }
		}
		
		public override void UpdateItems ()
		{
			Thread t = new Thread((ThreadStart) Youtube.updateSubscriptions);
			t.IsBackground = true;
			t.Start();
		}		
	}
}

