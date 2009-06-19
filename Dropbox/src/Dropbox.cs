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
		public readonly static string BasePath;
		public readonly static string PublicPath;
		public readonly static string DoSharedPath;
		
		private const string cli_path = "/usr/bin/dropbox";
		private const string db_url = "https://www.getdropbox.com/";
		
		static Dropbox ()
		{
			BasePath = DropboxConfig.BasePath;
			PublicPath = Path.Combine (BasePath, "Public");
			DoSharedPath = Path.Combine (PublicPath, "Files Shared from GNOME Do");
		}
		
		public static bool IsRunning {
			get {
				return !Exec ("status").StartsWith ("Dropbox isn't running!");
			}
		}
		
		public static bool HasCli {
			get {
				return File.Exists (cli_path);
			}
		}
		
		public static void Start ()
		{
			Exec ("start -i");
		}
		
		public static void Stop ()
		{
			Exec ("stop");
		}
		
		public static string GetPubUrl (string path)
		{
			string url = Exec (String.Format ("puburl \"{0}\"", path));
			if (!url.StartsWith ("http")) { url = null; }
			
			return url;
		}
		
		public static string GetWebUrl ()
		{
			return db_url + "home#";
		}
		
		public static string GetWebUrl (string path)
		{
			return GetWebUrl () + path.Substring (BasePath.Length);
		}
		
		public static string GetRevisionsUrl (string path)
		{
			return db_url + "revisions" + path.Substring (BasePath.Length);
		}
		
		private static string Exec (string args) 
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
