//  Paste2.cs
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
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Pastebin
{
	public class Paste2 : IPastebinProvider
	{		
		private const string urlRoot = "http://paste2.org";
	
		private NameValueCollection parameters;
		private List<TextSyntaxItem> supportedLanguages;
		
		public Paste2 ()
		{
			parameters = new NameValueCollection();
			parameters["lang"] = "text";
			parameters["description"] = "";
			parameters["code"] = "";
			parameters["parent"] = "0";		
			
			supportedLanguages = new List<TextSyntaxItem> (); 
			supportedLanguages.Add (new TextSyntaxItem ("Plain Text", "Plain Text", "file", "text"));
			supportedLanguages.Add (new TextSyntaxItem ("Actionscript", "Actionscript", "file", "actionscript"));
			supportedLanguages.Add (new TextSyntaxItem ("Ada", "Ada", "file", "ada"));
			supportedLanguages.Add (new TextSyntaxItem ("Apache Config", "Apache Config", "file", "apache"));
			supportedLanguages.Add (new TextSyntaxItem ("AppleScript", "AppleScript", "file", "applescript"));
			supportedLanguages.Add (new TextSyntaxItem ("ASP", "ASP", "file", "asp"));
			supportedLanguages.Add (new TextSyntaxItem ("Bash", "Bash", "file", "bash"));			
			supportedLanguages.Add (new TextSyntaxItem ("C", "C", "file", "c"));
			supportedLanguages.Add (new TextSyntaxItem ("Cold Fusion", "Cold Fusion", "file", "cfm"));
			supportedLanguages.Add (new TextSyntaxItem ("C++", "C++", "file", "cpp"));
			supportedLanguages.Add (new TextSyntaxItem ("C#", "C#", "file", "csharp"));
			supportedLanguages.Add (new TextSyntaxItem ("CSS", "CSS", "file", "css"));
			supportedLanguages.Add (new TextSyntaxItem ("D", "D", "file", "d"));
			supportedLanguages.Add (new TextSyntaxItem ("Delphi", "Delphi", "file", "delphi"));
			supportedLanguages.Add (new TextSyntaxItem ("UNIX Diff", "UNIX Diff", "file", "diff"));
			supportedLanguages.Add (new TextSyntaxItem ("Eiffel", "Eiffel", "file", "eiffel"));
			supportedLanguages.Add (new TextSyntaxItem ("Fortran", "Fortran", "file", "fortran"));
			supportedLanguages.Add (new TextSyntaxItem ("HTML 4 Strict", "HTML 4 Strict", "file", "html4strict"));
			supportedLanguages.Add (new TextSyntaxItem ("Ini", "Ini", "file", "ini"));
			supportedLanguages.Add (new TextSyntaxItem ("Java", "Java", "file", "java"));
			supportedLanguages.Add (new TextSyntaxItem ("Java5", "Java5", "file", "java5"));
			supportedLanguages.Add (new TextSyntaxItem ("Javascript", "Javascript", "file", "javascript"));
			supportedLanguages.Add (new TextSyntaxItem ("LaTeX", "LaTeX", "file", "latex"));
			supportedLanguages.Add (new TextSyntaxItem ("LISP", "LISP", "file", "lisp"));
			supportedLanguages.Add (new TextSyntaxItem ("Lua", "Lua", "file", "lua"));
			supportedLanguages.Add (new TextSyntaxItem ("MATLAB", "MATLAB", "file", "matlab"));
			supportedLanguages.Add (new TextSyntaxItem ("Perl", "Perl", "file", "perl"));
			supportedLanguages.Add (new TextSyntaxItem ("PHP", "PHP", "file", "php"));
			supportedLanguages.Add (new TextSyntaxItem ("Python", "Python", "file", "python"));
			supportedLanguages.Add (new TextSyntaxItem ("QBasic / QuickBASIC", "QBasic / QuickBASIC", "file", "qbasic"));
			supportedLanguages.Add (new TextSyntaxItem ("Robots", "Robots", "file", "robots"));
			supportedLanguages.Add (new TextSyntaxItem ("Ruby", "Ruby", "file", "ruby"));	
			supportedLanguages.Add (new TextSyntaxItem ("SQL", "SQL", "file", "sql"));		
			supportedLanguages.Add (new TextSyntaxItem ("TCL", "TCL", "file", "tcl"));
			supportedLanguages.Add (new TextSyntaxItem ("Visual BASIC", "Visual BASIC", "file", "vb"));
			supportedLanguages.Add (new TextSyntaxItem ("VB.NET", "VB.NET", "file", "vbnet"));
			supportedLanguages.Add (new TextSyntaxItem ("Winbatch", "Winbatch", "file", "winbatch"));		
			supportedLanguages.Add (new TextSyntaxItem ("XML", "XML", "file", "xml"));
		}
	
		public Paste2 (string content, string syntax) : this ()
		{		
			parameters["lang"] = syntax;
			parameters["code"] = content;
		}
		
		public Paste2 (string content) : this ()
		{		
			parameters["code"] = content;
		}

		public bool ShouldAllowAutoRedirect
		{
			get { return false; }
		}
		
		public string Name
		{
			get { return "paste2.org"; }
		}

		public string UserAgent
		{
			get { return ""; }
		}
		
		public string BaseUrl 
		{ 
			get { return urlRoot + "/new-paste"; } 
		}
		
		public NameValueCollection Parameters 
		{ 
			get { return parameters; } 
		}
		
		public string GetPasteUrlFromResponse (HttpWebResponse response)
		{
			return urlRoot + response.Headers["Location"];
		}
		
		public List<TextSyntaxItem> SupportedLanguages 
		{ 
			get { return supportedLanguages; }
		}
	}
}
