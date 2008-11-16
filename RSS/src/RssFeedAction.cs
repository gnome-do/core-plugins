/* RssFeedAction.cs
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Do.Universe;

using Rss;

namespace Do.Plugins.Rss
{
	public class RssFeedAction : AbstractAction
	{
		private Dictionary<string, CachedFeed> cachedFeeds 
			= new Dictionary<string, CachedFeed>();

		public override string Name {
			get { return "RSS feed"; }
		}

		public override string Description {
			get { return "View RSS feed contents"; }
		}

		public override string Icon { get { return "feed-icon.png@" + GetType ().Assembly.FullName; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (RssFeedItem)
				};
			}
		}

		public override IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modifierItems)
		{
			List<IItem> rssItems = new List<IItem> ();
			string url = (items.First () as RssFeedItem).URL;
			RssFeed feed;
			if (cachedFeeds.ContainsKey (url)) {
				CachedFeed cachedFeed = cachedFeeds[url];
				if (cachedFeed.Expiry < DateTime.Now) {
					// Only fetch the feed if it's been modified since it
					// was last read.
					feed = RssFeed.Read (cachedFeeds[url].RssFeed, 
							RssItemSource.Timeout);
				}
				else {
					// use the locally cached results
					feed = cachedFeed.RssFeed;
				}
			}
			else {
				// This feed hasn't been requested yet.
				// Fetch the feed and add it to the cache.
				feed = RssFeed.Read (url, RssItemSource.Timeout);
				CachedFeed cachedFeed = new CachedFeed ();
				cachedFeed.Expiry = DateTime.Now.AddMinutes 
					(RssItemSource.CacheDuration);
				cachedFeed.RssFeed = feed;
				// Add the feed to the cache
				cachedFeeds.Add (url, cachedFeed);
			}

			if (feed.Channels.Count > 0) {
				foreach (RssItem rssItem in feed.Channels[0].Items) {
					string title = TidyHtml (rssItem.Title);
					string description = TidyHtml (rssItem.Description);
					RssFeedLinkItem linkItem = 
						new RssFeedLinkItem (title, rssItem.Link.ToString(), 
								             description);
					rssItems.Add (linkItem);
				}
			}
			return rssItems.ToArray();	
		}

		/// <summary>
		/// Strips HTML, newlines and replaces HTML entities
		/// </summary>
		/// <param name="input">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Processed output <see cref="System.String"/>
		/// </returns>
		protected string TidyHtml (string input)
		{
			string output = Regex.Replace (input, @"<(.|\n)*?>",string.Empty);
			// Having more than 60 chars can cause Do's window to grow too big
			if (output.Length > 60) {
				output = output.Remove (60);
			}
			output = output.Replace (Environment.NewLine, string.Empty);
			// replace entities
			output = HttpUtility.HtmlDecode (output); 
			return output;
		}
	}

	/// <summary>
	/// Class used to cache Rss feeds locally.
	/// RSS.NET uses the last modified header to only pull
	/// the feed if it's been modified since the last request,
	/// but this still requires sending a HttpWebRequest
	/// which slows Do down somewhat.
	/// </summary>
	public class CachedFeed
	{
		private DateTime expiry;
		private RssFeed rssFeed;

		public DateTime Expiry {
			get { return expiry; }
			set { expiry = value; }
		}

		public RssFeed RssFeed {
			get { return rssFeed; }
			set { rssFeed = value; }
		}
	}
}
