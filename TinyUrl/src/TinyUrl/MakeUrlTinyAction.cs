// MakeUrlTinyAction.cs
//
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Unix;

using Do.Universe;
using Do.Universe.Common;

namespace TinyUrl
{

	public class MakeUrlTinyAction : Act
	{

		const string TinyUrlScript = "http://tinyurl.com/api-create.php";
		const string UrlArgumentName = "url";

		// URL regex taken from http://www.osix.net/modules/article/?id=586
		const string UrlPattern = "^(https?://)"
		       + "?(([0-9a-zA-Z_!~*'().&=+$%-]+: )?[0-9a-zA-Z_!~*'().&=+$%-]+@)?" //user@
		       + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
		       + "|" // allows either IP or domain
		       + @"([0-9a-zA-Z_!~*'()-]+\.)*" // tertiary domain(s)- www.
		       + @"([0-9a-zA-Z][0-9a-zA-Z-]{0,61})?[0-9a-zA-Z]\." // second level domain
		       + "[a-zA-Z]{2,6})" // first level domain- .com or .museum
		       + "(:[0-9]{1,4})?" // port number- :80
		       + "((/?)|" // a slash isn't required if there is no file name
		       + "(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?) *$";

		readonly Regex url_regex;

		public MakeUrlTinyAction ()
		{
			url_regex = new Regex (UrlPattern, RegexOptions.Compiled);
		}
		
		public override string Name {
			get { return Catalog.GetString ("Make Tiny Url"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Creates a TinyUrl from an unwieldy mess."); }
		}
		
		public override string Icon {
			get { return "web-browser"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (IUrlItem);
			}
		}

		public override bool SupportsItem (Item item)
		{
			return url_regex.IsMatch (GetUrl (item));
		}

		string GetUrl (Item item)
		{
			if (item is ITextItem)
				return GetUrl (item as ITextItem);
			if (item is IUrlItem)
				return GetUrl (item as IUrlItem);
			throw new Exception ("Unsupported item type");
		}

		string GetUrl (ITextItem item)
		{
			return item.Text;
		}
		
		string GetUrl (IUrlItem item)
		{
			return item.Url;
		}

		string MakeTiny (string url)
		{
			if (url == null) throw new ArgumentNullException ("url");
			
			return GetTinyUrlRequest (CreateRequestUrl (url));
		}

		string GetTinyUrlRequest (string url)
		{
			if (url == null) throw new ArgumentNullException ("url");
			
			string result;
			WebRequest request = WebRequest.Create (url);
			using (WebResponse response = request.GetResponse ())
				using (StreamReader reader =
						new StreamReader (response.GetResponseStream ()))
					result = reader.ReadLine ();
			return result;
		}

		string CreateRequestUrl (string url)
		{
			if (url == null) throw new ArgumentNullException ("url");
			
			return string.Format ("{0}?{1}={2}",
					TinyUrlScript, UrlArgumentName, HttpUtility.UrlEncode (url));
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			return items
				.Select (item => new TextItem (MakeTiny (GetUrl (item))) as Item);
		}
	}
}
