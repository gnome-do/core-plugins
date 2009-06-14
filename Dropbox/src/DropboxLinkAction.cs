// 
// DropboxLinkAction.cs
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

using Mono.Unix;

using Do.Universe;
using Do.Universe.Common;
using Do.Platform;

namespace Dropbox
{
	
	public class DropboxLinkAction : DropboxAbstractAction
	{
				
		public override string Name {
			get { return Catalog.GetString ("Add to Dropbox...");  }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Links a file or folder to your Dropbox."); }
		}
		
		public override string Icon {
			get { return ("dropbox-add.png@") + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (IFileItem); }
		}
	
		public override bool SupportsItem (Item item) 
		{
			string path = GetPath (item);
			
			return !path.StartsWith (Dropbox.BasePath);
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			string path = GetPath (modItem);
			
			return Directory.Exists (path) && path.StartsWith (Dropbox.BasePath);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string target, folder, link_name;

			foreach (Item i in items) {
				target = GetPath (i);
				folder = GetPath (modItems.First ());
				link_name = Path.Combine (folder, Path.GetFileName (target));

				if (MakeLink (target, link_name))
					yield return Services.UniverseFactory.NewFileItem (link_name) as Item;
			}
			
			yield break;
		}


	}
}
