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
		string artwork_directory;
		List<IDictionary<string, object>> indexed_items;
		
		readonly string[] export_fields = new [] {"name", "artist", "year", "album", "local-path", "URI", "media-attributes", "artwork-id", "track-number"};
		
		public BansheeIndexer ()
		{
			AddExportField (export_fields);
			processing_mutex = new object ();
			IndexWhenCollectionChanged = false;
			artwork_directory = Path.Combine (ReadXdgUserDir ("XDG_CACHE_DIR", ".cache"), "album-art");
		
			Videos = Enumerable.Empty<VideoItem> ();
			Songs = Enumerable.Empty<SongMusicItem> ();
			Podcasts = Enumerable.Empty<PodcastItem> ();
			
			indexed_items = new List<IDictionary<string, object>> ();
		}

		public IEnumerable<VideoItem> Videos { get; private set; }
		public IEnumerable<SongMusicItem> Songs { get; private set; }
		public IEnumerable<PodcastItem> Podcasts { get; private set; }

#region SimpleIndexerClient overrides

		/// <summary>
		/// This gets called on a thread from SimpleIndexerClient so we need to be careful with our lists.
		/// </summary>
		/// <param name="result">
		/// A <see cref="IDictionary"/>
		/// </param>
		protected override void IndexResult (IDictionary<string, object> result)
		{
			indexed_items.Add (result);
		}
		
		protected override int CollectionCount {
			get { return Songs.Count () + Videos.Count () + Podcasts.Count (); }
		}
		
		protected override DateTime CollectionLastModified {
			get { return DateTime.UtcNow; }
		}
		
		protected override void OnBeginUpdateIndex()
		{
			Log.Debug ("Reading Banshee index results from DBus");
		}

		protected override void OnEndUpdateIndex()
		{
			ProcessesList ();
			indexed_items.Clear ();
			Log.Debug ("Finished indexing Banshee library, Found {0} media items", CollectionCount);
		}

		protected override void OnShutdownWhileIndexing ()
		{
			Log.Info ("Banshee requested a shutdown. Stopping indexer");
		}

#endregion

		void ProcessesList ()
		{
			List<VideoItem> videos = new List<VideoItem> ();
			List<SongMusicItem> songs = new List<SongMusicItem> ();
			List<PodcastItem> podcasts = new List<PodcastItem> ();

			foreach (IDictionary<string, object> result in indexed_items)
			{
				IMediaFile item;
				string path, artPath, mediaType;
				Dictionary<string, string> tags;
				
				tags = SetupTags ();
				foreach (string tag in export_fields) {
					object objTag;
					
					result.TryGetValue (tag, out objTag);
					tags [tag] = (objTag == null) ? "" : objTag.ToString ();
				}
				
				mediaType = tags ["media-attributes"];
	
				// some items dont have a local-path, we need to use the URI in this case.
				path = string.IsNullOrEmpty (tags ["local-path"]) ? tags ["URI"] : tags ["local-path"];
				artPath = string.IsNullOrEmpty (tags ["artwork-id"]) ? "" : Path.Combine (artwork_directory, tags ["artwork-id"] + ".jpg");
				
				//Handle videos in the collection
				if (mediaType.Contains ("VideoStream")) {		
					item = new VideoItem (tags ["name"], tags ["artist"], tags ["year"], artPath, path);
	
					videos.Add (item as VideoItem);
				
				//Handle the podcasts in collection
				} else if (mediaType.Contains ("Podcast")) {
					item = new PodcastPodcastItem (tags ["name"], tags ["album"], tags ["year"], artPath, path);
					
					podcasts.Add (item as PodcastPodcastItem);
				
				//everything else should be Music
				} else {
					item = new SongMusicItem (tags ["name"], tags ["artist"], tags ["album"], tags ["year"], 
						artPath, tags ["track-number"], path);
					
					songs.Add (item as SongMusicItem);
				}
					
				Videos = videos;
				Songs = songs;
				Podcasts = podcasts;
			}
		}
		
		Dictionary<string, string> SetupTags ()
		{
			Dictionary<string, string> tags = new Dictionary<string, string> ();
			foreach (string tag in export_fields) {
				tags.Add (tag, "");
			}

			return tags;
		}

		string ReadXdgUserDir (string key, string fallback)
		{
			string home_dir, config_dir, env_path, user_dirs_path;

			home_dir = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			config_dir = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);

			env_path = Environment.GetEnvironmentVariable (key);
			if (!String.IsNullOrEmpty (env_path)) {
				return env_path;
			}

			user_dirs_path = Path.Combine (config_dir, "user-dirs.dirs");
			if (!File.Exists (user_dirs_path)) {
				return Path.Combine (home_dir, fallback);
			}

			try {
				using (StreamReader reader = new StreamReader (user_dirs_path)) {
					string line;
					while ((line = reader.ReadLine ()) != null) {
						line = line.Trim ();
						int delim_index = line.IndexOf ('=');
						if (delim_index > 8 && line.Substring (0, delim_index) == key) {
							string path = line.Substring (delim_index + 1).Trim ('"');
							bool relative = false;

							if (path.StartsWith ("$HOME/")) {
								relative = true;
								path = path.Substring (6);
							} else if (path.StartsWith ("~")) {
								relative = true;
								path = path.Substring (1);
							} else if (!path.StartsWith ("/")) {
								relative = true;
							}
							return relative ? Path.Combine (home_dir, path) : path;
						}
					}
				}
			} catch (FileNotFoundException) {
			}
			return Path.Combine (home_dir, fallback);
		}
	}
}