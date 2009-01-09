// ProfileItem.cs
// 
// GNOME Do is the legal property of its developers, whose names are too numerous
// to list here.  Please refer to the COPYRIGHT file distributed with this
// source distribution.
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
using System.Collections.Generic;

using GConf;
using Mono.Unix;

using Do.Universe;

namespace GNOME.Terminal 
{
	public class ProfileItem : Item
	{
		readonly new string DefaultName = Catalog.GetString ("Unnamed Profile");
		readonly new string DefaultDescription = Catalog.GetString ("GNOME Terminal Profile");

		string name, description;

		public ProfileItem (string profilePath)
		{
			Client client = new Client ();

			name = description = null;
			try {
				name = (string) client.Get (profilePath + "/visible_name");
				description = (string) client.Get (profilePath + "/custom_command");
			} catch {
				name = description = null;
			} finally {
				name = name ?? DefaultName;
				description = description ?? DefaultDescription;
			}
		}

		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return description; }
		}

		public override string Icon {
			get { return "gnome-terminal"; }
		}
	}
}
