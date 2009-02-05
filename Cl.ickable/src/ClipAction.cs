// ClipAction.cs
// 
// Copyright (C) 2008 Idealab
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Cl.ickable
{
	
	public class ClipAction : Act
	{
		const int MaxTitleLength = 80;
		const string ClipPostURL = "http://cl.ickable.com/cgi-bin/SaveClip.cgi";
		
	    public override string Name {
			get { return Catalog.GetString ("Clip"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Create a clip with Cl.ickable"); }
		}
		
		public override string Icon {
			get { return "edit-cut"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		string AsParameterString (IDictionary<string, string> parameters)
		{
			string s = "";
			
			foreach (KeyValuePair<string, string> kv in parameters) {
				s += s.Length == 0 ? "?" : "&";
				s += kv.Key + "=" + HttpUtility.UrlEncode (kv.Value);
			}
			return s;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string text, title, url;
			Dictionary<string, string> parameters;
			
			text = (items.First () as ITextItem).Text;
			title = modItems.Any () ?
				(modItems.First () as ITextItem).Text :
				text.Substring (0, Math.Min (text.Length, MaxTitleLength)) + "...";
			
			parameters = new Dictionary<string, string> ();
			// "title" is the "human readable" title of the containing document,
			// available in FF JS as document.title
			parameters ["title"] = title;
			// "content" is HTML content (it can be just text, of course).
			parameters ["content"] = text;
			// "location" is an absolute URL to the containing page ... available in
			// Firefox JavaScript as document.location
			parameters ["location"] = "";
			// "base" is the "base URL" of the page ... available in Firefox
			// JavaScript as document.baseURI
			parameters ["base"] = "";
			
			url = ClipPostURL + AsParameterString (parameters);
			Services.Environment.OpenUrl (url);
			yield break;
		}
	}
}
