// DriveItemSource.cs
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
// along with this program; if not, see <http://www.gnu.org/licenses/> or 
// write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, 
// Boston, MA 02111-1307 USA
//

using System;
using System.Collections.Generic;
using Gnome.Vfs;

using Mount;
using Do.Universe;

namespace Mount
{
	public class DriveItemSource : IItemSource
	{
		List<IItem> items;
                private static Gnome.Vfs.VolumeMonitor monitor;
                
		public DriveItemSource ()
		{
                        Vfs.Initialize ();
			monitor = Gnome.Vfs.VolumeMonitor.Get();
			items = new List<IItem> ();
			UpdateItems ();
		}
                
		public string Name {
			get { return "Drive Item"; }
		}
		
		public string Description {
			get { return ""; }
		}
		
		public string Icon {
			get { return "harddrive"; }
		}
                
		public Type[] SupportedItemTypes {
			get {
			return new Type[] {
					typeof (MountedDriveItem),
                                        typeof (UnmountedDriveItem),
				};
			}
		}

		public ICollection<IItem> Items {
			get { return items; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		public void UpdateItems ()
		{

			Gnome.Vfs.Drive[] drives;
			
			try {                       
                                drives = monitor.ConnectedDrives;
			        items.Clear();
                      
			        foreach (Drive drive in drives){
                                        if (drive.IsMounted)
                                                items.Add (new MountedDriveItem(drive));
                                        else
                                                items.Add (new UnmountedDriveItem(drive));                 
                                }
			
			} catch (Exception e) {
				Console.Error.WriteLine ("Something went wrong while updating DriveItemSource :  {0}" , e.Message);
			}

		}  
                
	}
}