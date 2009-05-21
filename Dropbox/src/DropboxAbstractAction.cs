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
	
	
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}
		
		protected void Notify (string message)
		{
			Notification notification = new Notification ("Dropbox", message, "dropbox");
			Services.Notifications.Notify (notification);
		}
		
		protected bool MakeLink (string target, string link_name)
		{
			bool result = true;
			
			Services.Application.RunOnThread (() => {
				try {
					RunProcess ("ln", "-s " + 
						string.Format ("\"{0}\" \"{1}\"", target, link_name));
				} catch (Exception e) {
					Log.Error ("Could not link {0} to {1}: {2}", link_name, target, e.Message);
					Log.Debug (e.StackTrace);
					result = false;
				}
			});
			
			return result;
		}
		
		protected bool Unlink (string link_name)
		{
			bool result = true;
			
			Services.Application.RunOnThread (() => {
				try {
					RunProcess ("unlink", string.Format ("\"{0}\"", link_name));
				} catch (Exception e) {
					Log.Error ("Could not unlink {0}: {2}", link_name, e.Message);
					Log.Debug (e.StackTrace);
					result = false;
				}
			});
			
			return result;
		}
		
		protected string GetLink (string target)
		{
			return GetLink (target, Dropbox.DoSharedPath);
		}
		
		protected string GetLink (string target, string directory)
		{
			
			if (!Directory.Exists (directory))
				return null;
			
			UnixSymbolicLinkInfo link;
			UnixDirectoryInfo dir = new UnixDirectoryInfo (directory);
			
			foreach (UnixFileSystemInfo file in dir.GetFileSystemEntries ()) {
				if (!file.IsSymbolicLink) continue;
				
				link = new UnixSymbolicLinkInfo (file.FullName);
				if (link.HasContents && link.ContentsPath == target) 
					return file.FullName;
			}

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
		
		protected void RunProcess (string command, string args)
		{
			Process ps = Process.Start (command, args);
			ps.WaitForExit ();
		}
	}
}
