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
			
			SupportedLanguages.Add (new TextSyntaxItem ("Plain Text", "Plain Text", "file", "1"));
			SupportedLanguages.Add (new TextSyntaxItem ("Actionscript", "Actionscript", "file", "18"));
			SupportedLanguages.Add (new TextSyntaxItem ("Ada", "Ada", "file", "19"));
			SupportedLanguages.Add (new TextSyntaxItem ("Apache Config", "Apache Config", "file", "20"));
			SupportedLanguages.Add (new TextSyntaxItem ("ASP", "ASP", "file", "22"));
			SupportedLanguages.Add (new TextSyntaxItem ("Bash", "Bash", "file", "23"));
			SupportedLanguages.Add (new TextSyntaxItem ("C", "C", "file", "3"));
			SupportedLanguages.Add (new TextSyntaxItem ("C++", "C++", "file", "4"));
			SupportedLanguages.Add (new TextSyntaxItem ("C#", "C#", "file", "9"));
			SupportedLanguages.Add (new TextSyntaxItem ("CSS", "CSS", "file", "24"));
			SupportedLanguages.Add (new TextSyntaxItem ("Delphi", "Delphi", "file", "25"));
			SupportedLanguages.Add (new TextSyntaxItem ("Diff / Patch", "Diff / Patch", "file", "34"));
			SupportedLanguages.Add (new TextSyntaxItem ("HTML 4 Strict", "HTML 4 Strict", "file", "26"));
			SupportedLanguages.Add (new TextSyntaxItem ("Java", "Java", "file", "7"));
			SupportedLanguages.Add (new TextSyntaxItem ("Javascript", "Javascript", "file", "27"));
			SupportedLanguages.Add (new TextSyntaxItem ("LISP", "LISP", "file", "28"));
			SupportedLanguages.Add (new TextSyntaxItem ("Lua", "Lua", "file", "29"));
			SupportedLanguages.Add (new TextSyntaxItem ("Microprocessor ASM", "Microprocessor ASM", "file", "30"));
			SupportedLanguages.Add (new TextSyntaxItem ("mIRC Script", "mIRC SCript", "file", "13"));
			SupportedLanguages.Add (new TextSyntaxItem ("Objective C", "Objective C", "file", "31"));
			SupportedLanguages.Add (new TextSyntaxItem ("Perl", "Perl", "file", "6"));
			SupportedLanguages.Add (new TextSyntaxItem ("PHP", "PHP", "file", "5"));
			SupportedLanguages.Add (new TextSyntaxItem ("PL/I", "PL/I", "file", "14"));
			SupportedLanguages.Add (new TextSyntaxItem ("Python", "Python", "file", "11"));
			SupportedLanguages.Add (new TextSyntaxItem ("Ruby", "Ruby", "file", "10"));
			SupportedLanguages.Add (new TextSyntaxItem ("SQL", "SQL", "file", "16"));
			SupportedLanguages.Add (new TextSyntaxItem ("Scheme", "Scheme", "file", "17"));
			SupportedLanguages.Add (new TextSyntaxItem ("Script Log", "Script Log", "file", "33"));
			SupportedLanguages.Add (new TextSyntaxItem ("Visual BASIC", "Visual BASIC", "file", "8"));
			SupportedLanguages.Add (new TextSyntaxItem ("VB.NET", "VB.NET", "file", "32"));
			SupportedLanguages.Add (new TextSyntaxItem ("XML", "XML", "file", "15"));
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