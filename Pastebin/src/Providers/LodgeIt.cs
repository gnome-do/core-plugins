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

using Mono.Unix;

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
			
			Parameters[content_key] = "";
			Parameters[syntax_key] = "text";
			 
			SupportedLanguages.Add (new TextSyntaxItem ("Apache Config (.htaccess)", "Apache Config (.htaccess)", "file", "apache"));
			SupportedLanguages.Add (new TextSyntaxItem ("Bash", "Bash", "file", "bash"));
			SupportedLanguages.Add (new TextSyntaxItem ("Batch (.bat)", "Batch (.bat)", "file", "bat"));
			SupportedLanguages.Add (new TextSyntaxItem ("C", "C", "file", "c"));
			SupportedLanguages.Add (new TextSyntaxItem ("C#", "C#", "file", "csharp"));
			SupportedLanguages.Add (new TextSyntaxItem ("C++", "C++", "file", "cpp"));
			SupportedLanguages.Add (new TextSyntaxItem ("CSS", "CSS", "file", "css"));
			SupportedLanguages.Add (new TextSyntaxItem ("D", "D", "file", "d"));
			SupportedLanguages.Add (new TextSyntaxItem ("Django / Jinja Templates", "Django / Jinja Templates", "file", "html+django"));
			SupportedLanguages.Add (new TextSyntaxItem ("Dylan", "Dylan", "file", "dylan"));
			SupportedLanguages.Add (new TextSyntaxItem ("Erlang", "Erlang", "file", "erlang"));
			SupportedLanguages.Add (new TextSyntaxItem ("GAS", "GAS", "file", "gas"));
			SupportedLanguages.Add (new TextSyntaxItem ("Genshi Templates", "Genshi Templates", "file", "html+genshi"));
			SupportedLanguages.Add (new TextSyntaxItem ("HTML", "HTML", "file", "html"));
			SupportedLanguages.Add (new TextSyntaxItem ("Haskell", "Haskell", "file", "haskell"));
			SupportedLanguages.Add (new TextSyntaxItem ("IRC Logs", "IRC Logs", "file", "irc"));
			SupportedLanguages.Add (new TextSyntaxItem ("Interactive Ruby", "Interactive Ruby", "file", "irb"));
			SupportedLanguages.Add (new TextSyntaxItem ("JSP", "JSP", "file", "jsp"));
			SupportedLanguages.Add (new TextSyntaxItem ("Java", "Java", "file", "java"));
			SupportedLanguages.Add (new TextSyntaxItem ("JavaScript", "JavaScript", "file", "js"));
			SupportedLanguages.Add (new TextSyntaxItem ("Lua", "Lua", "file", "lua"));
			SupportedLanguages.Add (new TextSyntaxItem ("Mako Templates", "Mako Templates", "file", "html+mako"));
			SupportedLanguages.Add (new TextSyntaxItem ("MiniD", "MiniD", "file", "minid"));
			SupportedLanguages.Add (new TextSyntaxItem ("Myghty Templates", "Myghty Templates", "file", "html+myghty"));
			SupportedLanguages.Add (new TextSyntaxItem ("OCaml", "OCaml", "file", "ocaml"));
			SupportedLanguages.Add (new TextSyntaxItem ("PHP", "PHP", "file", "html+php"));
			SupportedLanguages.Add (new TextSyntaxItem ("Perl", "Perl", "file", "perl"));
			SupportedLanguages.Add (new TextSyntaxItem ("Python", "Python", "file", "python"));
			SupportedLanguages.Add (new TextSyntaxItem ("Python Console Sessions", "Python Console Sessions", "file", "pycon"));
			SupportedLanguages.Add (new TextSyntaxItem ("Python Tracebacks", "Python Tracebacks", "file", "pytb"));
			SupportedLanguages.Add (new TextSyntaxItem ("Ruby", "Ruby", "file", "ruby"));
			SupportedLanguages.Add (new TextSyntaxItem ("SQL", "SQL", "file", "sql"));
			SupportedLanguages.Add (new TextSyntaxItem ("Scheme", "Scheme", "file", "scheme"));
			SupportedLanguages.Add (new TextSyntaxItem ("Smarty", "Smarty", "file", "smarty"));
			SupportedLanguages.Add (new TextSyntaxItem ("SquidConf", "SquidConf", "file", "squidconf"));
			SupportedLanguages.Add (new TextSyntaxItem ("TeX / LaTeX", "TeX / LaTeX", "file", "tex"));
			SupportedLanguages.Add (new TextSyntaxItem ("Text", "Text", "file", "text"));
			SupportedLanguages.Add (new TextSyntaxItem ("Unified Diff", "Unified Diff", "file", "diff"));
			SupportedLanguages.Add (new TextSyntaxItem ("Vim", "Vim", "file", "vim"));
			SupportedLanguages.Add (new TextSyntaxItem ("XML", "XML", "file", "xml"));
			SupportedLanguages.Add (new TextSyntaxItem ("eRuby / rhtml", "eRuby / rhtml", "file", "rhtml"));
			SupportedLanguages.Add (new TextSyntaxItem ("reStructuredText", "reStructuredText", "file", "rst"));
			SupportedLanguages.Add (new TextSyntaxItem ("sources.list", "sources.list", "file", "sourceslist"));
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
			using (Stream responseStream = response.GetResponseStream ()) {
				using (StreamReader reader = new StreamReader (responseStream)) {
					responseText = reader.ReadToEnd ();
				}
			}
			
			Regex urlPattern = new Regex ("<a href=\"(.*?)\">"); 
			Match urlMatch = urlPattern.Match (responseText);
				
			string url = urlMatch.Groups[1].Value;
			if (url == string.Empty) {
				Log<LodgeIt>.Debug (responseText);
				throw new Exception (Catalog.GetString ("Parsed url was empty. Lodge It has probably changed its format."));
			}
			
			return url_root + url;
		}
	}
}
