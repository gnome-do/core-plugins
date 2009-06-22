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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Platform;
using Do.Universe;
using Do.Universe.Common;

using Mono.Addins;

namespace Quote
{
	public class PostQuote : Act
	{
		// thanks Ian Warford (iwarford) for the regexp help
		const string TimeStampRegexp = @"\n\s*\S?\d\d:\d\d(:\d\d)?\S?\s*|^\s*\S?\d\d:\d\d(:\d\d)?\S?\s*";
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Submit Quote"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Sends text to Quote service."); }
		}
		
		public override string Icon {
			get { return "quoted-globe.svg@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override bool ModifierItemsOptional  {
			get { return true; }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				yield return typeof (ITextItem);
				yield return typeof (QuoteTagItem);
			}
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			IQuoteProvider quoteProvider = QuoteProviderFactory.GetProviderFromPreferences ();
		
			return quoteProvider.SavedTags.Cast<Item> ();
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			Regex timestamps = new Regex (TimeStampRegexp, RegexOptions.Compiled);
			
			string text;
			string tags = "";
			IQuoteProvider quoteProvider;
		
			text = (items.First () as ITextItem).Text;
			text = timestamps.Replace (text, "\n");

			Console.Error.WriteLine (text);
		
			foreach (Item tag in modifierItems) {
				tags += tag is QuoteTagItem
					? (tag as QuoteTagItem).Name
					: (tag as ITextItem).Text;
		
				tags += " ";
			}
		
			quoteProvider = string.IsNullOrEmpty (tags)
				? QuoteProviderFactory.GetProviderFromPreferences (text)
				: QuoteProviderFactory.GetProviderFromPreferences (text, tags);		
		
			string url = Quote.PostUsing (quoteProvider);
		
			AddUnknownTags (tags, quoteProvider);
		
			yield return new BookmarkItem (url, url);
		}
		
		void AddUnknownTags (string tags, IQuoteProvider service)
		{
			QuoteTagItem tag;
		
			foreach (string tagName in tags.Trim ().Split (' ')) {
				tag = new QuoteTagItem (tagName);
		
				if (service.SavedTags.Contains (tag)) continue;
				
				service.AddTag (tag);
			}
		}
	}
}

