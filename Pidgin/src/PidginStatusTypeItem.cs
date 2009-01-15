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
using System.IO;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace PidginPlugin
{

	public class PidginStatusTypeItem : Item
	{

		const string IconBase = "/usr/share/pixmaps/pidgin/status/48";

		uint status;
		
		public PidginStatusTypeItem (uint status)
		{
			this.status = status;
		}
		
		public override string Name {
			get {
				switch (status) {
				case 1: return Catalog.GetString ("Offline");
				case 2: return Catalog.GetString ("Available");
				case 3: return Catalog.GetString ("Busy");
				case 4: return Catalog.GetString ("Invisible");
				case 5: return Catalog.GetString ("Away");
				default: return Catalog.GetString ("Unknown Status");
				}
			}
		}
		
		public override string Description {
			get { return Name; }
		}
		
		public override string Icon { 
			get  { 
				switch (status) {
				case 1: return Path.Combine (IconBase, "offline.png");
				case 2: return Path.Combine (IconBase, "available.png");
				case 3: return Path.Combine (IconBase, "busy.png");
				//there is not a 48px invisible icon.
				case 4: return "/usr/share/pixmaps/pidgin/status/32/invisible.png";
				case 5: return Path.Combine (IconBase, "away.png");
				default: return "pidgin";
				}
			}
		}
		
		public uint Status {
			get { return status; }
		}
	}
}
