//  OpenSearchItem.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Text;
using Do.Universe;

namespace OpenSearch
{
	public class OpenSearchItem	: IOpenSearchItem
	{
		private string name, description, urlTemplate;
		
		public OpenSearchItem (string name, string description, string urlTemplate)
		{
			this.name = name;
			this.description = description;
			this.urlTemplate = urlTemplate;
		}
		
		public string Name 
		{ 
			get { return name; } 
		}
		
		public string Description 
		{ 
			get { return description; } 
		}
		
		public string Icon
		{
			get { return "www"; }
		}
		
		public string UrlTemplate
		{
			get { return urlTemplate; }
		}
		
		/// <summary>
		/// Build a search URL for this OpenSearch item using the provided search terms.
		/// </summary>
		/// <param name="searchTerms">
		/// The search terms to use in the URL template.
		/// </param>
		/// <returns>
		/// A formatted search URL, using the provided search terms.
		/// </returns>
		public string BuildSearchUrl (string searchTerm)
		{
			return UrlTemplate.Replace ("{searchTerms}",  EncodeUrl (searchTerm));
		}
		
		private static string EncodeUrl (string input)
		{
			if (input == null) 
				return null;
			if (input.Length == 0)
				return string.Empty;
			
			StringBuilder sb = new StringBuilder ();
			
			foreach(char ch in input) {
				if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || 
				    (((ch > '/') && (ch < ':')) || (((ch == '.') || (ch == '-')) || (ch == '_')))) {
					sb.Append (ch);
				}
				else if (ch > '\x007f') {
					sb.Append ("%u" + TwoByteHex (ch));
				}
				else {
					sb.Append ("%" + SingleByteHex (ch));
				}
			}
	
			return sb.ToString ();	
		}
		
		private static string SingleByteHex (char c)
		{
			uint num = c;
			return num.ToString("x").PadLeft(2,'0');
		}
		
		private static string TwoByteHex (char c)
		{
			uint num = c;
			return num.ToString("x").PadLeft(4,'0');
		}
	}
}
