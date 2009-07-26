/* MediaItems.cs
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

using Mono.Addins;

namespace Banshee
{
	public interface IMediaFile : IFileItem
	{
		string Year { get; }
		string Title { get; }
		string Artist { get; }
	}
	
	public class MediaItem : Item
	{
		protected string name, artist, year, cover;
		
		public MediaItem ()
		{
		}
		
		public MediaItem (string name, string artist, string year, string cover)
		{
			this.name = name;
			this.artist = artist;
			this.year = year;

			if (System.IO.File.Exists (cover))
				this.cover = cover;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return year == null ? Artist : string.Format ("{0} ({1})", artist, Year); }
		}
		
		public override string Icon {
			get { return cover ?? "applications-multimedia"; }
		}
		
		public virtual string Artist {
			get { return artist; }
		}
		
		public virtual string Year {
			get { return year; }
		}
		
		public virtual string Cover {
			get { return cover; }
		}
	}
	
	public class VideoItem : MediaItem, IMediaFile
	{
		string file;
		public VideoItem (string name, string artist, string year, string cover, string file) :
			base (name, artist, year, cover)
		{
			this.file = file;
		}
		
		public override string Icon {
			get { return cover ?? "video-x-generic"; }
		}
		
		public string Title {
			get { return Name; }
		}
		public string Path {
			get { return file; }
		}

		public string Uri {
			get { return string.Format ("file://{0}", Path); }
		}
	}
	
	public class PodcastItem : MediaItem
	{
		public PodcastItem ()
		{
		}
		
		public PodcastItem (string artist, string year, string cover) : base ()
		{
			this.artist = artist;
			this.year = year;
			this.cover = cover;
		}
		
		public override string Icon {
			get { return cover ?? "audio-x-generic"; }
		}
	}
	
	public class PodcastPublisherItem : PodcastItem
	{
		public PodcastPublisherItem (string artist, string year, string cover) :
			base (artist, year, cover)
		{
			this.name = artist;
		}
	}
	 
	public class PodcastPodcastItem : PodcastItem, IMediaFile
	{		
		string file;
		public PodcastPodcastItem (string name, string artist, string year, string cover, string file) :
			base (artist, year, cover)
		{
			this.name = name;
			this.file = file;
		}
		
		public override string Icon {
			get { return "audio-x-generic"; }
		}

		public string Title {
			get { return Name; }
		}
		
		public string Path {
			get { return file; }
		}

		public string Uri {
			get { return string.Format ("file://{0}", Path); }
		}
	}
			
	public class MusicItem : MediaItem
	{
		public MusicItem ()
		{
		}
		
		public MusicItem (string name, string artist, string year, string cover)
			: base (name, artist, year, cover)
		{
		}
		
		public override string Icon {
			get { return cover ?? "gtk-cdrom"; }
		}
	}
	
	public class AlbumMusicItem : MusicItem
	{
		public AlbumMusicItem (string name, string artist, string year, string cover) : 
			base (name, artist, year, cover)
		{
		}
	}
	
	public class ArtistMusicItem : MusicItem
	{
		public ArtistMusicItem (string name, string cover) : base ()
		{
			this.name = this.artist = name;
			this.cover = cover;
		}
		
		public override string Description {
			get { return string.Format ("{0} {1} {2}", 
				AddinManager.CurrentLocalizer.GetString ("All Music by"), artist, Year); }
		}
		
		public override string Icon {
			get { return cover ?? "audio-input-microphone"; }
		}
	}
	
	public class SongMusicItem : MusicItem, IMediaFile
	{
		string file, album, track;
		
		public SongMusicItem (string name, string artist, string album, string year,
			string cover, string track, string file) : base (name, artist, year, cover)
		{
			this.file = file;
			this.album = album;
			this.track = track;
		}
		
		public override string Description {
			get { return string.Format ("{0} - {1}", artist, album); }
		}
		
		public override string Icon {
			get { return "audio-x-generic"; }
		}

		public string Title {
			get { return Name; }
		}
		
		public string Path {
			get { return file; }
		}

		public string Uri {
			get { return string.Format ("file://{0}", Path); }
		}
		
		public string Album {
			get { return album; }
		}
		
		public string Track {
			get { return track != null ? track.PadLeft (3, '0') : string.Empty; }
		}
	}
}