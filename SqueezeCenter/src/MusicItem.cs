//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;


using Do.Universe;

namespace SqueezeCenter
{	

	public abstract class MusicItem : SqueezeCenterItem
	{	
		
		public readonly int Id;

		public MusicItem (int id)
		{
			this.Id = id;			
		}
		
		public abstract string SqueezeCenterIdKey { get; }
		public override abstract string Name { get; }
		public override abstract string Description { get; }
		public override abstract string Icon { get; }
	}

	public class AlbumMusicItem : MusicItem
	{		
		public readonly string Album, Year;
		readonly ArtistMusicItem artist;
		readonly int firstSongId;
				
		public AlbumMusicItem (int id, string album, ArtistMusicItem artist, string year, int firstSongId): base (id)
		{
			this.Album = album;
			this.Year = year;
			this.artist = artist;
			this.firstSongId = firstSongId;
		}
		
		public ArtistMusicItem Artist 
		{
			get	{			
				return artist;				
			}						
		}
			
		public int FirstSongId 
		{
			get	{		
				return firstSongId;				
			}						
		}
		
		public override string Icon 
		{
			get {				
				if (this.firstSongId >= 0) {
					return IconDownloader.GetIcon (Server.Instance.GetCoverUrl (this.firstSongId));					
				}
				// default:
				return "gtk-cdrom";								
			}
		}
		
		public override string Name 
		{
			get {
				return this.Album;
			}
		}
			
		public override string Description 
		{
			get {
				return "by " + (Artist == null ? "(unknown)" : Artist.Name);
			}
		}
		
		public override string SqueezeCenterIdKey 
		{
			get {
				return "album_id";
			}
		}
		
		public override int GetHashCode ()
		{
			return this.Id.GetHashCode();
		}
		
		public override bool Equals (object o)
		{
			return this.Id == (o as AlbumMusicItem).Id;
		}

	}

	public class ArtistMusicItem : MusicItem
	{
		public static readonly string SqueezeCenterKey = "artist_id";
		public readonly string Artist;
		
		public ArtistMusicItem (int id, string artist): base (id)
		{
			this.Artist = artist;			
		}

		public override string Description
		{
			get {
				return string.Format ("All music by {0}", Artist);
			}
		}
		
		public override string Name
		{
			get {
				return this.Artist;
			}
		}
		
		public override string Icon 
		{
			get {
				return "artist.svg@" + this.GetType ().Assembly.FullName;								
			}
		}
		
		public override string SqueezeCenterIdKey 
		{
			get {
				return "artist_id";
			}
		}				
	}
}
