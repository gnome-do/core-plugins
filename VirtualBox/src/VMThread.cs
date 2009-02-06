// VMThread.cs
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
using System.Collections.Generic;
using System.Diagnostics;


using Do.Platform;

namespace VirtualBox
{
	public class VMThread
	{
		private string op1;
		private string op2;
		private VMState NewState;
		private VMItem vm;
		
		//complicated constructor if you want to supply everything
		public VMThread(string o1, string o2, VMState ns, ref VMItem v)
		{
			this.op1 = o1;
			this.op2 = o2;
			this.NewState = ns;
			this.vm = v;
		}
		
		//empty constructor, only for complex actions
		public VMThread(ref VMItem v)
		{
			this.vm = v;	
		}
		
		//simple constructor, to make things easy
		public VMThread(VMState ns, ref VMItem v)
		{
			NewState = ns;
			vm = v;
			op1 = "VBoxManage";
			op2 = "controlvm " + v.Uuid + " ";
			switch (NewState) //do some action, depending on what the desired NewState is
			{
			case VMState.saved:
				op2 += "savestate";
				break;
			case VMState.off:
				op2 += "poweroff";
				break;
			case VMState.on:
				if (v.Status == VMState.paused)
					op2 += "resume";
				else if ((v.Status == VMState.off) || (v.Status == VMState.saved))
					op2 = "startvm " + v.Uuid + " -type gui";
				break;
			case VMState.paused:
				op2 += "pause";
				break;
			case VMState.headless:
				if (v.Status == VMState.paused)
					op2 += "resume";
				else if ((v.Status == VMState.off) || (v.Status == VMState.saved))
					op2 = "startvm " + v.Uuid + " -type vrdp";
				break;
			}	
		}
		
		public void DoAction()
		{
			try 
			{
				if (!CheckState())
				{
					Log.Error("State mismatch for {0}.", vm.Name);
					return;
				}
				vm.Status = VMState.limbo;
				ProcessStartInfo ps = new ProcessStartInfo (op1, op2);
				ps.UseShellExecute = false;
				ps.RedirectStandardOutput = true;
				using (Process p = Process.Start (ps)) {
					Log<VMThread>.Info("Execution thread for {0} started.", vm.Name);
					p.WaitForExit ();
					if (p.HasExited)
					{
						vm.Status = NewState;
						Log<VMThread>.Info("Execution thread for {0} finished.", vm.Name);
					}
				}	
			}
			catch 
			{
				Log<VMThread>.Fatal("Something horrible happened to {0}.", vm.Name);
			}
		}
		
		public void DoShutdownRestoreAction()
		{
			try
			{
				if (!CheckState())
				{
					Log<VMThread>.Error("State mismatch for {0}", vm.Name);
					return;
				}
				vm.Status = VMState.limbo;
				List<ProcessStartInfo> Processes = new List<ProcessStartInfo>();
				Processes.Add( new ProcessStartInfo ("VBoxManage", "controlvm " + vm.Uuid + " poweroff") );
				Processes.Add( new ProcessStartInfo ("sleep", "2") );
				Processes.Add( new ProcessStartInfo ("VBoxManage", "snapshot " + vm.Uuid + " discardcurrent -state") );
				
				Log<VMThread>.Info("Execution thread for {0} started.", vm.Name);
				foreach (ProcessStartInfo ps in Processes)
				{
					ps.UseShellExecute = false;
					ps.RedirectStandardOutput = true;
					using (Process p = Process.Start (ps))
						p.WaitForExit ();
				}
				Log<VMThread>.Info("Execution thread for {0} finished.", vm.Name);
				vm.Status = VMState.off;
			}
			catch 
			{
				Log<VMThread>.Fatal("Something horrible happened to {0}.", vm.Name);
			}
		}
		
		public bool CheckState()
		{
			return (vm.Status == vm.CurrentState) ? true : false;
		}
	}
}
