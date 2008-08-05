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
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Pastebin
{
   public class PastebinCA : IPastebinProvider
   {
       private const string urlRoot = "http://pastebin.ca";

       private NameValueCollection parameters;
       private List<TextSyntaxItem> supportedLanguages;

       public PastebinCA ()
       {
           parameters = new NameValueCollection();
           parameters["api"] = "4xPQUdtxHQ9wxlAJ9t/ztpv36MM/ZE9G";
           parameters["type"] = "1";
           parameters["description"] = "";
           parameters["content"] = "";
           parameters["name"] = "";
           parameters["expiry"] = "1 month";

           supportedLanguages = new List<TextSyntaxItem>();
           supportedLanguages.Add (new TextSyntaxItem ("Plain Text", "Plain Text", "file", "1"));
           supportedLanguages.Add (new TextSyntaxItem ("Actionscript", "Actionscript", "file", "18"));
           supportedLanguages.Add (new TextSyntaxItem ("Ada", "Ada", "file", "19"));
           supportedLanguages.Add (new TextSyntaxItem ("Apache Config", "Apache Config", "file", "20"));
           supportedLanguages.Add (new TextSyntaxItem ("ASP", "ASP", "file", "22"));
           supportedLanguages.Add (new TextSyntaxItem ("Bash", "Bash", "file", "23"));
           supportedLanguages.Add (new TextSyntaxItem ("C", "C", "file", "3"));
           supportedLanguages.Add (new TextSyntaxItem ("C++", "C++", "file", "4"));
           supportedLanguages.Add (new TextSyntaxItem ("C#", "C#", "file", "9"));
           supportedLanguages.Add (new TextSyntaxItem ("CSS", "CSS", "file", "24"));
           supportedLanguages.Add (new TextSyntaxItem ("Delphi", "Delphi", "file", "25"));
           supportedLanguages.Add (new TextSyntaxItem ("Diff / Patch", "Diff / Patch", "file", "34"));
           supportedLanguages.Add (new TextSyntaxItem ("HTML 4 Strict", "HTML 4 Strict", "file", "26"));
           supportedLanguages.Add (new TextSyntaxItem ("Java", "Java", "file", "7"));
           supportedLanguages.Add (new TextSyntaxItem ("Javascript", "Javascript", "file", "27"));
           supportedLanguages.Add (new TextSyntaxItem ("LISP", "LISP", "file", "28"));
           supportedLanguages.Add (new TextSyntaxItem ("Lua", "Lua", "file", "29"));
           supportedLanguages.Add (new TextSyntaxItem ("Microprocessor ASM", "Microprocessor ASM", "file", "30"));
           supportedLanguages.Add (new TextSyntaxItem ("mIRC Script", "mIRC SCript", "file", "13"));
           supportedLanguages.Add (new TextSyntaxItem ("Objective C", "Objective C", "file", "31"));
           supportedLanguages.Add (new TextSyntaxItem ("Perl", "Perl", "file", "6"));
           supportedLanguages.Add (new TextSyntaxItem ("PHP", "PHP", "file", "5"));
           supportedLanguages.Add (new TextSyntaxItem ("PL/I", "PL/I", "file", "14"));
           supportedLanguages.Add (new TextSyntaxItem ("Python", "Python", "file", "11"));
           supportedLanguages.Add (new TextSyntaxItem ("Ruby", "Ruby", "file", "10"));
           supportedLanguages.Add (new TextSyntaxItem ("SQL", "SQL", "file", "16"));
           supportedLanguages.Add (new TextSyntaxItem ("Scheme", "Scheme", "file", "17"));
           supportedLanguages.Add (new TextSyntaxItem ("Script Log", "Script Log", "file", "33"));
           supportedLanguages.Add (new TextSyntaxItem ("Visual BASIC", "Visual BASIC", "file", "8"));
           supportedLanguages.Add (new TextSyntaxItem ("VB.NET", "VB.NET", "file", "32"));
           supportedLanguages.Add (new TextSyntaxItem ("XML", "XML", "file", "15"));
       }

       public PastebinCA(string content, string syntax)
           : this()
       {
           parameters["type"] = syntax;
           parameters["content"] = content;
       }

       public PastebinCA(string content)
           : this()
       {
           parameters["content"] = content;
       }

       public bool ShouldAllowAutoRedirect
       {
           get { return true; }
       }

       public string BaseUrl
       {
           get { return urlRoot + "/quiet-paste.php"; }
       }

       public NameValueCollection Parameters
       {
           get { return parameters; }
       }

       public string GetPasteUrlFromResponse(HttpWebResponse response)
       {
           string responseText;
           using (Stream responseStream = response.GetResponseStream ()) {
               using (StreamReader reader = new StreamReader (responseStream)) {
                   responseText = reader.ReadToEnd ();
               }
           }

           string url = String.Empty;
           if (responseText.Contains ("SUCCESS")) {
               url = urlRoot + "/" + responseText.Split (new string[]{":"}, StringSplitOptions.RemoveEmptyEntries)[1];
           }

           return url;
       }

       public List<TextSyntaxItem> SupportedLanguages
       {
           get { return supportedLanguages; }
       }
   }
}