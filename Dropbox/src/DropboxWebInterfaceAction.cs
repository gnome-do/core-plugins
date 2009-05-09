// 
// DropboxWebInterfaceAction.cs
// 
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this
// source distribution.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Do.Universe;
using Do.Platform;

namespace Dropbox
{
	
	
	public class DropboxWebInterfaceAction : Act
	{
		
		public override string Name {
			get { return "Dropbox web interface";  }
		}
		
		public override string Description {
			get { return "Views folder in Dropbox web interface."; }
		}
		
		public override string Icon {
			get { return "dropbox"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (IApplicationItem),
					typeof (IFileItem)
				};
			}

		}
		
		public override bool SupportsItem (Item item) 
		{
			if (item is IApplicationItem) {
				return item.Name == "Dropbox";
			} else {
				string path = (item as IFileItem).Path;
				return path.StartsWith (Dropbox.BasePath) &&
					Directory.Exists (path);
			}
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string url, path;
			Item item = items.First ();
			
			if (item is IApplicationItem) {
				url = Dropbox.GetWebUrl ();
			} else {
				path = (item as IFileItem).Path;
				url = Dropbox.GetWebUrl (path);
			}
			
			Services.Environment.OpenUrl (url);
			
			return null;
		}
	}
}
