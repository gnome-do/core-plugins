// 
// Dropbox.cs
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
using System.Diagnostics;

using Do.Platform;

namespace Dropbox
{
	
	
	public class Dropbox
	{
		public string BasePath;
		public string PublicPath;
		public string DoSharedPath;
		
		private string cli_path = "/usr/bin/dropbox";
		private string db_url = "https://www.getdropbox.com/";
		
		public Dropbox ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			
			BasePath = Path.Combine (home, "Dropbox");
			PublicPath = Path.Combine (BasePath, "Public");
			DoSharedPath = Path.Combine (PublicPath, "Do Shared Files");
		}
		
		public bool IsRunning {
			get {
				return !Exec ("status").StartsWith ("Dropbox isn't running!");
			}
		}
		
		public bool HasCli {
			get {
				return File.Exists (cli_path);
			}
		}
		
		public void Start ()
		{
			Exec ("start -i");
		}
		
		public void Stop ()
		{
			Exec ("stop");
		}
		
		public string GetPubUrl (string path)
		{
			string url = Exec (String.Format ("puburl \"{0}\"", path));
			if (!url.StartsWith ("http")) { url = null; }
			
			return url;
		}
		
		public string GetWebUrl ()
		{
			return db_url + "home#";
		}
		
		public string GetWebUrl (string path)
		{
			return GetWebUrl () + path.Substring (BasePath.Length);
		}
		
		public string GetRevisionsUrl (string path)
		{
			return db_url + "revisions" + path.Substring (BasePath.Length);
		}
		
		private string Exec (string args) 
		{
			string stdout = "";
			
			try {
				ProcessStartInfo cmd = new ProcessStartInfo ();
				cmd.FileName = cli_path;
				cmd.Arguments = args; 
				cmd.UseShellExecute = false;
				cmd.RedirectStandardOutput = true;
				
				Process run = Process.Start (cmd);
				run.WaitForExit ();
				
				stdout = run.StandardOutput.ReadLine ();
				
			} catch (Exception e) {
				Log<Dropbox>.Error ("Error running dropbox {0}: {1}", args, e.Message);
				Log<Dropbox>.Debug (e.StackTrace);
			}
			
			return stdout;
		}
			
	}
}
