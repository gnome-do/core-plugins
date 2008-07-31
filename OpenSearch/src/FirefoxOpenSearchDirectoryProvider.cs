//  FirefoxOpenSearchDirectoryProvider.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;

using Do;

namespace OpenSearch
{
	/// <summary>
	/// Provides a list of OpenSearch plugin directories
	/// that are associated with Firefox. The OpenSearch plugins
	/// are just XML files within these directories. There are two
	/// key places that the xml files are stored: the default plugins 
	/// are stored in Firefox's lib directory, and user installed plugins
	/// are stored in under the user's Firefox profile.
	/// </summary>
	public class FirefoxOpenSearchDirectoryProvider
	{
		private List<string> openSearchPluginDirectories;
		
		/// <summary>
		/// Initialize the provider with the default directories.
		/// </summary>
		public FirefoxOpenSearchDirectoryProvider ()
		{		
			openSearchPluginDirectories = new List<string> ();			
			
			string firefoxProfilePath = GetProfileSearchPluginsPath ();
			if (firefoxProfilePath != null) {
				openSearchPluginDirectories.Add (firefoxProfilePath);
			}
			
			string firefoxLibPath = GetLibSearchPluginsPath ();
			if (firefoxLibPath != null) {
				openSearchPluginDirectories.Add (firefoxLibPath);	
			}
		}
		
		/// <value>
		/// A list of Firefox OpenSearch plugin directories.
		/// </value>
		public List<string> OpenSearchPluginDirectories
		{
			get { return openSearchPluginDirectories; }
		}
		
		/// <summary>
		/// Retrieves the LIB plugin directory, which is where the default
		/// OpenSearch plugins are installed.
		/// </summary>
		/// <returns>
		/// The full path to the Firefox searchplugins LIB directory.
		/// </returns>
		private string GetLibSearchPluginsPath ()
		{
			try {
				// The lib directory is stored in the startup script, which is
				// located at /usr/bin/firefox. At some point, we'll want to account 
				// for installing in different directories. We could certainly shell
				// out and call which or something...
				string beginLibDir = "LIBDIR=";
				string binFile = "/usr/bin/firefox";
				string line, libDir;		
				
				libDir = null;
				
				using (StreamReader r = File.OpenText (binFile)) {
					while (null != (line = r.ReadLine ())) {
						if (line.StartsWith (beginLibDir)) {
							line = line.Trim ();
							line = line.Substring (beginLibDir.Length);
							libDir = line;
						}
					}
				}				
					
				if (libDir != null) {
					string path = Path.Combine (libDir, "searchplugins");		
					return path;
				}
			}
			catch {
				// just return null if we've got problems
			}
				
			return null;
		}
		
		/// <summary>
		/// Retrieves the profile plugin directory, which is where the 
		/// user installed OpenSearch plugins are located.
		/// </summary>
		/// <returns>
		/// The full path to the Firefox searchplugins profile directory.
		/// </returns>
		private string GetProfileSearchPluginsPath ()
		{
			try {
				string beginProfileName = "Path=";
				string beginDefaultProfile = "Default=1";
				
				string line, profile, profilePath;
				
				profile = null;				
				
				profilePath = Path.Combine (Paths.UserHome, ".mozilla/firefox/profiles.ini");
				using (StreamReader r = File.OpenText (profilePath)) {
					while (null != (line = r.ReadLine ())) {
						if (line.StartsWith (beginDefaultProfile)) break;
						if (line.StartsWith (beginProfileName)) {
							line = line.Trim ();
							line = line.Substring (beginProfileName.Length);
							profile = line;
						}
					}
				}
							
				if(profile != null) {
					string path = Path.Combine (Paths.UserHome, ".mozilla/firefox");
					path = Path.Combine (path, profile);
					path = Path.Combine (path, "searchplugins");
				
					return path;
				}
			}
			catch {
				// just return null if we've got problems
			}
				
			return null;
		}
	}
}
