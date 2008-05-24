// PidginStatusTypeItem.cs
// 
// Copyright (C) 2008 [Alex Launi]
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
//

using System;
using Do.Universe;

namespace Do.Addins.Pidgin
{
	public class PidginStatusTypeItem : IItem
	{
		string iconBase;
		uint status;
		
		public PidginStatusTypeItem(uint status)
		{
			this.status = status;
			this.iconBase = "/usr/share/pixmaps/pidgin/status/48/";
		}
		
		public string Name {
			get {
				switch (status) {
				case 1: return "Offline";
				case 2: return "Available";
				case 3: return "Busy";
				case 4: return "Invisible";
				case 5: return "Away";
				default: return "Unknown Status";
				}
			}
		}
		
		public string Description {
			get { return Name; }
		}
		
		public string Icon { 
			get  { 
				switch (status) {
				case 1: return iconBase + "offline.png";
				case 2: return iconBase + "available.png";
				case 3: return iconBase + "busy.png";
				//there is not a 48px invisible icon.
				case 4: return "/usr/share/pixmaps/pidgin/status/32/invisible.png";
				case 5: return iconBase + "away.png";
				default: return "pidgin";
				}
			}
		}
		
		public uint Status {
			get { return status; }
		}
	}
}
