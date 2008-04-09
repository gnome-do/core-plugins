// WindowListItems.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Do.Universe;
using Wnck;

namespace WindowManager
{
	public class WindowListItems
	{
		private static Dictionary<string, List<Window>> windowList;
		private static Wnck.Screen scrn;
		private static object listLock = new object ();
		private static Window currentWindow;
		
		public static Window CurrentWindow {
			get {
				return currentWindow;
			}
		}
		
		static WindowListItems ()
		{
			Gtk.Application.Init (); //why do i HAVE to do this?
			
			windowList = new Dictionary<string,List<Window>> ();
			
			scrn = Screen.Default;
			scrn.WindowOpened        += OnWindowOpened;
			scrn.WindowClosed        += OnWindowClosed;
			scrn.ActiveWindowChanged += OnActiveWindowChanged;
		}
			
		public static void GetList (out Dictionary<string, List<Window>> winList)
		{
			winList = new Dictionary<string,List<Window>> ();
			
			foreach (KeyValuePair<string, List<Window>> kvp in windowList) {
				winList.Add (kvp.Key, kvp.Value);
			}
		}
		
		public static List<Window> GetApplication (string application)
		{
			List<Window> windows;
			
			windowList.TryGetValue (application, out windows);
			
			return windows;
		}
		
		private static void OnActiveWindowChanged (object o, ActiveWindowChangedArgs args)
		{
			try {
				if (!scrn.ActiveWindow.IsSkipTasklist)
					currentWindow = scrn.ActiveWindow;
			}
			catch {
				
			}
		}
		
		private static void OnWindowOpened (object o, WindowOpenedArgs args)
		{
			if (args.Window.IsSkipTasklist) return;

			// there is a reason, but its stupid and unimportant.  Wnck reports a window
			// open/close event if things minimize/restore really fast.  The window also gets
			// temporarily dropped from the list for some reason.  To account for this, a 2 second
			// lag on our updates lets Wnck unfuck itself.
			GLib.Timeout.Add (2000, delegate {
				Thread updateRunner = new Thread (new ThreadStart (UpdateList));
				
				updateRunner.Start ();
				return false;
			});
		}
		
		private static void OnWindowClosed (object o, WindowClosedArgs args)
		{
			if (args.Window.IsSkipTasklist) return;

			// there is a reason, see above
			GLib.Timeout.Add (2000, delegate {
				Thread updateRunner = new Thread (new ThreadStart (UpdateList));
				
				updateRunner.Start ();
				return false;
			});
		}
		
		private static void UpdateList ()
		{
			if (!Monitor.TryEnter (listLock))
				return;
			
			windowList.Clear ();
			
			ProcessStartInfo st = new ProcessStartInfo ("ps");
			st.RedirectStandardOutput = true;
			st.UseShellExecute = false;
			
			Process process;
			string processName;
			
			foreach (Window w in scrn.WindowsStacked) {
				if (w.Pid == 0) continue;
				
				if (w.IsSkipTasklist) continue;
				
				st.Arguments = "c -o cmd --no-headers " + w.Pid;
				
				process = Process.Start (st);
				
				process.WaitForExit ();
				if (process.ExitCode != 0) continue;
				
				processName = process.StandardOutput.ReadLine ();
				if (windowList.ContainsKey (processName)) {
					List<Window> winList;
					windowList.TryGetValue (processName, out winList);
					winList.Add (w);
				} else {
					List<Window> winList = new List<Window> ();
					winList.Add (w);
					windowList.Add (processName, winList);
				}
			}
			
			ListUpdated ();
			Monitor.Exit (listLock);
		}
		
		public static event ListUpdatedDelegate ListUpdated;
		
		public delegate void ListUpdatedDelegate ();
	}
}
