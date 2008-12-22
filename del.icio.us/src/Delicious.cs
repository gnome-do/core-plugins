/* Delicious.cs
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
using System.Net;
using System.Web;
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Do.Universe;

namespace Delicious
{	
	public class Delicious
	{
		private static Dictionary<string,List<Item>> bookmarks; 
		private static object book_lock;
		
		private static string username;
		private static string password;
		//private static string new_tags = "dookie";
		
		static Delicious ()
		{	
			username = password = "";
			
			bookmarks = new Dictionary<string,List<Item>> ();
			book_lock = new object ();
			
			Connect ();
		}
		
		public static void Connect ()
		{
			Configuration.GetAccountData (out username, out password, typeof (Configuration));
		}
		
		public static Dictionary<string, List<Item>> Tags {
			get { return bookmarks; }
		}
		
		public static List<Item> BookmarksForTag (string tag)
		{
			List<Item> list = null;
			bookmarks.TryGetValue (tag.ToLower (), out list);
			return list;
		}
		
		public static void UpdateBookmarks ()
		{
			//if (!NeedsUpdated ()) return;
			
			XmlTextReader reader = GetXMLFromWebRequest ("https://api.del.icio.us/v1/posts/all", 
				username, password);
			
			if (reader == null) return;
			
			if (!Monitor.TryEnter (book_lock)) return;
			
			bookmarks.Clear ();
			bookmarks ["all bookmarks"] = new List<Item> ();
			try {
				while (reader.Read ()) {
					if (reader.Name == "post") {
						BookmarkItem bookmark =  new BookmarkItem (
							reader.GetAttribute ("description"), reader.GetAttribute ("href"));
						bookmarks ["all bookmarks"].Add (bookmark);
						string [] itemTags;
						itemTags = reader.GetAttribute ("tag").Split (' ');
						foreach (string tag in itemTags) {
							string t = tag.ToLower ();
							if (string.IsNullOrEmpty (tag))
								t = "untagged";
							if (!bookmarks.ContainsKey (tag))
								bookmarks [t] = new List<Item> ();
							bookmarks [t].Add (bookmark);
						}
					}
				}
			} catch (NullReferenceException e) {
				Console.Error.WriteLine (e.Message);
			}
			finally {
				Monitor.Exit (book_lock);
			}
		}
		
		/*
		public string NewBookmarkTags {
			set { new_tags = value; }
		}
		
		
		public static void NewBookmark (object url)
		{
			string postUrl = "https://api.del.icio.us/v1/posts/add?url={0}&description=fromgnomedo";
			postUrl = string.Format (postUrl, (url as string));
			Console.Error.WriteLine (postUrl);
			XmlTextReader reader = GetXMLFromWebRequest (postUrl, username, password);
			while (reader.Read ()) {
				if (reader.Name == "result") {
					if (!reader.GetAttribute ("code").Equals ("done"))
						Console.Error.WriteLine ("del.icio.us error: {0}",
							reader.GetAttribute ("code"));
				}
			}
		}
		*/

		private static XmlTextReader GetXMLFromWebRequest (string url, string username, string password)
		{
			HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
			
			ServicePointManager.CertificatePolicy = new DeliciousCertify();
			request.Credentials = new NetworkCredential (username, password);
			request.UserAgent = "GNOME-Do";
			request.Method = "POST";
			
			try {
				HttpWebResponse response = request.GetResponse () as HttpWebResponse;
				return new XmlTextReader (response.GetResponseStream ());
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return null;
			} 
		}
	}
}