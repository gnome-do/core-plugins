// OpenVolumeAction.cs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Gnome.Vfs;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace DiskMounter
{
	public class OpenVolume : Act
	{
	
		public OpenVolume ()
        {
        }
                
		public override string Name {
			get { return Catalog.GetString ("Open"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Open a removable volume"); }
		}
		
		public override string Icon {
			get { return "gtk-open"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (DriveItem); }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			DriveItem drive = (DriveItem) items.First ();
			try {
				if (!drive.IsMounted)
					drive.Mount ();
				Services.Environment.OpenPath (drive.Path);
			} catch (Exception e) {
				Log.Error ("Error opening {0} - {1}", drive.Path, e.Message);
				Log.Debug (e.StackTrace);
			}
			yield break;
		}
	}
}