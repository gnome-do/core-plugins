//  OpenSearchItemAction.cs
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
using System.Linq;

using Mono.Unix;


using Do.Universe;

namespace OpenSearch
{
	public class OpenSearchAction : Act
	{		
		public override string Name
		{
			get { return Catalog.GetString ("Search Web"); }
		}
		
		public override string Description
		{
			get { return Catalog.GetString ("Searches the web using OpenSearch plugins."); }
		}
		
		public override string Icon
		{
			get { return "web-browser"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) 
				return true;
			return false;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get {
				return new Type[] {
					typeof (IOpenSearchItem),
				};
			}
		}			
	
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{					
			List<string> searchTerms;
				
			if (modifierItems.Any ()) {
				searchTerms = new List<string> ();
				foreach (Item item in items) {
					searchTerms.Add ((item as ITextItem).Text);
				}
				
				foreach (string searchTerm in searchTerms) {
					string url = (modifierItems.First () as IOpenSearchItem).BuildSearchUrl(searchTerm);
					Util.Environment.Open (url);	
				}
			}
			return null;
		}
	}
}
