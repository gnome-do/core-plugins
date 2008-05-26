// DriveItem.cs
// 
// Copyright (C) 2008 [Alex Launi]
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
using Do.Universe;

using Gnome.Vfs;

namespace DiskMounter
{
	public class DriveItem : IItem
	{
		private Drive drive;
		
		public DriveItem(Drive drive)
		{
			this.drive = drive;
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
		
		public string Path {
			get { return drive.ActivationUri.ToString(); }
		}
		
		public void Unmount ()
		{
			try {
				drive.Unmount (VolumeOpCallback);
			} catch
			{    // error message will be handled by VolumeOpCallback                                                     
			}
		}
		
		public void Mount ()
		{
			try {
				drive.Mount (VolumeOpCallback);
			} catch
			{  // error message will be handled by VolumeOpCallback    
			}
		}
		
		public bool IsMounted
		{
			get {
				return drive.IsMounted;
			}
		}
                			
		public void VolumeOpCallback (bool succeeded, string error, string detailed_error)
		{
			if ( succeeded )
				Console.WriteLine("Unmount operation succeeded");
			else
				Console.Error.WriteLine("Unmount operation failed {0}, detail : {1}" , error, detailed_error );
		}
	}
}
