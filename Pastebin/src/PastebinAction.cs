//  PastebinAction.cs
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


using Do.Universe;

using Mono.Unix;

namespace Pastebin
{
	public class PastebinAction : Act, IConfigurable
	{
		public override string Name
		{
			get { return Catalog.GetString ("Send to Pastebin"); }
		}
		
		public override string Description
		{
			get { return Catalog.GetString ("Sends the text to Pastebin."); }
		}
		
		public override string Icon
		{
			get { return "gtk-paste"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get 
			{
				return new Type[] {
					typeof (ITextItem),
					typeof (IFileItem)
				};
			}
		}
			
		public override bool ModifierItemsOptional
		{
			get { return true; }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get 
			{
				return new Type[] {
					typeof (ITextSyntaxItem)
				};
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) return true;
			if (item is FileItem) {	
				if ((item as FileItem).MimeType.StartsWith ("text/") || 
				(item as FileItem).MimeType.EndsWith ("/x-perl"))
				return true;
			}
			return false;
		}
			
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			IPastebinProvider pastebinProvider = PastebinProviderFactory.GetProviderFromPreferences ();

			List<Item> items = new List<Item> ();

			foreach (Item languageItem in pastebinProvider.SupportedLanguages) {
				items.Add (languageItem);
			}
										
			return items.ToArray ();
		}
				
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{		
			string text = string.Empty;
			ITextItem titem = null;
			
			foreach (Item item in items) {
				if (item is IFileItem)
					titem = new TextItem (File.ReadAllText (
						(item as IFileItem).Path));
				else
					titem = new TextItem ((item as ITextItem).Text);
				text += titem.Text;
			}
			
			IPastebinProvider pastebinProvider = null;
					
			if (modifierItems.Any ()) {
				pastebinProvider = PastebinProviderFactory.GetProviderFromPreferences (text, (modifierItems.First () as ITextSyntaxItem).Syntax);
			}
			else {
				pastebinProvider = PastebinProviderFactory.GetProviderFromPreferences (text);		
			}
					
			string url = Pastebin.PostUsing (pastebinProvider);	
					
			return new Item[] { new TextItem (url) };
		}
				
		public Gtk.Bin GetConfiguration ()
		{
			return new PastebinConfig();
		}				
	}
}


