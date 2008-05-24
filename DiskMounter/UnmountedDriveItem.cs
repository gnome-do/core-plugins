// UnmountedDriveItem.cs
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
using Do.Universe;

using Gnome.Vfs;

namespace Mount
{
	public class UnmountedDriveItem : IItem
	{

		private Drive drive;
		
		public UnmountedDriveItem (Drive drive){
                        if (!drive.IsMounted)
                                this.drive = drive;
		}

		public void Mount ()
		{
                        try {
                                drive.Mount (VolumeOpCallback);
                        } catch
                        {  // error message will be handled by VolumeOpCallback    
                        }
                         
		}
               
		public void VolumeOpCallback (bool succeeded, string error, string detailed_error)
		{
			if ( succeeded )
				Console.WriteLine("Mount operation succeeded");
			else
				Console.Error.WriteLine("Mount operation failed {0}, detail : {1}" , error, detailed_error );
		}
						
		public string Name {
			get { return drive.DisplayName; }		
		}
		
		public string Description {
			get { return String.Format("{0}",drive.DeviceType); }
		}
		
		public string Icon {
			get { return drive.Icon; }
		}
	}
}