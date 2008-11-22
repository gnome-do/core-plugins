/* BansheeIndexer.cs
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
using Do.Addins;
using Banshee.Collection.Indexer.RemoteHelper;

namespace Banshee1
{
	public class BansheeIndexer : SimpleIndexerClient
	{
		static List<IItem> songs;
		static object songs_mutex;
		
		static DateTime last_index;
		readonly string[] export_fields = new string[] {"name", "artist", "year", "album", "URI"};
		
		static BansheeIndexer ()
		{
			songs = new List<IItem> ();
			songs_mutex = new object (); 
			last_index = DateTime.MinValue;
		}
			
		public BansheeIndexer ()
		{
			AddExportField (export_fields);
		}
		
		public static List<IItem> Songs {
			get { lock (songs_mutex) { return songs; } }
		}
		
		protected override void IndexResult (IDictionary<string, object> result)
		{
			try {
				SongMusicItem song = new SongMusicItem ((string) result["name"], (string) result["artist"],
					(string) result["album"], "YEAR", null, (string) result["URI"]);
				//Console.Error.WriteLine (((DateTime) result["year"]).Year);
				
				if (!songs.Contains (song)) {
					lock (songs_mutex) {
						songs.Add (song);
					}
				}
			} catch (KeyNotFoundException e) {
				Log.Error ("{0}", e);
			}
		}
		
		protected override int CollectionCount {
			get { lock (songs_mutex) { return songs.Count; } }
		}
		
		protected override DateTime CollectionLastModified {
			get { return last_index; }
		}
		
		protected override void OnShutdownWhileIndexing ()
		{
			Log.Info ("Banshee requested a shutdown. Stopping indexing");
		}
	}
}
