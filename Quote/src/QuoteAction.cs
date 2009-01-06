/* QuoteAction.cs 
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Platform;
using Do.Universe;
using Do.Universe.Common;

using Mono.Unix;

namespace Quote
{
	public class PostQuote : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Submit Quote"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Sends text to Quote service."); }
		}
		
		public override string Icon {
			get { return "gtk-paste"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
			}
		}
			
		public override bool ModifierItemsOptional  {
			get { return true; }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get { yield return typeof (ITextItem); }
		}
			
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			IQuoteProvider quoteProvider = QuoteProviderFactory.GetProviderFromPreferences ();
			return quoteProvider.SavedTags.OfType<Item> ();
		}
				
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			ITextItem titem = null;
			string text = string.Empty;
			
			
			foreach (Item item in items) {
				titem = new TextItem ((item as ITextItem).Text);
				text += titem.Text;
			}
			
			IQuoteProvider quoteProvider = null;
					
			if (modifierItems.Any ()) {
				quoteProvider = QuoteProviderFactory.GetProviderFromPreferences (text, (modifierItems.First () as ITextItem).Text);
			} else {
				quoteProvider = QuoteProviderFactory.GetProviderFromPreferences (text);		
			}
					
			string url = Quote.PostUsing (quoteProvider);	
					
			yield return new TextItem (url);
		}
	}
}


