// ProfileItem.cs
// 
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
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
using System.Diagnostics;
using System.Collections.Generic;

using GConf;
using Mono.Addins;

using Do.Platform;
using Do.Universe;

namespace GNOME.Terminal 
{

	public class ProfileItem : Item, IOpenableItem
	{
		readonly new string DefaultName = AddinManager.CurrentLocalizer.GetString ("Unnamed Profile");
		readonly new string DefaultDescription = AddinManager.CurrentLocalizer.GetString ("GNOME Terminal Profile");

		string name, description;

		public ProfileItem (string profilePath)
		{
			Client client = new Client ();

			name = description = null;
			try {
				name = (string) client.Get (profilePath + "/visible_name");
				description = (string) client.Get (profilePath + "/custom_command");
				if (!string.IsNullOrEmpty (description))
					description = string.Format ("{0} - {1}", description, DefaultDescription);
			} catch {
				name = description = null;
			} finally {
				if (string.IsNullOrEmpty (name))
					name = DefaultName;
				if (string.IsNullOrEmpty (description))
					description = DefaultDescription;
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

		public void Open ()
		{
			try {
				string args = string.Format ("--window-with-profile=\"{0}\"", Name);
				Process.Start ("gnome-terminal", args);
			} catch (Exception e) {
				Log<ProfileItem>
					.Error ("Could not open gnome-terminal for {0}: {1}", Name, e.Message );
				Log<ProfileItem>.Debug (e.StackTrace);
			}
		}
	}
}
