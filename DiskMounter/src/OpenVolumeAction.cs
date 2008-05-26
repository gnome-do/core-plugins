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

using Gnome.Vfs;

using Do.Universe;
using Do.Addins;

namespace DiskMounter
{
	public class OpenVolume : AbstractAction
	{
	
		public OpenVolume ()
        {
        }
                
		public override string Name {
			get { return "Open"; }
		}
		
		public override string Description {
			get { return "Open"; }
		}
		
		public override string Icon {
			get { return "gtk-open"; }
		}
		
		public override Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (DriveItem),
				};
			}
		}
                
		public override bool SupportsItem (IItem item) 
        {
			return true;
		}
                
        public override bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
        {
			return false;
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			DriveItem drive = (DriveItem) items[0];
			try {
				if (!drive.IsMounted)
					drive.Mount ();
				Do.Addins.Util.Environment.Open(drive.Path);
			} catch (Exception e) {
				Console.WriteLine("Error opening {0} - {1}", (items[0] as DriveItem).Path, e.Message);
			}
			return null;
		}
	}
}
