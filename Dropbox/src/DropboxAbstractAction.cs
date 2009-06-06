// 
// DropboxAbstractAction.cs
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
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Dropbox
{
	
	
	public abstract class DropboxAbstractAction : Act
	{
		
		protected static Random rand = new Random ();
		protected string pub_url_title = Catalog.GetString ("URL of your shared file");
	
	
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}
		
		protected void Notify (string message)
		{
			Notification notification = new Notification (Catalog.GetString ("Dropbox"), message, "dropbox");
			Services.Notifications.Notify (notification);
		}
		
		protected string ReadLink (string link_name)
		{
			return RunProcess ("readlink", string.Format ("\"{0}\"", link_name));
		}
		
		protected bool MakeLink (string target, string link_name)
		{
			string result = RunProcess ("ln", 
				string.Format ("-s \"{0}\" \"{1}\"", target, link_name));
			
			return result != null;
	
		}
		
		protected bool Unlink (string link_name)
		{
			string result = RunProcess ("unlink", 
				string.Format ("\"{0}\"", link_name));

			return result != null;
		}
		
		protected string GetLink (string target)
		{
			return GetLink (target, Dropbox.DoSharedPath);
		}
		
		protected string GetLink (string target, string directory)
		{
			
			if (!Directory.Exists (directory))
				return null;
			
			foreach (string file in Directory.GetFiles (directory)) 
				if (ReadLink (file) == target) 
					return file;

			return null;
		}
		
		protected bool HasLink (string target)
		{
			return GetLink (target) != null;
		}
		
		protected bool HasLink (string target, string directory)
		{
			return GetLink (target, directory) != null;
		}
		
		protected string GetPath (Item item)
		{
			if (item is IFileItem)
				return GetPath (item as IFileItem);
			if (item is ITextItem)
				return GetPath (item as ITextItem);
			throw new Exception ("Inappropriate Item type.");
		}

		protected string GetPath (IFileItem item)
		{
			return item.Path;
		}

		protected string GetPath (ITextItem item)
		{
			return item.Text.Replace ("~", 
				Environment.GetFolderPath (Environment.SpecialFolder.Personal));
		}
		
		protected string RunProcess (string command, string args) 
		{
			try {
				ProcessStartInfo cmd = new ProcessStartInfo ();
				cmd.FileName = command;
				cmd.Arguments = args; 
				cmd.UseShellExecute = false;
				cmd.RedirectStandardOutput = true;
				
				Process ps = Process.Start (cmd);
				ps.WaitForExit ();
				
				string stdout = ps.StandardOutput.ReadLine ();
				
				if (stdout == null) return string.Empty;
				return stdout;
				
			} catch (Exception e) {
				Log<Dropbox>.Error ("Error running {0} {1} : {2}", command, args, e.Message);
				Log<Dropbox>.Debug (e.StackTrace);
				
				return null;
			}
		}
	}
}
