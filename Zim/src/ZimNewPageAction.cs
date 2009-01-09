// ZimNewPageAction.cs 
// User: Karol Będkowski at 12:30 2008-10-26
//
//Copyright Karol Będkowski 2008
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;

namespace Zim
{
	
	/// <summary>
	/// Create new zim page (as subpage to selected).
	/// </summary>
	public class ZimNewPageAction: Act {
	
		public override string Name {
			get { return Catalog.GetString ("New Zim page"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Create new page in Zim"); }
		}
		
		public override string Icon {
			get { return "document-new"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
            get { yield return typeof (ZimPage); }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}
	
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
			ZimPage page = items.First () as ZimPage;
			ITextItem newPageNameItem = modItems.First () as ITextItem;

			Process zim = new Process ();
			zim.StartInfo.FileName = "zim";						
	        zim.StartInfo.Arguments = page.Notebook + " " + page.Name + ":" + newPageNameItem.Text; 	            
			zim.Start ();
			
			yield break;
		}			
	}
}
