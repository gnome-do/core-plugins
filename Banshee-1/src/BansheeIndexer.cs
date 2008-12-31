/* Banshee.cs
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
 * 
 * THE PHILLIES WON TONIGHT! WE'RE GOING TO THE WORLD SERIES!!!!!
 */


using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Banshee.Collection.Indexer.RemoteHelper;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Banshee
{
	public class BansheeIndexer : SimpleIndexerClient
	{
		DateTime last_index;
		object indexing_mutex;
		string artwork_directory;
		
		List<VideoItem> videos;
		List<SongMusicItem> songs;
		List<PodcastItem> podcasts;
		
		readonly string[] export_fields = new [] {"name", "artist", "year", "album", "local-path", "URI", "media-attributes", "artwork-id", "track-number"};
		
		public BansheeIndexer ()
		{
			indexing_mutex = new object ();
			videos = new List<VideoItem> ();
			songs = new List<SongMusicItem> ();
			podcasts = new List<PodcastItem> ();

			Videos = Enumerable.Empty<VideoItem> ();
			Songs = Enumerable.Empty<SongMusicItem> ();
			Podcasts = Enumerable.Empty<PodcastItem> ();

			last_index = DateTime.MinValue;

			AddExportField (export_fields);
			//artwork_directory = Path.Combine (Paths.ReadXdgUserDir ("XDG_CACHE_DIR", ".cache"), "album-art");
			artwork_directory = "/home/alex/.cache/album-art";
		}

		public IEnumerable<VideoItem> Videos { get; private set; }
		public IEnumerable<SongMusicItem> Songs { get; private set; }
		public IEnumerable<PodcastItem> Podcasts { get; private set; }
		
		/// <summary>
		/// This gets called on a thread from SimpleIndexerClient so we need to be careful with our lists.
		/// </summary>
		/// <param name="result">
		/// A <see cref="IDictionary"/>
		/// </param>
		protected override void IndexResult (IDictionary<string, object> result)
		{
			MediaItem item;
			string path, artPath, mediaType;
			Dictionary<string, string> exports;
					
			exports = SetupExports ();
			foreach (string export in export_fields) {
				object objExport;
				
				result.TryGetValue (export, out objExport);
				exports [export] = (objExport == null) ? "" : objExport.ToString ();
			}
			
			mediaType = exports ["media-attributes"];

			// some items dont have a local-path, we need to use the URI in this case.
			path = string.IsNullOrEmpty (exports ["local-path"]) ? exports ["URI"] : exports ["local-path"];
			artPath = string.IsNullOrEmpty (exports ["artwork-id"]) ? "" : Path.Combine (artwork_directory, exports ["artwork-id"] + ".jpg");
			Console.Error.WriteLine (path);
			lock (indexing_mutex) {
			
				//Handle videos in the collection
				if (mediaType.Contains ("VideoStream")) {		
					item = new VideoItem (exports ["name"], exports ["artist"], exports ["year"], artPath, path);
	
					videos.Add (item as VideoItem);
				
				//Handle the podcasts in collection
				} else if (mediaType.Contains ("Podcast")) {
					item = new PodcastPodcastItem (exports ["name"], exports ["album"], exports ["year"], artPath, path);
					
					podcasts.Add (item as PodcastPodcastItem);
				
				//everything else should be Music
				} else {
					item = new SongMusicItem (exports ["name"], exports ["artist"], exports ["album"], exports ["year"], 
						artPath, exports ["track-number"], path);
					
					songs.Add (item as SongMusicItem);
				}
			}
		}
		
		protected override int CollectionCount {
			get { return Songs.Count () + Videos.Count () + Podcasts.Count (); }
		}
		
		protected override DateTime CollectionLastModified {
			get { return last_index; }
		}
		
		protected override void OnStarted ()
		{
			Log.Debug (Catalog.GetString ("Starting Banshee indexer"));
		}

		protected override void OnEndUpdateIndex()
		{
			last_index = DateTime.Now;
		
			CopyLists ();
			
			Log.Debug (Catalog.GetString ("Finished indexing Banshee library"));
			Log.Debug ("" + CollectionCount);
		}

		protected override void OnShutdownWhileIndexing ()
		{
			CopyLists ();
			Log.Info (Catalog.GetString ("Banshee requested a shutdown. Stopping indexer"));
		}
		
		void CopyLists ()
		{
			lock (indexing_mutex) {
				Videos = new List<VideoItem> (videos);
				Songs = new List<SongMusicItem> (songs);
				Podcasts = new List<PodcastItem> (podcasts);
	
				songs.Clear ();
				videos.Clear ();
				podcasts.Clear ();
			}
		}
		
		Dictionary<string, string> SetupExports ()
		{
			Dictionary<string, string> exports = new Dictionary<string, string> ();
			foreach (string export in export_fields) {
				exports.Add (export, "");
			}

			return exports;
		}
	}
}