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
	public class Paste2 : AbstractPastebinProvider
	{		
		const string url_root = "http://paste2.org";
		const string content_key = "code";
		const string syntax_key = "lang";
		
		public Paste2 ()
		{
			Name = "paste2.org";
			BaseUrl = url_root + "/new-paste";
							
			ShouldAllowAutoRedirect = false;
			
			Parameters[syntax_key] = "text";
			Parameters["description"] = "";
			Parameters[content_key] = "";
			Parameters["parent"] = "0";		
			
			SupportedLanguages.Add (new TextSyntaxItem ("Plain Text", "Plain Text", "file", "text"));
			SupportedLanguages.Add (new TextSyntaxItem ("Actionscript", "Actionscript", "file", "actionscript"));
			SupportedLanguages.Add (new TextSyntaxItem ("Ada", "Ada", "file", "ada"));
			SupportedLanguages.Add (new TextSyntaxItem ("Apache Config", "Apache Config", "file", "apache"));
			SupportedLanguages.Add (new TextSyntaxItem ("AppleScript", "AppleScript", "file", "applescript"));
			SupportedLanguages.Add (new TextSyntaxItem ("ASP", "ASP", "file", "asp"));
			SupportedLanguages.Add (new TextSyntaxItem ("Bash", "Bash", "file", "bash"));			
			SupportedLanguages.Add (new TextSyntaxItem ("C", "C", "file", "c"));
			SupportedLanguages.Add (new TextSyntaxItem ("Cold Fusion", "Cold Fusion", "file", "cfm"));
			SupportedLanguages.Add (new TextSyntaxItem ("C++", "C++", "file", "cpp"));
			SupportedLanguages.Add (new TextSyntaxItem ("C#", "C#", "file", "csharp"));
			SupportedLanguages.Add (new TextSyntaxItem ("CSS", "CSS", "file", "css"));
			SupportedLanguages.Add (new TextSyntaxItem ("D", "D", "file", "d"));
			SupportedLanguages.Add (new TextSyntaxItem ("Delphi", "Delphi", "file", "delphi"));
			SupportedLanguages.Add (new TextSyntaxItem ("UNIX Diff", "UNIX Diff", "file", "diff"));
			SupportedLanguages.Add (new TextSyntaxItem ("Eiffel", "Eiffel", "file", "eiffel"));
			SupportedLanguages.Add (new TextSyntaxItem ("Fortran", "Fortran", "file", "fortran"));
			SupportedLanguages.Add (new TextSyntaxItem ("HTML 4 Strict", "HTML 4 Strict", "file", "html4strict"));
			SupportedLanguages.Add (new TextSyntaxItem ("Ini", "Ini", "file", "ini"));
			SupportedLanguages.Add (new TextSyntaxItem ("Java", "Java", "file", "java"));
			SupportedLanguages.Add (new TextSyntaxItem ("Java5", "Java5", "file", "java5"));
			SupportedLanguages.Add (new TextSyntaxItem ("Javascript", "Javascript", "file", "javascript"));
			SupportedLanguages.Add (new TextSyntaxItem ("LaTeX", "LaTeX", "file", "latex"));
			SupportedLanguages.Add (new TextSyntaxItem ("LISP", "LISP", "file", "lisp"));
			SupportedLanguages.Add (new TextSyntaxItem ("Lua", "Lua", "file", "lua"));
			SupportedLanguages.Add (new TextSyntaxItem ("MATLAB", "MATLAB", "file", "matlab"));
			SupportedLanguages.Add (new TextSyntaxItem ("Perl", "Perl", "file", "perl"));
			SupportedLanguages.Add (new TextSyntaxItem ("PHP", "PHP", "file", "php"));
			SupportedLanguages.Add (new TextSyntaxItem ("Python", "Python", "file", "python"));
			SupportedLanguages.Add (new TextSyntaxItem ("QBasic / QuickBASIC", "QBasic / QuickBASIC", "file", "qbasic"));
			SupportedLanguages.Add (new TextSyntaxItem ("Robots", "Robots", "file", "robots"));
			SupportedLanguages.Add (new TextSyntaxItem ("Ruby", "Ruby", "file", "ruby"));	
			SupportedLanguages.Add (new TextSyntaxItem ("SQL", "SQL", "file", "sql"));		
			SupportedLanguages.Add (new TextSyntaxItem ("TCL", "TCL", "file", "tcl"));
			SupportedLanguages.Add (new TextSyntaxItem ("Visual BASIC", "Visual BASIC", "file", "vb"));
			SupportedLanguages.Add (new TextSyntaxItem ("VB.NET", "VB.NET", "file", "vbnet"));
			SupportedLanguages.Add (new TextSyntaxItem ("Winbatch", "Winbatch", "file", "winbatch"));		
			SupportedLanguages.Add (new TextSyntaxItem ("XML", "XML", "file", "xml"));
		}
	
		public Paste2 (string content, string syntax) : this ()
		{		
			Parameters[syntax_key] = syntax;
			Parameters[content_key] = content;
		}
		
		public Paste2 (string content) : this ()
		{		
			Parameters[content_key] = content;
		}
				
		public override string GetPasteUrlFromResponse (HttpWebResponse response)
		{
			return url_root + response.Headers["Location"];
		}
	}
}
