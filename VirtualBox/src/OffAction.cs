//  OffAction.cs
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
//

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using Mono.Unix;

using Do.Universe;

namespace VirtualBox
{
	//Power Off VM State
	public class PowerOffVM : Act
	{
		public PowerOffVM()
		{
		}

		public override string Name {
			get { return Catalog.GetString("Power Off Virtual Machine"); }
		}

		public override string Description {
			get { return Catalog.GetString("Powers off the selected Virtual Machine"); }
		}

		public override string Icon {
			get { return "vm_delete_32px.png@"+GetType().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (VMItem) };
			}
		}

		public override bool SupportsItem (Item item) {
			//only allow "poweringoff" of a machine if it is running
			VMItem v = item as VMItem;
			if ((v.Status == VMState.on) || (v.Status == VMState.headless))
				return true;
			else return false;
		}
		
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{			
			List<Item> DynItems = new List<Item> ();
			VMItem vm = (item as VMItem);
			
			DynItems.Add(
			             new VMDynItm(Catalog.GetString("Discard State"),
			                          Catalog.GetString("Restore VM state to current Snapshot"),
			                          "vm_discard_32px.png@"+GetType().Assembly.FullName,
			                          VMState.off
			                          )
			             );			
			try 
			{ 
				if (vm.HasSavedStates)
					return DynItems.ToArray(); 
				else return null;
			}
			catch { return null; }

		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get {
				return new Type[] { typeof (VMDynItm) };
			}
		}
		
		public override bool ModifierItemsOptional
		{
			get { return true; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (Item i in items)
			{
				VMItem vm = (i as VMItem);
				VMThread thread;
				Thread t;
				if (modItems.Any ())
				{
					thread = new VMThread(ref vm);
					t = new Thread (new ThreadStart(thread.DoShutdownRestoreAction));
				}
				else 
				{
					thread = new VMThread(VMState.off, ref vm);
					t = new Thread (new ThreadStart(thread.DoAction));
				}
				t.IsBackground = true;
				t.Start();
			}
			return null;
		}
	}
}
