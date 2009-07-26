//  BaseServiceAction.cs 
// 
// Copyright Karol Będkowski 2008
// 
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

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace SystemServices {

	/// <summary>
	/// Base class for other actions
	/// </summary>
	public abstract class BaseServiceAction: Act {
	
		string name, description;
		ServiceActionType action;
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString (name); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString (description); }
		}

		public override string Icon {
			get { return SystemServices.GetIconForActionType (action); }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (Service); }
		}
 		
		public BaseServiceAction (string name, string description, ServiceActionType action) {
			this.name = AddinManager.CurrentLocalizer.GetString (name);
			this.description = AddinManager.CurrentLocalizer.GetString (description);
			this.action = action;			
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
			foreach (Service service in items) {
				string output = service.Perform (action);
				Services.Notifications.Notify (new Notification (service.Name, output, Icon));
			}
			yield break;
		}
	}

	/// <summary>
	/// Restart service.
	/// </summary>
	public class ServiceRestartAction: BaseServiceAction	{
		public ServiceRestartAction () :
			base ("Restart Service", "Restarts a system service.", ServiceActionType.Restart) {
		}
	}

	/// <summary>
	/// Start service.
	/// </summary>
	public class ServiceStartAction : BaseServiceAction	{
		public ServiceStartAction () :
			base ("Start Service", "Starts a system service.", ServiceActionType.Start) {
		}
	}

	/// <summary>
	/// Stop service.
	/// </summary>
	public class ServiceStopAction: BaseServiceAction {
		public ServiceStopAction () :
			base ("Stop Service", "Stops a system service.", ServiceActionType.Stop) {
		}
	}
}
