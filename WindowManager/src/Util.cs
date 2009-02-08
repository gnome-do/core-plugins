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
		static IEnumerable<string> BadPrefixes {
			get {
				yield return "gksu";
				yield return "sudo";
				yield return "java";
				yield return "mono";
				yield return "ruby";
				yield return "padsp";
				yield return "aoss";
				yield return "python";
				yield return "python2.4";
				yield return "python2.5";
			}
		}
		
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
			List<Application> apps = new List<Application> ();
			if (string.IsNullOrEmpty (exec))
				return apps;
			
			exec = ProcessExecString (exec);
			
			Application out_app = null;
			foreach (string dir in Directory.GetDirectories ("/proc")) {
				int pid;
				out_app = null;
				try { pid = Convert.ToInt32 (Path.GetFileName (dir)); } 
				catch { continue; }
				
				string exec_line = CmdLineForPid (pid);
				if (string.IsNullOrEmpty (exec_line))
					continue;

				exec_line = ProcessExecString (exec_line);

				if (exec_line != null && exec_line.Contains (exec)) {
					foreach (Application app in GetApplications ()) {
						if (app == null)
							continue;
						
						if (app.Pid == pid || app.Windows.Any (w => w.Pid == pid)) {
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
		
		public static string ProcessExecString (string exec)
		{
			string [] parts = exec.Split (' ');
			for (int i = 0; i < parts.Length; i++) {
				if (parts [i].StartsWith ("-"))
					continue;
				
				if (parts [i].Contains ("/"))
					parts [i] = parts [i].Split ('/').Last ();
				
				foreach (string prefix in BadPrefixes) {
					if (parts [i] == prefix)
						parts [i] = null;
				}
				
				if (!string.IsNullOrEmpty (parts [i])) {
					return parts [i].ToLower ();
				}
			}
			return null;
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
			string cmdline = null;
			
			try {
				string procPath = new [] { "/proc", pid.ToString (), "cmdline" }.Aggregate (Path.Combine);
				reader = new StreamReader (procPath);
				cmdline = reader.ReadLine ().Replace (Convert.ToChar (0x0), ' ');
				reader.Close ();
				reader.Dispose ();
			} catch { }
			
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
