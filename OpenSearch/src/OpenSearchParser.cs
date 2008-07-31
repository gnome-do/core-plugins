//  OpenSearchParser.cs
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

using System.Collections.Generic;
using Do.Universe;

using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace OpenSearch
{
	public class OpenSearchParser
	{	
		/// <summary>
		/// Creates an OpenSearchItem from the specified file.
		/// </summary>
		/// <param name="file">
		/// The file to process.
		/// </param>
		/// <returns>
		/// A populated OpenSearchItem, or null if it was unable to create an
		/// OpenSearchItem from the file.
		/// </returns>
		public static OpenSearchItem Create ( string file )
		{
			OpenSearchFileType fileType = DetermineOpenSearchFileType (file);
			
			switch (fileType) {
				case OpenSearchFileType.MozillaSearch:
					return CreateMozillaItem (file);
				case OpenSearchFileType.OpenSearch:
					return CreateOpenSearchItem (file);
				default:
					return null;
			}
		}
			
		/// <summary>
		/// Creates an OpenSearchItem from a file in the MozillaSearch format.
		/// </summary>
		/// <param name="file">
		/// The file to process.
		/// </param>
		/// <returns>
		/// A populated OpenSearchItem
		/// </returns>
		private static OpenSearchItem CreateMozillaItem (string file)
		{
			XmlDocument doc = new XmlDocument ();
			doc.Load (file);						
			XmlNamespaceManager namespaceManager = PopulateNamespaceManager (doc);
			
			XmlNode shortName = doc.SelectSingleNode ("//*/default:ShortName", namespaceManager);		
			XmlNode description = doc.SelectSingleNode ("//*/default:Description", namespaceManager);	
			XmlNode url = doc.SelectSingleNode ("//*/default:Url[@type='text/html' and @method='GET']", namespaceManager);
			
			if(shortName == null || description == null || url == null)
				return null;

			string templateUrl = url.Attributes["template"].Value + "?";
			
			// For the Mozilla format, we parse the additional Param nodes to find the one that contains the searchTerms. 
			XmlNodeList paramList = url.ChildNodes;
			foreach (XmlNode node in paramList) {
				// We only want to deal with Param nodes.
				if (node.Name != "Param")
					continue;
				// It's possible to have multiple replacable bits, signified by {sometext}. Since we only know how to 
				// replace searchTerms, we skip the node if it has a {} and isn't searchTerms.
				if (Regex.IsMatch (node.Attributes["value"].Value, "{.*}") && !(Regex.IsMatch(node.Attributes["value"].Value, "{searchTerms}")))
					continue;
				// Append the parameter name and value.
				templateUrl += node.Attributes["name"].Value + "=" + node.Attributes["value"].Value + "&";
			}
				             
			templateUrl = templateUrl.TrimEnd (new char [] {'&','?'});	 

			return new OpenSearchItem (shortName.InnerText, "OpenSearch plugin: " + description.InnerText, templateUrl);
		}
		
		/// <summary>
		/// Creates an OpenSearchItem from a file in the OpenSearch format.
		/// </summary>
		/// <param name="file">
		/// The file to process.
		/// </param>
		/// <returns>
		/// A populated OpenSearchItem
		/// </returns>
		private static OpenSearchItem CreateOpenSearchItem ( string file)
		{
			XmlDocument doc = new XmlDocument ();
			doc.Load (file);						
			XmlNamespaceManager namespaceManager = PopulateNamespaceManager (doc);
			
			XmlNode shortName = doc.SelectSingleNode ("//*/os:ShortName", namespaceManager);		
			XmlNode description = doc.SelectSingleNode ("//*/os:Description", namespaceManager);	
			XmlNode url = doc.SelectSingleNode ("//*/os:Url[@type='text/html' and @method='GET']", namespaceManager);
			
			if(shortName == null || description == null || url == null)
				return null;

			string templateUrl = url.Attributes["template"].Value;
							
			return new OpenSearchItem (shortName.InnerText, "OpenSearch plugin: " + description.InnerText, templateUrl);
		}
		
		/// <summary>
		/// Determines if the provided file is in the OpenSearch format or the MozillaSearch format,
		/// which we need to know so we can parse it.
		/// </summary>
		/// <param name="file">
		/// The source file.
		/// </param>
		/// <returns>
		/// The file type.
		/// </returns>
		private static OpenSearchFileType DetermineOpenSearchFileType (string file)
		{
			XmlDocument doc = new XmlDocument ();
			doc.Load (file);				
			
			// Figure out what namepsaces are in the document. 
			XmlNamespaceManager namespaceManager = PopulateNamespaceManager (doc);
						
			// An OpenSearch document can both the mozilla namespace and the opensearch namespace, 
			// so we figure if it contains either...
			bool isMozillaSearch = (namespaceManager.LookupPrefix ("http://www.mozilla.org/2006/browser/search/") != null);
			bool isOpenSearch = (namespaceManager.LookupPrefix ("http://a9.com/-/spec/opensearch/1.1/") != null);
					
			// ...and then apply a little logic.
			if (isOpenSearch)
				return OpenSearchFileType.OpenSearch;
						
			if (isMozillaSearch && !isOpenSearch)
				return OpenSearchFileType.MozillaSearch;
			
			return OpenSearchFileType.Unknown;
		}
		
		/// <summary>
		/// Populates the namespace manager with all the namespaces in the document.
		/// </summary>
		/// <param name="doc">
		/// The xml document to retrieve the namespaces from.
		/// </param>
		/// <returns>
		/// The populated namespace manager.
		/// </returns>
		private static XmlNamespaceManager PopulateNamespaceManager (XmlDocument doc)
		{
		    XmlNamespaceManager namespaceManager = new XmlNamespaceManager ( doc.NameTable) ;
		    foreach (XmlAttribute attr in doc.SelectSingleNode ( "/*") .Attributes)
			{
		        if (attr.Prefix == "xmlns") 
					namespaceManager.AddNamespace (attr.LocalName , attr.Value);
				if (attr.Name == "xmlns")
					namespaceManager.AddNamespace ("default",attr.Value);
			}
		    return namespaceManager;
		} 
	}
	
	/// <summary>
	/// There are two formats that we need to handle for OpenSearch. The first 
	/// is the actual OpenSearch format, which I would love to have be the only.
	/// The second is what I call the MozillaSearch format, which is the format 
	/// Mozilla has shipped its default plugins in (starting with Firefox 2).
	/// </summary>
	public enum OpenSearchFileType
	{
		OpenSearch,
		MozillaSearch,
		Unknown
	}
}
