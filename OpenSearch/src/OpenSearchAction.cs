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

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace OpenSearch
{
	public class OpenSearchAction : Act
	{		
		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Search Web"); }
		}
		
		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Searches the web using OpenSearch plugins."); }
		}
		
		public override string Icon
		{
			get { return "web-browser"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (ITextItem);
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				yield return typeof (IOpenSearchItem);
			}
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			CachingOpenSearchItemSource.UpdateItems ();
			return CachingOpenSearchItemSource.Items;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			items.Cast<ITextItem> ()
				.Select (item => modItems.Cast<IOpenSearchItem> ().First ().BuildSearchUrl (item.Text))
				.ForEach (Services.Environment.OpenUrl);

			yield break;
		}
	}
}
