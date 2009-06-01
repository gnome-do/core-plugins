
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Do.Platform;

namespace Dropbox
{
	
	[System.ComponentModel.Category("File")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DropboxConfig : Gtk.Bin
	{
	
		static IPreferences prefs;

		protected virtual void OnOpenBtnClicked (object sender, System.EventArgs e)
		{
			Log.Debug ("clicked");
		}
			
		public DropboxConfig()
		{
			this.Build();
		}
		
		static DropboxConfig ()
		{
			prefs = Services.Preferences.Get<DropboxConfig> ();
		}
		
	}
}
