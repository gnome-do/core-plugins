
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
 
using Do.Universe;
using Do.Universe.Common;
using Do.Platform;

using Mono.Unix;


namespace Dropbox
{
	
	
	public class DropboxPuburlAction : Act
	{
		
		private string dropbox_dir = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/Dropbox";
		
		public override string Name {
			get { return "Public URL";  }
		}
		
		public override string Description {
			get { return "Get public url of a file in your dropbox."; }
		}
		
		public override string Icon {
			get { return "dropbox"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}
		
		public override bool SupportsItem (Item item) 
		{
			return (item as IFileItem).Path.StartsWith (dropbox_dir);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string path = (items.First () as IFileItem).Path;
			string url = GetPubUrl (path);
			
			if (url == "") {
				
				string msg = String.Format ("Sorry, the file \"{0}\" is not public", 
					UnixPath.GetFileName (path));
				
				Log<DropboxPuburlAction>.Debug (msg);
				Notification notification = new Notification ("Dropbox", msg, "dropbox");
				Services.Notifications.Notify (notification);
				
			} else {
				yield return new BookmarkItem (url, url);
			}
			
		}
		
		private string GetPubUrl (string path)
		{
			string url = "";
			
			try {
				ProcessStartInfo cmd = new ProcessStartInfo ();
				cmd.FileName = "dropbox";
				cmd.Arguments = String.Format ("puburl \"{0}\"", path); 
				cmd.UseShellExecute = false;
				cmd.RedirectStandardOutput = true;
				
				Process run = Process.Start (cmd);
				run.WaitForExit ();
				
				url = run.StandardOutput.ReadLine ();
				if (!url.StartsWith ("http")) { url = ""; }
				
			} catch {
			}
			
			return url;
		}

	}
}
