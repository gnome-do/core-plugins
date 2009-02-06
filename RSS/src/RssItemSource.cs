/* RssItemSource
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
using System.Xml;

using Do.Universe;
using Do.Platform;
using Do.Platform.Linux;

namespace Do.Plugins.Rss
{
	public class RssItemSource : ItemSource, IConfigurable
	{
		private static List<Item> items;        
		private static IPreferences prefs;

		public RssItemSource()
		{
			items = new List<Item>();
			//UpdateItems ();  
		}

		static RssItemSource () {
			prefs = Services.Preferences.Get<RssItemSource> ();
		}
		
		public override string Name { get { return "RSS Feeds"; } }
		public override string Description { get { return "RSS Feeds from OPML"; } }
		public override string Icon { get { return "feed-icon.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				yield return typeof(RssFeedItem);
			}
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			yield break; 
		}

		
		/// <value>
		/// URI of the OPML file
		/// </value>
		public static string OpmlFile {
			get { 
				return prefs.Get<string> ("opmlfile", "http://www.scripting.com/feeds/top100.opml");
			}
			set {
				Log.Debug("Setting OPML file to " + value);
				prefs.Set<string> ("opmlfile", value);
			}
		}

		/// <value>
		/// Timeout period in seconds
		/// </value>
		public static int Timeout {
			get {
				return prefs.Get<int> ("timeout", 5);
			}
			set {
				prefs.Set<int> ("timeout", value);
			}
		}

		/// <value>
		/// Cache duration in minutes
		/// </value>
		public static int CacheDuration {
			get {
				return prefs.Get<int> ("cacheDuration", 10);
			}
			set {
				prefs.Set<int> ("cacheDuration", value);
			}
		}

		public override void UpdateItems () {
			// Assemble the path to the bookmarks xml file.
			string opmlFile = OpmlFile.Replace ("file://", "");

			items.Clear ();
			try {
				using (XmlReader reader = XmlReader.Create (opmlFile)) {
					while (reader.ReadToFollowing ("outline")) {
						string title, link;

						// Use the XmlReader to scan for the bookmark's title 
						// and link.
						reader.MoveToAttribute ("text");
						title = reader.Value;
						
						// If it doesn't find a URL, then this is a category outline
						if (reader.MoveToAttribute ("xmlUrl")) {
							link = reader.Value;
							// Create a new bookmark using the BookmarkItem class 
							// from the Do.Addins library. The Open URL action 
							// already knows how to open BookmarkItems.
							items.Add (new RssFeedItem (title, link));
						}						
					}
				}
			} catch (Exception e) {
				// Something went horribly wrong, so we print the error message.
				Log.Error("Could not read OPML file {0}: {1}", opmlFile, e.Message);
			}
		}

		public Gtk.Bin GetConfiguration () {
			return new Configuration ();
		}
	}
}
