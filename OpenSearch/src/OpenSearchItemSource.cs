//  OpenSearchItemSource.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Do.Universe;
using Mono.Unix;

namespace OpenSearch
{
	public class OpenSearchItemSource : IItemSource
	{	
		List<IItem> items;
		
		public OpenSearchItemSource ()
		{
			items = new List<IItem> ();
			UpdateItems();
		}		
		
		public string Name {
			get {
				return Catalog.GetString ("Open Search Items");
			}
		}

		public string Description {
			get {
				return Catalog.GetString ("Installed Open Search Items");
			}
		}

		public string Icon {
			get {
				return "www";
			}
		}

		public Type[] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (IOpenSearchItem)};
			}
		}

		public ICollection<IItem> Items {
			get {
				return items;
			}
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}

		public void UpdateItems ()
		{
			List<IItem> openSearchItems = new List<IItem> ();
			
			FirefoxOpenSearchDirectoryProvider firefoxProvider = new FirefoxOpenSearchDirectoryProvider ();
			string validFilePattern = @"^.*\.xml$";			
			
			foreach (string path in firefoxProvider.OpenSearchPluginDirectories) {
				try {
					if(!Directory.Exists(path))
						continue;
					
					string [] filePaths = Directory.GetFiles (path);
					foreach (string filePath in filePaths) {
						if (!Regex.IsMatch (filePath, validFilePattern))
							continue;
						
						IOpenSearchItem item = OpenSearchParser.Create (filePath);
						if (item != null) {
							openSearchItems.Add ( item );
						}
					}
				} catch {
					continue;
				}
			}
			items = openSearchItems;
		}	
	}
}
