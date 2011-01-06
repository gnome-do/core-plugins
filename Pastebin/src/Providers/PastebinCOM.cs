//  PastebinCOM.cs
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
	public class PastebinCOM : AbstractPastebinProvider
	{
		const string url_root = "http://pastebin.com";
		const string content_key = "paste_code";
		const string syntax_key = "paste_format";
		
		public PastebinCOM ()
		{
			Name = "pastebin.com";
			BaseUrl = url_root + "/api_public.php";
			Expect100Continue = false;
			
			Parameters = new NameValueCollection();
			Parameters[syntax_key] = "text";
			Parameters[content_key] = "";
			Parameters["paste_expire_date"] = "1M";
			
			SupportedLanguages = PopulateTextSyntaxItemsFromXml ("Pastebin.xml");
		}
		
		public PastebinCOM(string content, string syntax) : this()
		{
			Parameters[syntax_key] = syntax;
			Parameters[content_key] = content;
		}
		
		public PastebinCOM(string content) : this()
		{
			Parameters[content_key] = content;
		}
		
		public override string GetPasteUrlFromResponse(HttpWebResponse response)
		{
			string responseText = String.Empty;
			using (Stream responseStream = response.GetResponseStream ())
				using (StreamReader reader = new StreamReader (responseStream))
					responseText = reader.ReadToEnd ();
			
			if (responseText.Contains ("ERROR")) {
				Log<PastebinCOM>.Debug (responseText);
				return String.Empty;
			}
			
			return responseText;
		}
	}
}
