// SystemServicesConfig.cs 
// User: Karol Będkowski at 10:39 2008-10-24
//
//Copyright Karol Będkowski 2008
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using Gtk;
using Mono.Unix;
using Mono.Addins;

namespace SystemServices {
	
	[System.ComponentModel.Category("SystemServices")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SystemServicesConfig : Gtk.Bin
	{
		public SystemServicesConfig () {
			this.Build ();

			// create checkbox for each service found in /etc/*/init.d/*
			IDictionary<string, bool> services = SystemServices.GetServicesNamesWithStatus ();
			foreach (KeyValuePair<string, bool> service in services) {
				CheckButton cbutton = new CheckButton (service.Key);
				// and set active 
				cbutton.Active = service.Value;				
				this.boxServicesList.Add (cbutton);
				cbutton.Clicked += OnServiceCheckToggled;
			}
			
			this.eCommand.Text = SystemServices.SudoCommand;
		}

		/// <summary>
		/// On update eCommand textbox save this text to preferences.
		/// </summary>
		protected virtual void OnECommandChanged (object sender, System.EventArgs e) {
			SystemServices.SudoCommand = this.eCommand.Text;
		}

		/// <summary>
		/// On check or uncheck each checkbox update list of user services.
		/// </summary>
		protected virtual void OnServiceCheckToggled (object sender, System.EventArgs e) {
			CheckButton cbutton = sender as CheckButton;
			if (cbutton.Active) {
				SystemServices.AddItemToUserlist (cbutton.Label);
			} else {
				SystemServices.RemoveItemFromUserklist (cbutton.Label);
			}			
		}

		/// <summary>
		/// On click "..." button - select file
		/// </summary>
		protected virtual void OnBtnSelectFileClicked (object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog (
					AddinManager.CurrentLocalizer.GetString ("Choose the file to open"), new Dialog(),
					Gtk.FileChooserAction.Open, 
					AddinManager.CurrentLocalizer.GetString ("Cancel"), ResponseType.Cancel,
					AddinManager.CurrentLocalizer.GetString ("Open"), ResponseType.Accept);

			if (!string.IsNullOrEmpty (this.eCommand.Text)) {
				fc.SetFilename(this.eCommand.Text);
			}

			if (fc.Run() == (int) ResponseType.Accept) {
				// check
				UnixFileInfo info = new UnixFileInfo (fc.Filename);
				if ((info.FileAccessPermissions & FileAccessPermissions.UserExecute) 
				    	!= FileAccessPermissions.UserExecute) {
					
					MessageDialog md = new MessageDialog (new Dialog(), 
							DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, 
							AddinManager.CurrentLocalizer.GetString ("Selected invalid file!\nShould be executable."));
					md.Run ();
					md.Destroy();
				} else {
					this.eCommand.Text = fc.Filename;
				}
			}

			fc.Destroy();
		}
	
		
	}
}
