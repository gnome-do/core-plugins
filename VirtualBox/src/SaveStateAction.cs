//  SaveStateAction.cs
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
	//Saves the current state as a snapshot
	public class TakeSnapshot : Act
	{
		public TakeSnapshot ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString("Take Snapshot"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString("Save the current state as a Snapshot"); }
		}

		public override string Icon {
			get { return "take_snapshot_22px.png@"+GetType().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (VMItem) };
			}
		}

		public override bool SupportsItem (Item item) {
			//only allow "taking shapshot" of a machine if it is not in limbo
			VMItem v = item as VMItem;
			if (v.Status != VMState.limbo)
				return true;
			else return false;
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			yield break;
		}

		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get {
				yield return typeof (ITextItem);
			}
		}

		public override bool ModifierItemsOptional
		{
			get { return true; }
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (!(string.IsNullOrEmpty((modItem as ITextItem).Text)))
				return true;
			else 
				return false;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			VMItem vm = (items.First () as VMItem);
			
			string SnapshotName;
			if (modItems.Any ())
				SnapshotName = (modItems.First () as ITextItem).Text;
			else
				SnapshotName = AddinManager.CurrentLocalizer.GetString("Snapshot (") + DateTime.Now + ")";
			
			VMThread thread = new VMThread("VBoxManage", "snapshot " + vm.Uuid + " take '" + SnapshotName + "'", vm.Status, ref vm);
			Thread t = new Thread (new ThreadStart(thread.DoAction));
			t.IsBackground = true;
			t.Start(); 
			yield break;
		}
	}
}
