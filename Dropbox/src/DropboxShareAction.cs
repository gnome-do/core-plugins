// 
// DropboxShareAction.cs
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
using Do.Universe.Common;
using Do.Platform;


namespace Dropbox
{
	
	
	public class DropboxShareAction : DropboxAbstractAction
	{
				
		public override string Name {
			get { return "Share with Dropbox";  }
		}
		
		public override string Description {
			get { return "Links a file to your Dropbox public folder."; }
		}
		
		public override string Icon {
			get { return "dropbox"; }
		}
		
		public override bool SupportsItem (Item item) 
		{
			string path = GetPath (item);
			
			return File.Exists (path) && 
				!path.StartsWith (dropbox.PublicPath) && 
				!HasLink (path);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (Item item in items) {
				string target = GetPath (item);
				string extension = Path.GetExtension (target);
				string filename = Path.GetFileNameWithoutExtension (target);
				string link_name = Path.Combine (dropbox.DoSharedPath, 
					String.Format ("{0}-{1}{2}", filename, rand.Next (), extension));
				
				Directory.CreateDirectory (dropbox.DoSharedPath);
				
				if (MakeLink (target, link_name)) {
					
					string url = dropbox.GetPubUrl (link_name);	
					yield return new BookmarkItem (url, url);
				
				}
			}
		}

	}
}

