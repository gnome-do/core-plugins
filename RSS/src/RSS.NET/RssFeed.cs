/* RssFeed.cs
 * ==========
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright © 2002 - 2005 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Rss
{
	/// <summary>The contents of a RssFeed</summary>
	[Serializable()]
	public class RssFeed
	{
		private List<RssChannel> channels = new List<RssChannel>();
		private List<RssModule> modules = new List<RssModule>();
		private List<Exception> exceptions = null;
		private DateTime lastModified = RssDefault.DateTime;
		private RssVersion rssVersion = RssVersion.Empty;
		private bool cached = false;
		private string etag = RssDefault.String;
		private string url = RssDefault.String;
		private Encoding encoding = null;
		/// <summary>Initialize a new instance of the RssFeed class.</summary>
		public RssFeed() {}
		/// <summary>Initialize a new instance of the RssFeed class with a specified encoding.</summary>
		public RssFeed(Encoding encoding)
		{ 
			this.encoding = encoding;
		}
		/// <summary>Returns a string representation of the current Object.</summary>
		/// <returns>The Url of the feed</returns>
		public override string ToString()
		{
			return url;
		}
		/// <summary>The channels that are contained in the feed.</summary>
		public List<RssChannel> Channels
		{
			get { return channels; }
		}
		/// <summary>The modules that the feed adhears to.</summary>
		public List<RssModule> Modules
		{
			get { return modules; }
		}
		/// <summary>A collection of all exceptions encountered during the reading of the feed.</summary>
		public List<Exception> Exceptions
		{
			get { return exceptions == null ? new List<Exception>() : exceptions; }
		}
		/// <summary>The Version of the feed.</summary>
		public RssVersion Version
		{
			get { return rssVersion; }
			set { rssVersion = value; }
		}
		/// <summary>The server generated hash of the feed.</summary>
		public string ETag
		{
			get { return etag; }
		}
		/// <summary>The server generated last modfified date and time of the feed.</summary>
		public DateTime LastModified
		{
			get { return lastModified; }
		}
		/// <summary>Indicates this feed has not been changed on the server, and the local copy was returned.</summary>
		public bool Cached
		{
			get { return cached; }
		}
		/// <summary>Location of the feed</summary>
		public string Url
		{
			get { return url; }
		}
		/// <summary>Encoding of the feed</summary>
		public Encoding Encoding	
		{
			get { return encoding; }
			set { encoding = value; }
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="url">The url or filename of the RSS feed</param>
		/// <returns>The contents of the feed</returns>
		public static RssFeed Read(string url, int timeout)
		{
			return read(url, null, null, timeout);
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="Request">The specified way to connect to the web server</param>
		/// <returns>The contents of the feed</returns>
		public static RssFeed Read(HttpWebRequest Request, int timeout)
		{
			return read(Request.RequestUri.ToString(), Request, null, timeout);
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="oldFeed">The cached version of the feed</param>
		/// <returns>The current contents of the feed</returns>
		/// <remarks>Will not download the feed if it has not been modified</remarks>
		public static RssFeed Read(RssFeed oldFeed, int timeout)
		{
			return read(oldFeed.url, null, oldFeed, timeout);
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="Request">The specified way to connect to the web server</param>
		/// <param name="oldFeed">The cached version of the feed</param>
		/// <returns>The current contents of the feed</returns>
		/// <remarks>Will not download the feed if it has not been modified</remarks>
		public static RssFeed Read(HttpWebRequest Request, RssFeed oldFeed, int timeout)
		{
			return read(oldFeed.url, Request, oldFeed, timeout);
		}
		private static RssFeed read(string url, HttpWebRequest request, RssFeed oldFeed, int timeout)
		{
			// ***** Marked for substantial improvement
			RssFeed feed = new RssFeed();
			RssElement element = null;
			Stream stream = null;
			Uri uri = new Uri(url);
			feed.url = url;

			switch (uri.Scheme)
			{	
				case "file":
					feed.lastModified = File.GetLastWriteTime(uri.AbsolutePath);
				if ((oldFeed != null) && (feed.LastModified == oldFeed.LastModified))
				{
					oldFeed.cached = true;
					return oldFeed;
				}
				stream = new FileStream(uri.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				break;
				case "https":
					goto case "http";
				case "http":
					if (request == null)
						request = (HttpWebRequest)WebRequest.Create(uri);
				request.Timeout = timeout * 1000;
				if (oldFeed != null)
				{
					request.IfModifiedSince = oldFeed.LastModified;
					request.Headers.Add("If-None-Match", oldFeed.ETag);
				}
				try
				{
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					feed.lastModified = response.LastModified;
					feed.etag = response.Headers["ETag"];
					try 
					{ 
						if (response.ContentEncoding != "")
							feed.encoding = Encoding.GetEncoding(response.ContentEncoding); 
					}
					catch {}
					stream = response.GetResponseStream();
				}
				catch (WebException we)
				{
					if (oldFeed != null)
					{
						oldFeed.cached = true;
						return oldFeed;
					}
					else throw we; // bad
				}
				break;
			}

			if (stream != null)
			{
				RssReader reader = null;
				try
				{
					reader = new RssReader(stream);
					do
					{
						element = reader.Read();
						if (element is RssChannel)
							feed.Channels.Add((RssChannel)element);
					}
					while (element != null);
					feed.rssVersion = reader.Version;
				}
				finally
				{
					feed.exceptions = reader.Exceptions;
					reader.Close();
				}
			}
			else
				throw new ApplicationException("Not a valid Url");

			return feed;
		}
	}
}
