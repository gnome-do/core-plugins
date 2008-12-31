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
	public class OpenSearchItemSource : ItemSource
	{	
		List<Item> items;
		
		public OpenSearchItemSource ()
		{
			items = new List<Item> ();
			UpdateItems();
		}		
		
		public override string Name {
			get {
				return Catalog.GetString ("Open Search Items");
			}
		}

		public override string Description {
			get {
				return Catalog.GetString ("Installed Open Search Items");
			}
		}

		public override string Icon {
			get {
				return "www";
			}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (IOpenSearchItem)};
			}
		}

		public override IEnumerable<Item> Items {
			get {
				return items;
			}
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			return null;
		}

		public override void UpdateItems ()
		{
			List<Item> openSearchItems = new List<Item> ();
			
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
						
						OpenSearchItem item = OpenSearchParser.Create (filePath);
						if (item != null) {
							openSearchItems.Add (item);
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
