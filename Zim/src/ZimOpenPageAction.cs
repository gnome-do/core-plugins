// ZimOpenPageAction.cs 
// User: Karol Będkowski at 18:01 2008-10-19
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

using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace Zim
{
	
	/// <summary>
	/// Open selected page in Zim. 
	/// If item is ITextItem we can enter ":page" for default notebook or "notebook :page".
	/// </summary>
	public class ZimOpenPageAction: Act {
	
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Open Zim page"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString("Open selected page in Zim"); }
		}
		
		public override string Icon {
			get { return "zim";	}
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
            get {
				yield return typeof (ITextItem);
				yield return typeof (ZimPage);
			}
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Item item = items.First ();
			using (Process zim = new Process ()) {
				zim.StartInfo.FileName = "zim";		
				
	            if (item is ITextItem) {
	                ITextItem textitem = item as ITextItem;
	                string args = textitem.Text;
	                if (!args.Contains (" :")) {
	                	args = "_default_ " + args;            
	                }
					
	                zim.StartInfo.Arguments = args; 	                
	            } else {
	                ZimPage page = item as ZimPage;
	                zim.StartInfo.Arguments = page.Notebook + " " + page.Name; 
	            }
				zim.Start ();
			}
			yield break;
		}		

	}
}
