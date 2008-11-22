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
using Do.Universe;
using Mono.Unix;

namespace Banshee1
{
	public class BrowseMediaItem : IItem 
	{
		string name, description;
		
		public BrowseMediaItem (string name, string description)
		{	
			this.name = name;
			this.description = description;
		}
		
		public string Name {
			get { return name; }
		}
		
		public string Description {
			get { return description; }
		}
		
		public virtual string Icon {
			get { return "media-optical"; }
		}
	}
	
	public class BrowseArtistMusicItem : BrowseMediaItem
	{
		public BrowseArtistMusicItem () :
			base (Catalog.GetString ("Browse Artists"),
				Catalog.GetString ("Browse Music by Artist"))
		{
		}
			
		public override string Icon {
			get { return "audio-input-microphone"; }
		}
	}
	
	public class BrowseAlbumsMusicItem : BrowseMediaItem
	{
		public BrowseAlbumsMusicItem () :
			base (Catalog.GetString ("Browse Albums"), 
				Catalog.GetString ("Browse Music by Album"))
		{
		}
	}
	
	public class BrowsePublisherPodcastItem : BrowseMediaItem
	{
		public BrowsePublisherPodcastItem () : base (Catalog.GetString ("Browse Podcasts"),
			Catalog.GetString ("Browse Podcasts by Publisher"))
		{
		}
	}
	
	public class BrowseVideoItem : BrowseMediaItem
	{
		public BrowseVideoItem () : base (Catalog.GetString ("Browse Videos"),
			Catalog.GetString ("Browse All Videos"))
		{
		}
	}
}
