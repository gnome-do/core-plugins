
using System;
using System.IO;

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
			    "Select location of Dropbox folder",
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
