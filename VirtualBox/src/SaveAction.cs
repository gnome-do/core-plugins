//  SaveAction.cs
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using Mono.Unix;

using Do.Universe;

namespace VirtualBox
{
	//Save VM State
	public class SaveVM : Act
	{
		public SaveVM()
		{
		}

		public override string Name {
			get { return Catalog.GetString("Save Virtual Machine State"); }
		}

		public override string Description {
			get { return Catalog.GetString("Saves the state of the selected Virtual Machine"); }
		}

		public override string Icon {
			get { return "state_saved_16px.png@"+GetType().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] { typeof (VMItem) };
			}
		}

		public override bool SupportsItem (Item item) {
			//only allow "saving" of a machine if it is running
			VMItem v = item as VMItem;
			if ((v.Status == VMState.on) || (v.Status == VMState.headless))
				return true;
			else return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			foreach (Item i in items)
			{
				VMItem vm = (i as VMItem);

				VMThread thread = new VMThread(VMState.saved, ref vm);
				Thread t = new Thread (new ThreadStart(thread.DoAction));
				t.IsBackground = true;
				t.Start();
			}		
			return null;
		}
	}
}
