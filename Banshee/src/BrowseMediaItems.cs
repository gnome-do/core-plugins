/* BrowseMediaItems.cs
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

using Mono.Addins;

using Do.Universe;

namespace Banshee
{
	public class BrowseMediaItem : Item 
	{
		string name, description;
		
		public BrowseMediaItem (string name, string description)
		{	
			this.name = name;
			this.description = description;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return description; }
		}
		
		public override string Icon {
			get { return "media-optical"; }
		}
	}
	
	public class BrowseArtistMusicItem : BrowseMediaItem
	{
		public BrowseArtistMusicItem () :
			base (AddinManager.CurrentLocalizer.GetString ("Browse Artists"),
				AddinManager.CurrentLocalizer.GetString ("Browse Music by Artist"))
		{
		}
			
		public override string Icon {
			get { return "audio-input-microphone"; }
		}
	}
	
	public class BrowseAlbumsMusicItem : BrowseMediaItem
	{
		public BrowseAlbumsMusicItem () :
			base (AddinManager.CurrentLocalizer.GetString ("Browse Albums"), 
				AddinManager.CurrentLocalizer.GetString ("Browse Music by Album"))
		{
		}
	}
	
	public class BrowsePublisherPodcastItem : BrowseMediaItem
	{
		public BrowsePublisherPodcastItem () : base (AddinManager.CurrentLocalizer.GetString ("Browse Podcasts"),
			AddinManager.CurrentLocalizer.GetString ("Browse Podcasts by Publisher"))
		{
		}
	}
	
	public class BrowseVideoItem : BrowseMediaItem
	{
		public BrowseVideoItem () : base (AddinManager.CurrentLocalizer.GetString ("Browse Videos"),
			AddinManager.CurrentLocalizer.GetString ("Browse All Videos"))
		{
		}
	}
}
