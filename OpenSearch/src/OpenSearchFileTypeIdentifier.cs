//  OpenSearchFileTypeIdentifier.cs
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

using System.Xml;

namespace Do.Plugins.OpenSearch
{
	public static class OpenSearchFileTypeIdentifier
	{
		public static OpenSearchFileType Identify (string file)
		{
			XmlDocument doc = new XmlDocument ();
			doc.Load (file);				
			
			XmlNamespaceManager namespaceManager = XmlNamespaceHelper.PopulateNamespaceManager (doc);
						
			bool isMozillaSearch = (namespaceManager.LookupPrefix ("http://www.mozilla.org/2006/browser/search/") != null);
			bool isOpenSearch = (namespaceManager.LookupPrefix ("http://a9.com/-/spec/opensearch/1.1/") != null);
							
			if (isOpenSearch)
				return OpenSearchFileType.OpenSearch;
						
			if (isMozillaSearch && !isOpenSearch)
				return OpenSearchFileType.MozillaSearch;
			
			return OpenSearchFileType.Unknown;
		}	
	}
}
