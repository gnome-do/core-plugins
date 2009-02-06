// VMItem.cs 
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
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
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading;

using Do.Universe;

using Do.Platform;

namespace VirtualBox
{
	public enum VMState
	{
		saved,
		on,
		paused,
		off,
		limbo,
		headless
	}

	public class VMItem : Item
	{
		string name;
		string uuid;
		string ico_file;
		bool has_saved_states = false;
		VMState state;
		
		public VMItem (XmlAttributeCollection MachineAttrs) 
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			
			uuid = MachineAttrs["uuid"].Value.Replace ("{", "").Replace("}", "");
			//find machine specific xml file
			string MachineSource = MachineAttrs["src"].Value;
			if (!File.Exists (MachineSource))
				MachineSource = Path.Combine (Path.Combine (home, ".VirtualBox"), MachineSource);
			//find the OS type of the machine
			XmlDocument MachineDoc = new XmlDocument ();
			MachineDoc.Load (MachineSource);
			XmlNodeList MachineInfo = MachineDoc.GetElementsByTagName("Machine");
			name = MachineInfo[0].Attributes["name"].Value;
			try
			{
				string icon = IconMap.LookUp (MachineInfo[0].Attributes["OSType"].Value);
				ico_file = string.Format("os_{0}.png@{1}", icon, GetType ().Assembly.FullName);
			}
			catch //something went bad trying to assign an icon
			{
				Log<VMItem>.Warn ("Could not determine machine type for VM: {0}", name);
				ico_file = string.Format ("VirtualBox_64px.png@{0}", GetType().Assembly.FullName);
			}
			this.state = CurrentState;
			Log<VMItem>.Info ("VM: {0} indexed [State: {1} Uuid: {2}]", name, state, uuid);
		}
		
		public VMState CurrentState
		{
			get
			{
				VMState cur_state = default (VMState);
				//determine the state of thte VM
				ProcessStartInfo ps = new ProcessStartInfo ("VBoxManage", "showvminfo " + uuid);
				ps.UseShellExecute = false;
				ps.RedirectStandardOutput = true;
				using (Process p = Process.Start (ps))
				{
					p.WaitForExit ();
					string output = p.StandardOutput.ReadToEnd ();
					int s = output.IndexOf ("State:");
					int e = output.IndexOf ("\n", s);
					string outputState = output.Substring(s, e-s);
					//States: saved, running, paused, powered off
					if (outputState.Contains ("saved"))
						cur_state = VMState.saved;
					else if (outputState.Contains ("running"))
						cur_state = VMState.on;
					else if (outputState.Contains ("paused"))
						cur_state = VMState.paused;
					else if (outputState.Contains ("powered off"))
						cur_state = VMState.off;
					if (output.Contains ("Snapshots:"))
					    has_saved_states = true;
				}
				return cur_state;
			}
		}
		
		public override string Name { get { return name; } }
		public string Uuid { get { return uuid; } }
		public bool HasSavedStates { get { return has_saved_states; } }
		public VMState Status 
		{ 
			get { return state; }
			set { state = value; }
		}
		public override string Description { get { return "Virtual Machine: " + name; } }
		public override string Icon { get { return ico_file; } }
	}
}