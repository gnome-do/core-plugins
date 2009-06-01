
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Do.Platform;

namespace Dropbox
{
	
	public partial class DropboxConfig : Gtk.Bin
	{
	
		static IPreferences prefs;
		
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
