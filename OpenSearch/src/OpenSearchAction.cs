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
using System.Web;

using Mono.Unix;

using Do.Addins;
using Do.Universe;

namespace Do.Plugins.OpenSearch
{
	public class OpenSearchAction : AbstractAction
	{
		static System.Threading.Thread th;
		
		static OpenSearchAction()
		{
			th = new System.Threading.Thread (new System.Threading.ThreadStart (OpenSearch.Init));
			th.IsBackground = true;
			th.Priority = System.Threading.ThreadPriority.Lowest;
			th.Start ();
			//OpenSearch.GetOpenSearchItems ();
		}
		
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
		
		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			if (item is ITextItem) 
				return true;
			return false;
		}
		
		public override Type[] SupportedModifierItemTypes
		{
			get {
				return new Type[] {
					typeof (OpenSearchItem),
				};
			}
		}
				
		public override IItem[] DynamicModifierItemsForItem (IItem item)
		{
			try
			{
				List<IItem> items = new List<IItem>();
				foreach(IItem openSearchItem in OpenSearch.GetOpenSearchItems ()) {
					items.Add (openSearchItem);
				}
				return items.ToArray ();
			}
			catch
			{
				return null;
			}
		}
	
		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			if (th.IsAlive) return null;
			
			List<string> searchTerms;
				
			if (modifierItems.Length > 0)
			{
				searchTerms = new List<string> ();
				foreach (IItem item in items) {
					searchTerms.Add ((item as ITextItem).Text);
				}
				
				foreach (string searchTerm in searchTerms) {
					string url = BuildSearchUrl ((modifierItems[0] as IOpenSearchItem), searchTerm);
					Util.Environment.Open (url);	
				}
			}
			return null; 
		}
		
		private static string BuildSearchUrl (IOpenSearchItem openSearchItem, string searchTerm)
		{
			return openSearchItem.UrlTemplate.Replace ("{searchTerms}",  HttpUtility.UrlEncode(searchTerm));
		}
	}
}
