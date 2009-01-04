// ClawsActionBase.cs 
// User: Karol Będkowski at 22:14 2008-10-14

// Copyright Karol Będkowski 2008

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

using System;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Claws
{
	
	public abstract class ClawsActionBase: Act {
		const string clawsMail =  "Claws Mail";		
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IApplicationItem); }
		}

		protected virtual string Command {
			get { return string.Empty; }
		}

		public override bool SupportsItem (Item item)
		{
			IApplicationItem applicationItem = item as IApplicationItem;
			if (item != null) {
				return clawsMail.Equals (applicationItem.Name);
			}
			return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			try {
				System.Diagnostics.Process.Start (this.Command);
			} catch (Exception err) {
				Log.Error("ClawsActionBase.Perform command='{0}', error={1}", this.Command, err.Message);
				Log.Debug("ClawsActionBase.Perform stack: {0}", err.StackTrace);
			}
			yield break;
		}
	}
}
