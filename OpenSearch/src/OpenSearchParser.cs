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
		readonly static string short_name_xpath = "//*/{0}:ShortName";
		readonly static string description_xpath = "//*/{0}:Description";
		readonly static string url_xpath = "//*/{0}:Url[@type='text/html' and @method='GET']";
		
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
		public static OpenSearchItem Create (string file)
		{	
			string elementNamespace = DetermineOpenSearchElementNamespace (file);		
						
			XmlDocument doc = new XmlDocument ();
			doc.Load (file);						
			XmlNamespaceManager namespaceManager = PopulateNamespaceManager (doc);
			
			XmlNode shortName = doc.SelectSingleNode (string.Format(short_name_xpath, elementNamespace), namespaceManager);		
			XmlNode description = doc.SelectSingleNode (string.Format(description_xpath, elementNamespace), namespaceManager);	
			XmlNode url = doc.SelectSingleNode (string.Format(url_xpath, elementNamespace), namespaceManager);

			if (description == null)
				description = shortName;
			
			if (shortName == null || description == null || url == null)
				return null;
			
			string templateUrl = url.Attributes["template"].Value;
			
			// If the template url doesn't contain the searchTerms text, we'll 
			// have to search params for it.
			if (!Regex.IsMatch (templateUrl, "{searchTerms}"))
			{			
				templateUrl += "?";
				
				// Parse the additional Param nodes to find the one that contains the searchTerms. 
				XmlNodeList paramList = url.ChildNodes;
				foreach (XmlNode node in paramList) {
					// We only want to deal with Param nodes.
					if (node.Name != string.Format ("{0}:Param", elementNamespace) && node.Name != "Param")
						continue;
					// It's possible to have multiple replacable bits, signified by {sometext}. Since we only know how to 
					// replace searchTerms, we skip the node if it has a {} and isn't searchTerms.
					if (Regex.IsMatch (node.Attributes["value"].Value, "{.*}") && !(Regex.IsMatch(node.Attributes["value"].Value, "{searchTerms}")))
						continue;
					// Append the parameter name and value.
					templateUrl += node.Attributes["name"].Value + "=" + node.Attributes["value"].Value + "&";
				}
					             
				templateUrl = templateUrl.TrimEnd (new [] {'&', '?'});	
			}
			
			return new OpenSearchItem (shortName.InnerText, description.InnerText, templateUrl);
		}			
		
		/// <summary>
		/// Determines the namespace we should look for OpenSearch elements in when parsing the documents.
		/// </summary>
		/// <param name="file">
		/// The source file.
		/// </param>
		/// <returns>
		/// The namespace to look for elements in.
		/// </returns>
		private static string DetermineOpenSearchElementNamespace (string file)
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
				return "os";
						
			if (isMozillaSearch && !isOpenSearch)
				return "default";
			
			throw new Exception ("Unable to determine OpenSearch plugin type.");
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
}
