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
	
	public abstract class ClawsSimpleActionBase: Act {
		const string clawsMail =  "Claws Mail";

		const string clawsMailCommand = "claws-mail";
		

		protected virtual string CommandParams {
			get { return string.Empty; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IApplicationItem); }
		}

		
		public override bool SupportsItem (Item item)
		{
			IApplicationItem applicationItem = item as IApplicationItem;
			if (item != null) { // connect to claws mail program
				return clawsMail.Equals (applicationItem.Name);
			}
			return false;
		}

		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			try {
				System.Diagnostics.Process.Start (clawsMailCommand, CommandParams);
			} catch (Exception err) {
				Log.Error("ClawsActionBase.Perform commandParams='{0}', error={1}", CommandParams, err.Message);
				Log.Debug("ClawsActionBase.Perform stack: {0}", err.StackTrace);
			}

			yield break;
		}
	}

	
	/// <summary>
	/// Change Claws mode to offline.
	/// </summary>
	public class ClawsActionOffline : ClawsSimpleActionBase {
		
		protected override string CommandParams {
			get { return "--offline"; }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Work Offline"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Switch ClawsMail to offline mode"); }
		}
		
		public override string Icon {
			get { return "stock_stop"; }
		}
	}

	
	/// <summary>
	/// Change Claws mode to online.
	/// </summary>
	public class ClawsActionOnline : ClawsSimpleActionBase {
	    
		protected override string CommandParams {
			get { return "--online"; }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Work Online"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Switch ClawsMail to online mode"); }
		}
		
		public override string Icon {
			get { return "stock_right"; }
		}
	} 

	
	/// <summary>
	/// Close Claws Mail action.
	/// </summary>
	public class ClawsActionQuit : ClawsSimpleActionBase {
		
		protected override string CommandParams {
			get { return "--quit"; }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Quit Claws Mail"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Close program"); }
		}
		
		public override string Icon {
			get { return "exit"; }
		}
	}

	
	/// <summary>
	/// Get all mail.
	/// </summary>
	public class ClawsActionReceiveAll : ClawsSimpleActionBase {
		
		protected override string CommandParams {
			get { return "--receive-all"; }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Receive all"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Receive Mail on all Accounts"); }
		}
		
		public override string Icon {
			get { return "mail-send-receive"; }
		}
	}
}
