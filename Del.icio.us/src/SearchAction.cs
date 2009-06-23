/* SearchAction.cs
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
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Mono.Addins;

using Do.Universe;
using Do.Universe.Common;

namespace Delicious
{
	public class DeliciousCertify : ICertificatePolicy
	{
		public bool CheckValidationResult (ServicePoint sp, X509Certificate cert, WebRequest chain, int errs)
		{
			return true;
		}
	}

	public class SearchAction : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Search del.icio.us"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("del.icio.us tag search"); }
		}
		
		public override string Icon {
			get { return "bookmark-new"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string tags = (items.First () as ITextItem).Text.Replace(" ","+");

			string url = "https://api.del.icio.us/v1/posts/recent?tag=" + tags;
			//Console.WriteLine (url);
			HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;

			ServicePointManager.CertificatePolicy = new DeliciousCertify();

			//Console.WriteLine ("made it");

			string username;
			string password;
			
			username = Delicious.Preferences.Username;
			password = Delicious.Preferences.Password;
			
			request.Credentials = new NetworkCredential (username, password);
			request.Method = "POST";

			XmlTextReader reader;

			List<Item> hits = new List<Item> ();
			try {
				HttpWebResponse response = request.GetResponse () as HttpWebResponse;
				reader = new XmlTextReader (response.GetResponseStream ());
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
				hits.Add (new BookmarkItem ("See everybody's...", "http://del.icio.us/tag/" + tags));
				return hits.ToArray ();
			}

			while (reader.Read ()) {
				if (reader.Name == "post")
					hits.Add (new BookmarkItem (reader.GetAttribute ("description"), reader.GetAttribute ("href")));
			}

			hits.Add (new BookmarkItem ("See all mine...", "http://del.icio.us/search/?type=user&p=" + tags));
			hits.Add (new BookmarkItem ("See everybody's...", "http://del.icio.us/tag/" + tags));
			
			return hits;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { return new Type [] {}; }
		}

		public bool ModifierItemsOptional {
			get { return true; }
		}
				
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return true;
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			yield break;
		}
		
	}
}
