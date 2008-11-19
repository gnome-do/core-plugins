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

namespace Pastebin
{
	public class LodgeIt : IPastebinProvider
	{		
		private const string urlRoot = "http://paste.pocoo.org";
	
		private NameValueCollection parameters;
		private List<TextSyntaxItem> supportedLanguages;
		
		public LodgeIt ()
		{
			parameters = new NameValueCollection();
			parameters["language"] = "text";
			parameters["code"] = "";
			
			supportedLanguages = new List<TextSyntaxItem> (); 
			supportedLanguages.Add (new TextSyntaxItem ("Apache Config (.htaccess)", "Apache Config (.htaccess)", "file", "apache"));
			supportedLanguages.Add (new TextSyntaxItem ("Bash", "Bash", "file", "bash"));
			supportedLanguages.Add (new TextSyntaxItem ("Batch (.bat)", "Batch (.bat)", "file", "bat"));
			supportedLanguages.Add (new TextSyntaxItem ("C", "C", "file", "c"));
			supportedLanguages.Add (new TextSyntaxItem ("C#", "C#", "file", "csharp"));
			supportedLanguages.Add (new TextSyntaxItem ("C++", "C++", "file", "cpp"));
			supportedLanguages.Add (new TextSyntaxItem ("CSS", "CSS", "file", "css"));
			supportedLanguages.Add (new TextSyntaxItem ("D", "D", "file", "d"));
			supportedLanguages.Add (new TextSyntaxItem ("Django / Jinja Templates", "Django / Jinja Templates", "file", "html+django"));
			supportedLanguages.Add (new TextSyntaxItem ("Dylan", "Dylan", "file", "dylan"));
			supportedLanguages.Add (new TextSyntaxItem ("Erlang", "Erlang", "file", "erlang"));
			supportedLanguages.Add (new TextSyntaxItem ("GAS", "GAS", "file", "gas"));
			supportedLanguages.Add (new TextSyntaxItem ("Genshi Templates", "Genshi Templates", "file", "html+genshi"));
			supportedLanguages.Add (new TextSyntaxItem ("HTML", "HTML", "file", "html"));
			supportedLanguages.Add (new TextSyntaxItem ("Haskell", "Haskell", "file", "haskell"));
			supportedLanguages.Add (new TextSyntaxItem ("IRC Logs", "IRC Logs", "file", "irc"));
			supportedLanguages.Add (new TextSyntaxItem ("Interactive Ruby", "Interactive Ruby", "file", "irb"));
			supportedLanguages.Add (new TextSyntaxItem ("JSP", "JSP", "file", "jsp"));
			supportedLanguages.Add (new TextSyntaxItem ("Java", "Java", "file", "java"));
			supportedLanguages.Add (new TextSyntaxItem ("JavaScript", "JavaScript", "file", "js"));
			supportedLanguages.Add (new TextSyntaxItem ("Lua", "Lua", "file", "lua"));
			supportedLanguages.Add (new TextSyntaxItem ("Mako Templates", "Mako Templates", "file", "html+mako"));
			supportedLanguages.Add (new TextSyntaxItem ("MiniD", "MiniD", "file", "minid"));
			supportedLanguages.Add (new TextSyntaxItem ("Myghty Templates", "Myghty Templates", "file", "html+myghty"));
			supportedLanguages.Add (new TextSyntaxItem ("OCaml", "OCaml", "file", "ocaml"));
			supportedLanguages.Add (new TextSyntaxItem ("PHP", "PHP", "file", "html+php"));
			supportedLanguages.Add (new TextSyntaxItem ("Perl", "Perl", "file", "perl"));
			supportedLanguages.Add (new TextSyntaxItem ("Python", "Python", "file", "python"));
			supportedLanguages.Add (new TextSyntaxItem ("Python Console Sessions", "Python Console Sessions", "file", "pycon"));
			supportedLanguages.Add (new TextSyntaxItem ("Python Tracebacks", "Python Tracebacks", "file", "pytb"));
			supportedLanguages.Add (new TextSyntaxItem ("Ruby", "Ruby", "file", "ruby"));
			supportedLanguages.Add (new TextSyntaxItem ("SQL", "SQL", "file", "sql"));
			supportedLanguages.Add (new TextSyntaxItem ("Scheme", "Scheme", "file", "scheme"));
			supportedLanguages.Add (new TextSyntaxItem ("Smarty", "Smarty", "file", "smarty"));
			supportedLanguages.Add (new TextSyntaxItem ("SquidConf", "SquidConf", "file", "squidconf"));
			supportedLanguages.Add (new TextSyntaxItem ("TeX / LaTeX", "TeX / LaTeX", "file", "tex"));
			supportedLanguages.Add (new TextSyntaxItem ("Text", "Text", "file", "text"));
			supportedLanguages.Add (new TextSyntaxItem ("Unified Diff", "Unified Diff", "file", "diff"));
			supportedLanguages.Add (new TextSyntaxItem ("Vim", "Vim", "file", "vim"));
			supportedLanguages.Add (new TextSyntaxItem ("XML", "XML", "file", "xml"));
			supportedLanguages.Add (new TextSyntaxItem ("eRuby / rhtml", "eRuby / rhtml", "file", "rhtml"));
			supportedLanguages.Add (new TextSyntaxItem ("reStructuredText", "reStructuredText", "file", "rst"));
			supportedLanguages.Add (new TextSyntaxItem ("sources.list", "sources.list", "file", "sourceslist"));
		}
	
		public LodgeIt (string content, string syntax) : this ()
		{		
			parameters["language"] = syntax;
			parameters["code"] = content;
		}
		
		public LodgeIt (string content) : this ()
		{		
			parameters["code"] = content;
		}

		public bool ShouldAllowAutoRedirect
		{
			get { return false; }
		}
		
		public string BaseUrl 
		{ 
			get { return urlRoot; } 
		}
		
		public NameValueCollection Parameters 
		{ 
			get { return parameters; } 
		}
		
		public string GetPasteUrlFromResponse (HttpWebResponse response)
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
				throw new Exception (Catalog.GetString ("Parsed url was empty. Lodge It has probably changed its format."));
			}
			
			return urlRoot + url;
		}
		
		public List<TextSyntaxItem> SupportedLanguages 
		{ 
			get { return supportedLanguages; }
		}
	}
}
