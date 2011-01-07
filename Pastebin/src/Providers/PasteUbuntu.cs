//  PasteUbuntu.cs
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
	public class PasteUbuntu : AbstractPastebinProvider
	{		
		const string url_root = "http://paste.ubuntu.com";
		const string content_key = "content";
		const string syntax_key = "syntax";
		
		public PasteUbuntu ()
		{
			Name = "paste.ubuntu.com";
			BaseUrl = url_root;
							
			ShouldAllowAutoRedirect = false;
			Expect100Continue = false;
			
			Parameters[syntax_key] = "text";
			Parameters[content_key] = "";
			Parameters["poster"] = "Do";		
			
			SupportedLanguages = PopulateTextSyntaxItemsFromXml ("PasteUbuntu.xml");
		}
	
		public PasteUbuntu (string content, string syntax) : this ()
		{		
			Parameters[syntax_key] = syntax;
			Parameters[content_key] = content;
		}
		
		public PasteUbuntu (string content) : this ()
		{		
			Parameters[content_key] = content;
		}
				
		public override string GetPasteUrlFromResponse (HttpWebResponse response)
		{
			return response.Headers["Location"];
		}
	}
}
