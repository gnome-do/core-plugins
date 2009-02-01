/* Monoport.cs 
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
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using Do.Platform;

namespace Pastebin
{
	public class Monoport : AbstractPastebinProvider
	{
		const string url_root = "http://monoport.com";
		const string syntax_key = "format";
		const string content_key = "code2";
		
		public Monoport ()
		{
			Name = "Monoport";
			BaseUrl = url_root + "/pastebin.php";
						
			Parameters = new NameValueCollection ();
			Parameters ["poster"] = System.Environment.UserName;
			Parameters [syntax_key] = "text";
			Parameters ["parent_pid"] = "";
			Parameters ["paste"] = "send";
			Parameters ["expiry_day"] = "d";
			Parameters [content_key] = "";
			
			UserAgent = "monoporter";
			
			SupportedLanguages = PopulateTextSyntaxItemsFromXml ("Monoport.xml");
		}

		public Monoport (string content) : this ()
		{
			Parameters [content_key] = content;
		}

		public Monoport (string content, string syntax) : this ()
		{
			Parameters [syntax_key] = syntax;
			Parameters [content_key] = content;
		}

		public override string GetPasteUrlFromResponse (HttpWebResponse response)
		{
			string responseText;
			using (Stream responseStream = response.GetResponseStream ()) {
				using (StreamReader reader = new StreamReader (responseStream)) {
					responseText = reader.ReadToEnd ();
				}
			}

			Regex urlPattern = new Regex ("<a href=\"\\/pastebin\\.php\\?dl=[0-9]*"); 
			Match urlMatch = urlPattern.Match (responseText);
			string url = urlMatch.Value;

			if (url == string.Empty) {
				Log<Monoport>.Debug (responseText);
				throw new Exception ("Parsed url was empty. Monoport may have changed their format.");
			}
			
			return url_root + "/" + url.Substring (url.LastIndexOf ('=') + 1);
		}
	}
}
