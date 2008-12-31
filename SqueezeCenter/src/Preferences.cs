// Preferences.cs created with MonoDevelop
// User: anders at 13:21 06/15/2008
//
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
	
	public partial class Preferences : Gtk.Bin
	{
		
		public Preferences()
		{
			this.Build();
			this.LoadSettingsFromFile();
		}

		void LoadSettingsFromFile ()
		{
			SqueezeCenter.Settings.ReadSettings ("SqueezeCenter", Server.settings.Values, true);
			
			this.txtHost.Text = Server.settings["host"].Value; 
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			Server.settings["host"].Value = this.txtHost.Text.Trim();
			SqueezeCenter.Settings.SaveSettings ("SqueezeCenter", Server.settings.Values);
		}
		
	}
}
