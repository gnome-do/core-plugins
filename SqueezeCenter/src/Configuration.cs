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
			this.Build();
			this.LoadSettingsFromFile ();
		}
		
		void LoadSettingsFromFile ()
		{
			SqueezeCenter.Settings.ReadSettings (Server.ConfigFile, Server.settings.Values, true);
			
			this.txtHost.Text = Server.settings["Host"].Value;
			this.txtPortCli.Text = Server.settings["CLIPort"].ValueAsInt.ToString ();
			this.txtPortWeb.Text = Server.settings["WebPort"].ValueAsInt.ToString ();
			this.chkLoadInBackground.Active = Server.settings["LoadInBackground"].ValueAsBool;
			this.txtRadios.Text = Server.settings["Radios"].Value;			
		}

		protected virtual void OntbnSaveClicked (object sender, System.EventArgs e)		
		{
			int i;
			if (!int.TryParse (this.txtPortCli.Text.Trim (), out i) )
			{
				ShowErrorMessage ("CLI port must be an integer!");
				return;
			}
			
			if (!int.TryParse (this.txtPortWeb.Text.Trim (), out i) )
			{
				ShowErrorMessage ("Web port must be an integer!");
				return;
			}
			
			Server.settings["Host"].Value = this.txtHost.Text.Trim ();
			Server.settings["CLIPort"].ValueAsInt = int.Parse (this.txtPortCli.Text.Trim ());
			Server.settings["WebPort"].ValueAsInt = int.Parse (this.txtPortWeb.Text.Trim ());
			Server.settings["LoadInBackground"].ValueAsBool = this.chkLoadInBackground.Active;
			Server.settings["Radios"].Value = this.txtRadios.Text.Trim ();
			
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
