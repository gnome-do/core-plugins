// Util.cs
// 
// Copyright (C) 2009 GNOME Do
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Wnck;

namespace WindowManager
{
	
	
	public static class Util
	{
		/// <summary>
		/// Returns a list of applications that match an exec string
		/// </summary>
		/// <param name="exec">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public static List<Application> GetApplicationList (string exec)
		{
			exec = exec.Split (' ')[0];
			List<Application> apps = new List<Application> ();
			Application out_app = null;
			foreach (string dir in Directory.GetDirectories ("/proc")) {
				int pid;
				out_app = null;
				try { pid = Convert.ToInt32 (dir.Substring (6)); } 
				catch { continue; }
				
				string exec_line = CmdLineForPid (pid);
				if (string.IsNullOrEmpty (exec_line))
					continue;
				
				if (exec_line.Contains (exec)) {
					foreach (Application app in GetApplications ()) {
						if (app.Pid == pid) {
							if (app.Windows.Select (win => !win.IsSkipTasklist).Any ())
								out_app = app;
							break;
						}
					}
				}
				
				if (out_app != null)
					apps.Add (out_app);
			}
			return apps;
		}
		
		/// <summary>
		/// Gets the command line excec string for a PID
		/// </summary>
		/// <param name="pid">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public static string CmdLineForPid (int pid)
		{
			StreamReader reader;
			string cmdline;
			try {
				string procPath = new [] { "/proc", pid.ToString (), "cmdline" }.Aggregate (Path.Combine);
				reader = new StreamReader (procPath);
				cmdline = reader.ReadLine ();
				reader.Close ();
				reader.Dispose ();
			} catch { return null; }
			
			return cmdline;
		}
		
		/// <summary>
		/// Returns a list of all applications on the default screen
		/// </summary>
		/// <returns>
		/// A <see cref="Application"/> array
		/// </returns>
		public static Application[] GetApplications ()
		{
			List<Application> apps = new List<Application> ();
			foreach (Window w in Wnck.Screen.Default.Windows) {
				if (!apps.Contains (w.Application))
					apps.Add (w.Application);
			}
			return apps.ToArray ();
		}
	}
}
