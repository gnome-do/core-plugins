/* CompareConfig.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;


using GConf;

namespace FilePlugin
{
	public partial class CompareConfig : Gtk.Bin
	{
		private static IPreferences prefs;
		private static GConf.Client gconf;
		
		private Dictionary<string, bool> known_diff_tools;
		
		public CompareConfig ()
		{
			Build();
			
			known_diff_tools = new Dictionary<string,bool> ();
			BuildDiffDict ();
			
			string [] keys = new string [known_diff_tools.Keys.Count + 1];
			int i = 0;
			foreach (string key in known_diff_tools.Keys) {
				if (DiffExistsInPath (key)) {
					keys [i] = key;
					diff_tool_combo.AppendText (key);
					i++;
				}
			}
			//If we have a custom command in gconf we should append it to the list
			if ((!(known_diff_tools.ContainsKey (DiffTool))) || String.IsNullOrEmpty (DiffTool)) {
				diff_tool_combo.AppendText (DiffTool);
				keys [i] = DiffTool;
			}
				
			//Set the UI values to their current gconf analogs
			//if we don't have one set, use the first from the list
			//worse case this is still just blank
			int diff_index = Array.IndexOf<string> (keys, DiffTool);
			if (diff_index == -1)
				diff_tool_combo.Active = 0;
			else
				diff_tool_combo.Active = Array.IndexOf<string> (keys, DiffTool);
				
			run_term_chk.Active = RunInTerminal;
		}
		
		static CompareConfig ()
		{
			gconf = new GConf.Client ();
			prefs = Do.Addins.Util.GetPreferences ("FilesAndFolders");
		}
		
		public static string DiffTool {
			get { return prefs ["DiffTool"]; }
			set { prefs ["DiffTool"] = value; }
		}
		
		public static string Terminal {
			get {
				const string gconfBase = "/desktop/gnome/applications/terminal/";
				string termCommand = "";
				try {
					termCommand = gconf.Get (gconfBase + "exec") as string;
					termCommand += " " + gconf.Get (gconfBase + "exec_arg") as string;
				} catch (GConf.NoSuchKeyException) {
					//we should all have an xterm...
					termCommand = "xterm -e";
				}
				return termCommand;
			}
		}
		
		public static bool RunInTerminal {
			get { return prefs.Get<bool> ("RunInTerminal", false); }
			set { prefs.Set<bool> ("RunInTerminal", value); }
		}
		
		private void BuildDiffDict ()
		{
			known_diff_tools.Add ("fldiff", false);
			known_diff_tools.Add ("imediff2", true);
			known_diff_tools.Add ("kompare", false);
			known_diff_tools.Add ("kdiff3", false);
			known_diff_tools.Add ("meld", false);
			known_diff_tools.Add ("tkdiff", false);
			known_diff_tools.Add ("vimdiff", true);
			known_diff_tools.Add ("xxdiff", false);
		}
		
		private bool DiffExistsInPath (string command)
		{
			string path;
			path = System.Environment.GetEnvironmentVariable ("PATH");
			if (path != null) {
				foreach (string part in path.Split (':')) {
					if (System.IO.File.Exists (System.IO.Path.Combine (part, command)))
						return true;
				}
			}
			return false;
		}

		protected virtual void OnRunTermChkClicked (object sender, System.EventArgs e)
		{
			RunInTerminal = run_term_chk.Active;
		}

		protected virtual void OnDiffToolComboChanged (object sender, System.EventArgs e)
		{
			DiffTool = diff_tool_combo.ActiveText;
			bool runInTerm;
			if (known_diff_tools.TryGetValue (DiffTool, out runInTerm)) {
				RunInTerminal = runInTerm;
				run_term_chk.Active = runInTerm;
			} else {
				RunInTerminal = false;
				run_term_chk.Active = false;
			}
		}
	}
}
