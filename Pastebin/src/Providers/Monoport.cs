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
			
			SupportedLanguages = BuildSyntaxList ();
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

		List<TextSyntaxItem> BuildSyntaxList ()
		{
			List<TextSyntaxItem> syntaxes;

			syntaxes = new List<TextSyntaxItem> ();
			syntaxes.Add (new TextSyntaxItem ("ASM", "ASM (NASM based)", "text-x-generic", "ASM"));
			syntaxes.Add (new TextSyntaxItem ("ASP", "ASP", "text-x-generic", "asp"));
			syntaxes.Add (new TextSyntaxItem ("ActionScript", "ActionScript", "text-x-generic", "actionscript"));
			syntaxes.Add (new TextSyntaxItem ("Ada", "Ada", "text-x-generic", "ada"));
			syntaxes.Add (new TextSyntaxItem ("Apache Log", "Apache Log", "text-x-generic", "apache"));
			syntaxes.Add (new TextSyntaxItem ("AppleScript", "AppleScript", "text-x-generic", "applescript"));
			syntaxes.Add (new TextSyntaxItem ("Bash", "Bash", "text-x-generic", "bash"));
			syntaxes.Add (new TextSyntaxItem ("C for Macs", "C for Macs", "text-x-generic", "c_mac"));
			syntaxes.Add (new TextSyntaxItem ("C", "C", "text-x-generic", "c"));
			syntaxes.Add (new TextSyntaxItem ("C", "C", "text-x-generic", "c"));
			syntaxes.Add (new TextSyntaxItem ("C#", "C#", "text-x-generic", "csharp"));
			syntaxes.Add (new TextSyntaxItem ("C++", "C++", "text-x-generic", "cpp"));
			syntaxes.Add (new TextSyntaxItem ("CAD DCL", "CAD DCL", "text-x-generic", "caddcl"));
			syntaxes.Add (new TextSyntaxItem ("CAD Lisp", "CAD Lisp", "text-x-generic", "cadlisp"));
			syntaxes.Add (new TextSyntaxItem ("CSS", "CSS", "text-x-generic", "css"));
			syntaxes.Add (new TextSyntaxItem ("ColdFusion", "ColdFusion", "text-x-generic", "cfm"));
			syntaxes.Add (new TextSyntaxItem ("D", "D", "text-x-generic", "d"));
			syntaxes.Add (new TextSyntaxItem ("DOS", "DOS", "text-x-generic", "dos"));
			syntaxes.Add (new TextSyntaxItem ("Delphi", "Delphia", "text-x-generic", "delphi"));
			syntaxes.Add (new TextSyntaxItem ("Diff", "Diff", "text-x-generic", "diff"));
			syntaxes.Add (new TextSyntaxItem ("Eiffel", "Eiffel", "text-x-generic", "eiffel"));
			syntaxes.Add (new TextSyntaxItem ("Fortran", "Fortrain", "text-x-generic", "fortran"));
			syntaxes.Add (new TextSyntaxItem ("FreeBasic", "FreeBasic", "text-x-generic", "freebasic"));
			syntaxes.Add (new TextSyntaxItem ("Game Maker", "Game Maker", "text-x-generic", "gml"));
			syntaxes.Add (new TextSyntaxItem ("HTML 4", "HTML 4 Strict", "text-x-generic", "html4strict"));
			syntaxes.Add (new TextSyntaxItem ("Java", "Java", "text-x-generic", "java"));
			syntaxes.Add (new TextSyntaxItem ("Javascript", "JavaScript", "text-x-generic", "javascript"));
			syntaxes.Add (new TextSyntaxItem ("Lua", "Lua", "text-x-generic", "lua"));
			syntaxes.Add (new TextSyntaxItem ("MPASM", "MPASM", "text-x-generic", "mpasm"));
			syntaxes.Add (new TextSyntaxItem ("Matlab", "Matlab", "text-x-generic", "matlab"));
			syntaxes.Add (new TextSyntaxItem ("MySQL", "MySQL", "text-x-generic", "mysql"));
			syntaxes.Add (new TextSyntaxItem ("NullSoft Installer", "NullSoft Installer", "text-x-generic", "nsis"));
			syntaxes.Add (new TextSyntaxItem ("OCaml", "OCaml", "text-x-generic", "ocaml"));
			syntaxes.Add (new TextSyntaxItem ("Objective-C", "Objective-C", "text-x-generic", "cobjc"));
			syntaxes.Add (new TextSyntaxItem ("OpenOffice.org BASIC", "OpenOffice.org BASIC", "text-x-generic", "oobas"));
			syntaxes.Add (new TextSyntaxItem ("Oracle 8", "Oracle 8", "text-x-generic", "oracle8"));
			syntaxes.Add (new TextSyntaxItem ("PHP", "PHP", "text-x-generic", "php"));
			syntaxes.Add (new TextSyntaxItem ("Pascal", "Pascal", "text-x-generic", "pascal"));
			syntaxes.Add (new TextSyntaxItem ("Perl", "Perl", "text-x-generic", "perl"));
			syntaxes.Add (new TextSyntaxItem ("Plain Text", "Plain Text", "text-x-generic", "text"));
			syntaxes.Add (new TextSyntaxItem ("Python", "Python", "text-x-generic", "python"));
			syntaxes.Add (new TextSyntaxItem ("QuickBASIC", "QuickBASIC", "text-x-generic", "qbasic"));
			syntaxes.Add (new TextSyntaxItem ("Robots", "Robots", "text-x-generic", "robots"));
			syntaxes.Add (new TextSyntaxItem ("Ruby", "Ruby", "text-x-generic", "ruby"));
			syntaxes.Add (new TextSyntaxItem ("SQL", "SQL", "text-x-generic", "sql"));
			syntaxes.Add (new TextSyntaxItem ("Scheme", "Scheme", "text-x-generic", "scheme"));
			syntaxes.Add (new TextSyntaxItem ("Smarrty", "Smarty", "text-x-generic", "smarty"));
			syntaxes.Add (new TextSyntaxItem ("Tcl", "Tcl", "text-x-generic", "tcl"));
			syntaxes.Add (new TextSyntaxItem ("VB.NET", "VB.NET", "text-x-generic", "vbnet"));
			syntaxes.Add (new TextSyntaxItem ("VisualBasic", "VisualBasic", "text-x-generic", "vb"));
			syntaxes.Add (new TextSyntaxItem ("VisualFoxPro", "VisualFoxPro", "text-x-generic", "visualfoxpro"));
			syntaxes.Add (new TextSyntaxItem ("XML", "XML", "text-x-generic", "xml"));

			return syntaxes;
		}
	}
}
