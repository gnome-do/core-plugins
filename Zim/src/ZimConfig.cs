// ZimConfig.cs 
// User: Karol Będkowski at 18:42 2008-10-21
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
using System.Collections.Generic;
using Mono.Unix;
using Do.Addins;


namespace Zim
{
	
	
	[System.ComponentModel.Category("Zim")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ZimConfig : Gtk.Bin {
		
		static IPreferences prefs;
		
		
		public ZimConfig() {
			this.Build();

			string current = prefs.Get("searchIn", "");
			int currentIdx = 0;
			
			this.cbNotebooks.AppendText(Catalog.GetString("<all>"));
			Dictionary<string, string> notebooks = Zim.LoadNotebooks();			
			int idx = 1;
			foreach (string key in notebooks.Keys) {
				this.cbNotebooks.AppendText(key);
				if (key == current) {
					currentIdx = idx; 
				}
				idx ++;
			}
			
			this.cbNotebooks.Active = currentIdx;
		}
		
		static ZimConfig() {
			prefs = Do.Addins.Util.GetPreferences("ZimPlugin");
		}

		protected virtual void OnCbNotebooksChanged (object sender, System.EventArgs e)	{
			string selected = this.cbNotebooks.ActiveText;
			if (selected == Catalog.GetString("<all>")) {
				selected = "";
			}
			
			prefs.Set("searchIn", selected);	
		}
	
	}
}
