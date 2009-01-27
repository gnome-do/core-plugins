//  PastebinCA.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify it
//  under the terms of the GNU General Public License as published by the Free
//  Software Foundation, either version 3 of the License, or (at your option)
//  any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//  more details.
//
//  You should have received a copy of the GNU General Public License along with
//  this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

using Do.Platform;

namespace Pastebin
{
	public class PastebinCA : AbstractPastebinProvider
	{
		const string url_root = "http://pastebin.ca";
		const string content_key = "content";
		const string syntax_key = "type";
		
		public PastebinCA ()
		{
			Name = "pastebin.ca";
			BaseUrl = url_root + "/quiet-paste.php";
			Expect100Continue = false;
			
			Parameters = new NameValueCollection();
			Parameters["api"] = "4xPQUdtxHQ9wxlAJ9t/ztpv36MM/ZE9G";
			Parameters[syntax_key] = "1";
			Parameters["description"] = "";
			Parameters[content_key] = "";
			Parameters["name"] = "";
			Parameters["expiry"] = "1 month";
			
			SupportedLanguages = PopulateTextSyntaxItemsFromXml ("PastebinCA.xml");
		}
		
		public PastebinCA(string content, string syntax)
			: this()
		{
			Parameters[syntax_key] = syntax;
			Parameters[content_key] = content;
		}
		
		public PastebinCA(string content)
			: this()
		{
			Parameters[content_key] = content;
		}
		
		public override string GetPasteUrlFromResponse(HttpWebResponse response)
		{
			string responseText;
			using (Stream responseStream = response.GetResponseStream ()) {
				using (StreamReader reader = new StreamReader (responseStream)) {
					responseText = reader.ReadToEnd ();
				}
			}
			
			string url = String.Empty;
			if (responseText.Contains ("SUCCESS")) {
				url = url_root + "/" + responseText.Split (new string[]{":"}, StringSplitOptions.RemoveEmptyEntries)[1];
			} else {
				Log<PastebinCA>.Debug (responseText);
			}
			
			return url;
		}
	}
}