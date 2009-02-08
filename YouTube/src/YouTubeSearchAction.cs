/* YouTubeSearchAction.cs
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
using Do.Platform;
using System.Linq;

namespace YouTube
{
	public class YouTubeSearchAction : Act
	{
					
		public YouTubeSearchAction()
		{
		}
		 
		public override string Name
		{
			get { return "Search in YouTube"; }
		}
		
		public override string Description
		{
			get { return "Searches in YouTube"; }
		}
		
		public override string Icon
		{
			get { return "youtube_logo.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return	typeof (ITextItem); }
		}

		public override bool SupportsItem (Item item)
		{
			return (item is ITextItem);
		}
	
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{				
			
			//string search = "";
			
//			foreach (Item item in items) {
//				if (item is IUrlItem) {
//					search = (item as IUrlItem).Url;
//				} else if (item is ITextItem) {
//					search = (item as ITextItem).Text;
//				}
//				search = search.Replace (" ", "%20");
//				Services.Environment.OpenUrl(url+search);
//			}
			
			string search = (items.First() as ITextItem).Text; 
			search = search.Replace (" ", "%20");
			Services.Environment.OpenUrl(Youtube.searchUrl+search);
			yield break;
		}
	}
}
