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
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Addins;
using Do.Universe;

using Mono.Unix;

namespace Pastebin
{
	public class PastebinAction : AbstractAction
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
		
		public override Type[] SupportedItemTypes
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
		
		public override Type[] SupportedModifierItemTypes
		{
			get 
			{
				return new Type[] {
					typeof (ITextSyntaxItem)
				};
			}
		}
		
		public override bool SupportsItem (IItem item)
		{
			if (item is ITextItem) return true;
			if ((item as FileItem).MimeType.StartsWith ("text/") || 
				(item as FileItem).MimeType.EndsWith ("/x-perl"))
				return true;
			return false;
		}
			
		public override IItem[] DynamicModifierItemsForItem (IItem item)
		{
			try
			{
				Paste2 pastebinProvider = new Paste2 ();
				List<IItem> items = new List<IItem> ();

				foreach(IItem languageItem in pastebinProvider.SupportedLanguages)
					items.Add (languageItem);
										
				return items.ToArray ();
			}
			catch
			{
				return null;
			}
		}
				
		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{		
			string text = string.Empty;
			ITextItem titem = null;
			
			foreach (IItem item in items)
			{
				if (item is IFileItem)
					titem = new TextItem (File.ReadAllText (
						(item as IFileItem).Path));
				else
					titem = new TextItem ((item as ITextItem).Text);
				text += titem.Text;
			}
			
			Paste2 pastebinProvider = null;
					
			if (modifierItems.Length > 0)
			{
				pastebinProvider = new Paste2 (text, (modifierItems[0] as ITextSyntaxItem).Syntax);
			}
			else
			{
				pastebinProvider = new Paste2 (text);		
			}
					
			string url = Pastebin.PostUsing (pastebinProvider);	
					
			return new IItem[] { new TextItem (url) };
		}
		
	}
}


