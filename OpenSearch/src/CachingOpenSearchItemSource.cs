//  OpenSearchItemSource.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do;
using Do.Platform;
using Do.Universe;

namespace OpenSearch
{
	public class CachingOpenSearchItemSource 
	{	
		static readonly string valid_file_pattern = @"^.*\.xml$";		
		static Dictionary<string, Item> cached_items;
		static FirefoxOpenSearchDirectoryProvider firefox_provider;
		
		static CachingOpenSearchItemSource ()
		{
			cached_items = new Dictionary<string, Item> ();
			firefox_provider = new FirefoxOpenSearchDirectoryProvider ();
			UpdateItems();
		}				

		public static IEnumerable<Item> Items {
			get {
				return cached_items.Values;
			}
		}
		
		public static void UpdateItems ()
		{
			foreach (string filePath in GetUnprocessedOpenSearchFiles (firefox_provider.OpenSearchPluginDirectories)) {
				try {
					OpenSearchItem item = OpenSearchParser.Create (filePath);
					if (item != null) {
						cached_items.Add (filePath, item);
						Log<CachingOpenSearchItemSource>.Debug ("Adding new OpenSearch plugin: {0}", filePath);
					}
				} catch (Exception e) {
					Log<CachingOpenSearchItemSource>.Debug ("Error adding new OpenSearch plugin: {0} {1}", filePath, e.Message);
					Log<CachingOpenSearchItemSource>.Debug (e.StackTrace);
					continue;
				}
			}
		}	
		
		private static IEnumerable<string> GetUnprocessedOpenSearchFiles (IEnumerable<string> directoriesToProcess)
		{
			List<string> unprocessedFiles = new List<string> ();
			
			foreach (string path in directoriesToProcess) {

				if(!Directory.Exists (path))
					continue;

				IEnumerable<string> filePaths = Directory.GetFiles (path).Concat (Directory.GetDirectories (path));

				foreach (string filePath in filePaths) {
					// It's a trap! The firefox-addons/searchplugins folder stashes some of its plugins in folders
					// so we need to recurse...but it also has a symlink called common which links to it's containing 
					// folder, which means if we blindly recurse, we'll keep following it. So let's just skip it.
					if((File.GetAttributes(filePath) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint) {
						Log<CachingOpenSearchItemSource>.Debug ("Skipping symlink: {0}", filePath);
						continue;		
					}
					if (Directory.Exists(filePath)) {
						Log<CachingOpenSearchItemSource>.Debug ("Recursing into: {0}",filePath);
					    unprocessedFiles.AddRange (GetUnprocessedOpenSearchFiles (new[]{filePath}));
					}
					if (cached_items.ContainsKey (filePath))
						continue;
					if (!Regex.IsMatch (filePath, valid_file_pattern))
						continue;		
					
					unprocessedFiles.Add (filePath);
				}
			}
			return unprocessedFiles;
		}
	}
}
