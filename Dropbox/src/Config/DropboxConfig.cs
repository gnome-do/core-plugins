// 
// DropboxConfig.cs
// 
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this
// source distribution.
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
using System.IO;

using Mono.Addins;

using Gtk;

using Do.Platform;

namespace Dropbox
{
	
	[System.ComponentModel.Category("File")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DropboxConfig : Gtk.Bin
	{
		private static string home_path = Environment.GetFolderPath (Environment.SpecialFolder.Personal); 
		private static string base_path = System.IO.Path.Combine (home_path, "Dropbox");
				
		static IPreferences prefs;
			
		public DropboxConfig()
		{
			Build ();
			RefreshView ();
		}
		
		private void RefreshView ()
		{
			base_path_entry.Text = BasePath;
		}
		
		static DropboxConfig ()
		{
			prefs = Services.Preferences.Get<DropboxConfig> ();
		}
		
		public static string BasePath
		{
			get { return prefs.Get<string> ("BasePath", base_path);	}
			set { prefs.Set<string> ("BasePath", value); }
		}
		
		protected virtual void OnBasePathBtnClicked (object sender, System.EventArgs e)
		{
			FileChooserDialog chooser = new FileChooserDialog (
			    AddinManager.CurrentLocalizer.GetString ("Select location of Dropbox folder"),
				new Dialog (), FileChooserAction.SelectFolder,
			    Gtk.Stock.Cancel, ResponseType.Cancel,
			    Gtk.Stock.Open, ResponseType.Accept);
			
			chooser.SetCurrentFolder (BasePath);	
			if (chooser.Run () == (int) ResponseType.Accept) {
				BasePath = chooser.Filename;
				RefreshView ();
			}
			
			chooser.Destroy ();
		}
	}
}
