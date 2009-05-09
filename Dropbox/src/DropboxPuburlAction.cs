/* DropboxPuburlAction.cs
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
using System.Linq;
using System.Collections.Generic;
 
using Do.Universe;
using Do.Universe.Common;
using Do.Platform;

using Mono.Unix;


namespace Dropbox
{
	
	
	public class DropboxPuburlAction : Act
	{
				
		public override string Name {
			get { return "Public URL";  }
		}
		
		public override string Description {
			get { return "Get public url of a file in your dropbox."; }
		}
		
		public override string Icon {
			get { return "dropbox"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}
		
		public override bool SupportsItem (Item item) 
		{
			string path = (item as IFileItem).Path;
			
			return File.Exists (path) && 
				(path.StartsWith (Dropbox.PublicPath) || 
				Dropbox.FileIsShared (path));
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string path = (items.First () as IFileItem).Path;
			string url = Dropbox.GetPubUrl (path);
			
			if (url == "") {
				
				string msg = String.Format ("Sorry, the file \"{0}\" is not public", 
					UnixPath.GetFileName (path));
				
				Notification notification = new Notification ("Dropbox", msg, "dropbox");
				Services.Notifications.Notify (notification);
				
			} else {
				yield return new BookmarkItem (url, url);
			}
			
		}

	}
}
