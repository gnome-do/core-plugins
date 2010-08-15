//  EmapthyStatusItem.cs
//  
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//  
//  Copyright (c) 2010 
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

using Telepathy;

namespace EmpathyPlugin
{

	public class EmpathyStatusItem : Item
	{

		public EmpathyStatusItem (ConnectionPresenceType status)
		{
			this.Status = status;
		}
				
		public EmpathyStatusItem (ConnectionPresenceType status, string message)
		{
			this.Status = status;
		}
		
		public override string Name {
			get {
				return Description;
			}
		}
		
		public override string Description {
			get { 				
				switch (this.Status) {
				case ConnectionPresenceType.Offline: return AddinManager.CurrentLocalizer.GetString ("Offline");
				case ConnectionPresenceType.Available: return AddinManager.CurrentLocalizer.GetString ("Available");
				case ConnectionPresenceType.Away: return AddinManager.CurrentLocalizer.GetString ("Away");
				case ConnectionPresenceType.Hidden: return AddinManager.CurrentLocalizer.GetString ("Invisible");
				case ConnectionPresenceType.Busy: return AddinManager.CurrentLocalizer.GetString ("Busy");
				default: return AddinManager.CurrentLocalizer.GetString ("Unknown Status");
				}
			}
		}
		
		public override string Icon { 
			get  { 
				switch (this.Status) {
				case ConnectionPresenceType.Offline: return "user-offline";
				case ConnectionPresenceType.Available: return "user-available";
				case ConnectionPresenceType.Away: return "user-away";
				case ConnectionPresenceType.Hidden: return "user-invisible";
				case ConnectionPresenceType.Busy: return "user-busy";
				default: return "empathy";
				}
			}
		}
		
		public ConnectionPresenceType Status { get; private set; }
	}
}
