//  AbstractPastebinProvider.cs
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;

using Do.Platform;

namespace Pastebin
{
	public abstract class AbstractPastebinProvider : IPastebinProvider
	{
		public AbstractPastebinProvider()
		{
			ShouldAllowAutoRedirect = true;
			UserAgent = "";
			Expect100Continue = true;
			Parameters = new NameValueCollection ();
			SupportedLanguages = new List<TextSyntaxItem> ();
		}
		
		public bool ShouldAllowAutoRedirect { get; protected set; }
		
		public string UserAgent { get; protected set; }
		
		public bool Expect100Continue { get; protected set; }
		
		public NameValueCollection Parameters { get; protected set; }
				
		public List<TextSyntaxItem> SupportedLanguages { get; protected set;}
		
		public string Name { get; protected set; }
		
		public string BaseUrl { get; protected set; }
		
		public abstract string GetPasteUrlFromResponse (HttpWebResponse response);
		
		protected List<TextSyntaxItem> PopulateTextSyntaxItemsFromXml (string resourceName)
		{
			try
			{				
				string textSyntaxItemXml;
				using (Stream resourceStream = Assembly.GetAssembly (this.GetType ()).GetManifestResourceStream (resourceName)) {
					using (StreamReader reader = new StreamReader (resourceStream)) {
						textSyntaxItemXml = reader.ReadToEnd ();
					}
				}
				using (StringReader reader = new StringReader (textSyntaxItemXml)) {
					XmlSerializer serializer = new XmlSerializer (typeof (List<TextSyntaxItem>));
					return (List<TextSyntaxItem>) serializer.Deserialize (reader);
				}
			} catch (Exception e) {
				Log<AbstractPastebinProvider>.Error (e.ToString ());
				return new List<TextSyntaxItem> ();
			}
		}
	}
}
