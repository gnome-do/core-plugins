////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 3 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace SqueezeCenter
{
	public partial class Configuration : Gtk.Bin
	{
		public Configuration()
		{
			Build();
			LoadSettingsFromFile ();
		}
		
		void LoadSettingsFromFile ()
		{
			SqueezeCenter.Settings.ReadSettings (Server.ConfigFile, Server.settings.Values, true);
			
			txtHost.Text = Server.settings["Host"].Value;
			txtPortCli.Text = Server.settings["CLIPort"].ValueAsInt.ToString ();
			txtPortWeb.Text = Server.settings["WebPort"].ValueAsInt.ToString ();
			chkLoadInBackground.Active = Server.settings["LoadInBackground"].ValueAsBool;
			txtRadios.Text = Server.settings["Radios"].Value;			
		}

		protected virtual void OntbnSaveClicked (object sender, System.EventArgs e)		
		{
			int i;
			if (!int.TryParse (txtPortCli.Text.Trim (), out i)) {
				ShowErrorMessage ("CLI port must be an integer!");
				return;
			}
			
			if (!int.TryParse (txtPortWeb.Text.Trim (), out i)) {
				ShowErrorMessage ("Web port must be an integer!");
				return;
			}
			
			Server.settings["Host"].Value = txtHost.Text.Trim ();
			Server.settings["CLIPort"].ValueAsInt = int.Parse (txtPortCli.Text.Trim ());
			Server.settings["WebPort"].ValueAsInt = int.Parse (txtPortWeb.Text.Trim ());
			Server.settings["LoadInBackground"].ValueAsBool = chkLoadInBackground.Active;
			Server.settings["Radios"].Value = txtRadios.Text.Trim ();
			
			SqueezeCenter.Settings.SaveSettings (Server.ConfigFile, Server.settings.Values);			
			
			// Dispose current server object. 
			// A new one with the new configuration will be created when needed.   
			Server.DisposeInstance ();
		}
		
		void ShowErrorMessage (string msg)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog (null ,  
			                                              Gtk.DialogFlags.DestroyWithParent,
			                                              Gtk.MessageType.Error, 
			                                              Gtk.ButtonsType.Close, msg, 
			                                              new object [] {});
			md.Run ();
			md.Destroy ();
		}
	}
}
