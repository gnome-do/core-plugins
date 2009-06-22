/* VNCHostSource.cs

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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace VinagreVNC 
{  
	public class VNCHostItem : ItemSource 
	{
		List<Item> items;

		public VNCHostItem ()
		{
        	    items = new List<Item> ();
	        }

		public override string Name { 
			get { return AddinManager.CurrentLocalizer.GetString ("Vinagre Bookmarks"); }
		}

		public override string Description { 
			get { return AddinManager.CurrentLocalizer.GetString ("Indexes your Vinagre Bookmarks"); }
		}

		public override string Icon {
			get { return "gnome-globe"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (HostItem);
				yield return typeof (VNCHostItem);
				yield return typeof (IApplicationItem);
				yield return typeof (VinagrgeBrowseBookmarksItem);
			}
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (IsVinagre (item)) {
				yield return new VinagrgeBrowseBookmarksItem ();
			} else if (item is VinagrgeBrowseBookmarksItem) {
				foreach (Item host in Items)
					yield return host;
			}
		}
		
		bool IsVinagre (Item item) 
		{
			return item.Equals (Do.Platform.Services.UniverseFactory.MaybeApplicationItemFromCommand ("vinagre"));
		}

		public override void UpdateItems ()
		{
			XmlDocument xml;
			XmlNodeList elements;
			string bookmarksFile;
		
			items.Clear ();
			xml = new XmlDocument ();
			bookmarksFile = new [] {ReadXdgUserDir ("XDG_DATA_HOME", ".local/share"), "vinagre", "vinagre-bookmarks.xml"}.Aggregate (Path.Combine);
		
			try {
				xml.Load (bookmarksFile);
			
				elements = xml.GetElementsByTagName ("item");
		
				foreach (XmlNode node in elements) {
					string bookmark = "", host = "", port = "";
				
					foreach (XmlNode child in node.ChildNodes) {
						switch (child.Name) {
						case "name":
							bookmark = child.InnerText;
							break;
						case "host":
							host = child.InnerText;
							break;
						case "port":
							port = child.InnerText;
							break;
						}
					}
				
					items.Add (new HostItem (bookmark, host, port));
				}
			} catch (FileNotFoundException e) {
				Log.Debug ("Cound not find vinagre bookmarks file {0}", bookmarksFile);
			} catch (XmlException e) {
				Log.Debug ("An error occured parsing bookmarks file {0}:", e.Message);
			}
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
			} catch (FileNotFoundException e) {
			}

			return Path.Combine (home_dir, fallback);
		}
	}
}

