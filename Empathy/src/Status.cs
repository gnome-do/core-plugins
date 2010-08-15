//  Status.cs
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
using System.Collections.Generic;
using Telepathy;

namespace EmpathyPlugin
{
	
	public enum BaseStatus 
	{
		available,
		away,
		brb,
		busy,
		dnd,
		xa,
		hidden,
		offline,
		unknown,
		error
	}
	
	public class EmpathyStatus
	{

		public BaseStatus identifier {get; private set;}
		public string message {get; private set;}
		
		public EmpathyStatus(BaseStatus identifier, string message)
		{
			this.identifier = identifier;
			this.message = message;
		}
	
		public static List<BaseStatus> GetStatusList(ConnectionPresenceType presenceType) 
		{
			List<BaseStatus> res = new List<BaseStatus>();
				
			switch (presenceType) {
				case ConnectionPresenceType.Available:
					res.Add(BaseStatus.available);
						break;
				case ConnectionPresenceType.Away:
					res.Add(BaseStatus.away);
					res.Add(BaseStatus.brb);
						break;
				case ConnectionPresenceType.Busy:
					res.Add(BaseStatus.busy);
					res.Add(BaseStatus.dnd);
						break;
				case ConnectionPresenceType.Error:
					res.Add(BaseStatus.error);
						break;
				case ConnectionPresenceType.ExtendedAway:
					res.Add(BaseStatus.xa);
						break;
				case ConnectionPresenceType.Hidden:
					res.Add(BaseStatus.hidden);
						break;
				case ConnectionPresenceType.Offline:
					res.Add(BaseStatus.offline);
						break;
				case ConnectionPresenceType.Unknown:
					res.Add(BaseStatus.unknown);
						break;
				default:
				break;
			}
				
			return res;
		}

		public static ConnectionPresenceType GetPresence(string pres)
		{
			switch (pres) {
				case "available":
					return ConnectionPresenceType.Available;
				case "away":
					return ConnectionPresenceType.Away;
				case "busy":
					return ConnectionPresenceType.Busy;
				case "error":
					return ConnectionPresenceType.Error;
				case "extended-away":
					return ConnectionPresenceType.ExtendedAway;
				case "hidden":
					return ConnectionPresenceType.Hidden;
				case "offline":
					return ConnectionPresenceType.Offline;
				case "unknown":
					return ConnectionPresenceType.Unknown;
				default:
					return ConnectionPresenceType.Unknown;
			}
		}
	}
}

