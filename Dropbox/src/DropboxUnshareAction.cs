// 
// DropboxUnshareAction.cs
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

using Mono.Addins;
 
using Do.Universe;
using Do.Universe.Common;
using Do.Platform;


namespace Dropbox
{
	
	public class DropboxUnshareAction : DropboxAbstractAction
	{
	
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Stop sharing with Dropbox");  }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Unlinks a file from your Dropbox public folder."); }
		}
		
		public override string Icon {
			get { return ("dropbox-remove.png@") + GetType ().Assembly.FullName; }
		}
		
		public override bool SupportsItem (Item item) 
		{
			string path = GetPath(item);

			return File.Exists (path) && HasLink (path);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (Item item in items) {
				string path = GetPath (item);
				string link_path = GetLink (path);
				
				Unlink (link_path);
				
				Notify (String.Format (AddinManager.CurrentLocalizer.GetString ("Stopped sharing") + " {0}", path));
			}
			
			yield break;
		}

	}
}

