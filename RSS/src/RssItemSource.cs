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

using GConf;

using Do.Addins;
using Do.Universe;

namespace Do.Plugins.Rss
{
	public class RssItemSource : IItemSource, IConfigurable
	{
		private static List<IItem> items;        
		private static GConf.Client gconfClient;

		public RssItemSource()
		{
			items = new List<IItem>();
			//UpdateItems ();  
		}

		public string Name { get { return null; } }
		public string Description { get { return null; } }
		public string Icon { get { return "feed-icon.png@" + GetType ().Assembly.FullName; } }
		
		public IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] { typeof(RssFeedItem) };
			}
		}

		public IEnumerable<IItem> Items {
			get { return items; }
		}

		public IEnumerable<IItem> ChildrenOfItem (IItem parent)
		{
			return null;
		}

		public static GConf.Client GConfClient {
			get {
				if(gconfClient == null) {
					gconfClient = new GConf.Client ();
				}
				return gconfClient;
			}   
		}
		/// <value>
		/// URI of the OPML file
		/// </value>
		public static string OpmlFile {
			get {
				string opmlFile;
				try {
					opmlFile = GConfClient.Get (GConfKeyBase + "opmlfile") as string;
				} catch (GConf.NoSuchKeyException) {
					opmlFile = "http://www.scripting.com/feeds/top100.opml"; 
				}
				return opmlFile;
			}

			set {
				if(value != null)
					GConfClient.Set (GConfKeyBase + "opmlfile", value);
			}
		}

		/// <value>
		/// Timeout period in seconds
		/// </value>
		public static int Timeout {
			get {
				int timeout;
				try {
					timeout = (int)GConfClient.Get (GConfKeyBase + "timeout");
				} catch (GConf.NoSuchKeyException) {
					timeout = 5;
				}
				return timeout;
			}

			set {
				GConfClient.Set(GConfKeyBase + "timeout", value);
			}
		}

		/// <value>
		/// Cache duration in minutes
		/// </value>
		public static int CacheDuration {
			get {
				int cacheDuration;
				try {
					cacheDuration = 
						(int)GConfClient.Get (GConfKeyBase + "cacheDuration");
				} catch (GConf.NoSuchKeyException) {
					cacheDuration = 10;
				}
				return cacheDuration;
			}

			set {
				GConfClient.Set(GConfKeyBase + "cacheDuration", value);
			}
		}

		private static string GConfKeyBase {
			get { return "/apps/gnome-do/plugins/rss/"; }
		}

		public void UpdateItems () {
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
						reader.MoveToAttribute ("xmlUrl");
						link = reader.Value;

						// Create a new bookmark using the BookmarkItem class 
						// from the Do.Addins library. The Open URL action 
						// already knows how to open BookmarkItems.
						items.Add (new RssFeedItem (title, link));
					}
				}
			} catch (Exception e) {
				// Something went horribly wrong, so we print the error message.
				Console.Error.WriteLine ("Could not read OPML file {0}: {1}",
						opmlFile, e.Message);
			}
		}

		public Gtk.Bin GetConfiguration () {
			return new Configuration ();
		}
	}
}
