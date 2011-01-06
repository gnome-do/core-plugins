//  LodgeIt.cs
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
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Platform;

namespace Pastebin
{
	public class LodgeIt : AbstractPastebinProvider
	{		
		const string url_root = "http://paste.pocoo.org";
		const string content_key = "code";
		const string syntax_key = "language";
		
		public LodgeIt ()
		{
			Name = "paste.pocoo.org";
			BaseUrl = url_root;
			ShouldAllowAutoRedirect = false;
			Expect100Continue = false;
			
			Parameters[content_key] = "";
			Parameters[syntax_key] = "text";
			 
			SupportedLanguages = PopulateTextSyntaxItemsFromXml ("LodgeIt.xml");
		}
	
		public LodgeIt (string content, string syntax) : this ()
		{		
			Parameters[syntax_key] = syntax;
			Parameters[content_key] = content;
		}
		
		public LodgeIt (string content) : this ()
		{		
			Parameters[content_key] = content;
		}
		
		public override string GetPasteUrlFromResponse (HttpWebResponse response)
		{				
			string responseText;
			using (Stream responseStream = response.GetResponseStream ())
				using (StreamReader reader = new StreamReader (responseStream))
					responseText = reader.ReadToEnd ();
			
			Regex urlPattern = new Regex ("<a href=\"(.*?)\">"); 
			Match urlMatch = urlPattern.Match (responseText);
				
			string url = urlMatch.Groups[1].Value;
			if (url == string.Empty) {
				Log<LodgeIt>.Debug (responseText);
				throw new Exception (AddinManager.CurrentLocalizer.GetString ("Parsed url was empty. Lodge It has probably changed its format."));
			}
			
			return url_root + url;
		}
	}
}
