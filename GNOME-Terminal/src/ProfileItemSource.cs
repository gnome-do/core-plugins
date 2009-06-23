// ProfileItemSource.cs
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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Universe;

namespace GNOME.Terminal
{

	public class ProfileItemSource : ItemSource
	{
		const string GConfTerminalPath = "/apps/gnome-terminal/profiles";
		static readonly string ProfilesDirectory = 
			Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.Personal),
				".gconf" + GConfTerminalPath);

		List<Item> items;

		public ProfileItemSource ()
		{
			items = new List<Item> ();
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("GNOME Terminal Profiles"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Indexes your GNOME Terminal profiles."); } 
		}

		public override string Icon {
			get { return "gnome-terminal"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ProfileItem);  }
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override void UpdateItems ()
		{
			items.Clear ();

			if (!Directory.Exists (ProfilesDirectory)) return;

			string [] profiles = Directory.GetDirectories (ProfilesDirectory);
			foreach (string _profile in profiles) {
				string profile = Regex.Replace (_profile, ProfilesDirectory, GConfTerminalPath);
				if (profile.EndsWith ("template", StringComparison.CurrentCultureIgnoreCase))
					continue;
				items.Add (new ProfileItem (profile));
			}
		}
	}
}
