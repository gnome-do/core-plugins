//  StartAction.cs
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
using Mono.Addins;

using Do.Universe;

namespace VirtualBox
{
	//starts a virtual machine
	public class StartVM : Act
	{
		public StartVM ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString("Start Virtual Machine"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString("Starts the selected Virtual Machine"); }
		}

		public override string Icon {
			get { return "vm_start_32px.png@"+GetType().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (VMItem);
			}
		}

		public override bool SupportsItem (Item item) {
			//only allow "starting" of a machine if it is off or saved
			VMItem v = item as VMItem;
			if ((v.Status ==  VMState.off) || (v.Status == VMState.saved))
				return true;
			return false;
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			List<Item> DynItems = new List<Item> ();
			
			DynItems.Add(
			             new VMDynItm(AddinManager.CurrentLocalizer.GetString("Open in GUI"),
			                          AddinManager.CurrentLocalizer.GetString("Open in VirtualBox GUI"),
			                          "VirtualBox_64px.png@"+GetType().Assembly.FullName,
			                          VMState.on
			                          )
			             );
			DynItems.Add(
			             new VMDynItm(AddinManager.CurrentLocalizer.GetString("Start Headless"),
			                          AddinManager.CurrentLocalizer.GetString("Start in Headless mode"),
			                          "vrdp_16px.png@"+GetType().Assembly.FullName,
			                          VMState.headless
			                          )
			             );			
			try { return DynItems.ToArray(); }
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
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			foreach (Item i in items)
			{
				VMItem vm = (i as VMItem);
				VMState NewState;
				VMDynItm mod;
				
				if (modifierItems.Any ())
				{
					mod = modifierItems.First () as VMDynItm;
					NewState = mod.Mode;
				}
				else
					NewState = VMState.on;
				
				VMThread thread = new VMThread(NewState, ref vm);
				Thread t = new Thread (new ThreadStart(thread.DoAction));
				t.IsBackground = true;
				t.Start(); 
			}		
			yield break;
		}
	}
}
