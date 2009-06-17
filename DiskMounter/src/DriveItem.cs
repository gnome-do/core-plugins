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
using Do.Platform;

using Gnome.Vfs;

namespace DiskMounter
{
	public class DriveItem : Item, IUriItem
	{
		private Drive drive;
		
		public DriveItem(Drive drive)
		{
			this.drive = drive;
		}
		
		public override string Name {
			get { return drive.DisplayName; }
		}
		
		public override string Description {
			get {
				string status = IsMounted ? Uri : "Unmounted";
				return drive.DeviceType.ToString () + " (" + status + ")";
			}
		}
		
		public override string Icon {
			get { return IsMounted ? drive.MountedVolume.Icon : drive.Icon; }
		}
		
		public string Uri {
			get { return IsMounted ? drive.MountedVolume.ActivationUri : ""; }
		}
		
		public void Unmount ()
		{
			try {
				if (drive.NeedsEject ())
					drive.Eject (new VolumeOpCallback (OnEject));
				else 
					drive.Unmount (new VolumeOpCallback (OnUnmount));
			} catch {
			    // error message will be handled by VolumeOpCallback                                                     
			}
		}
		
		public void Mount ()
		{
			try {
				drive.Mount (new VolumeOpCallback (OnMount));
			} catch (Exception ex) {
				Log.Debug ("An error occurred while executing the Mount operation.");
				Log.Error (ex.Message);
			}
		}
		
		public bool IsMounted
		{
			get {
				return drive.IsMounted && drive.MountedVolumes.Count > 0;
			}
		}
		
		void OnMount (bool succeeded, string error, string detailed_error)
		{
			if (succeeded)
				Log.Debug ("Mount operation succeeded");
			else
				Log.Error ("Mountt operation failed {0}, detail: {1}", error, detailed_error);
		}

		void OnUnmount (bool succeeded, string error, string detailed_error)
		{
			if (succeeded)
				Log.Debug ("Unmount operation succeeded");
			else
				Log.Error ("Unmount operation failed {0}, detail: {1}", error, detailed_error);
		}
		
		void OnEject (bool succeeded, string error, string detailed_error)
		{
			if (succeeded)
				Log.Debug ("Eject operation succeeded");
			else
				Log.Error ("Eject operation failed {0}, detail: {1}", error, detailed_error);
		}
	}
}
