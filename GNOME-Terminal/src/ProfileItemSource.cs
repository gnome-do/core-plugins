/* ProfileItemSource.cs
 * 
 * GNOME Do is the legal property of its developers, whose names are too numerous
 * to list here.  Please refer to the COPYRIGHT file distributed with this
 * source distribution.
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Unix;
using Do.Universe;

namespace GNOME.Terminal
{

	public class ProfileItemSource : IItemSource
	{
		private static string GCONF_TERMINAL = "/apps/gnome-terminal/profiles";

		List<IItem> items;

		public ProfileItemSource()
		{
			items = new List<IItem> ();
		}

	    public string Name { get { return Catalog.GetString ("Gnome Terminal Profiles"); } }
	    
	    public string Description {
	    	get { return Catalog.GetString ("Indexes your Gnome Terminal profiles."); } 
	    }
	    
	    public string Icon { get { return "gnome-terminal"; } }

	    public Type[] SupportedItemTypes {
	      get {
	        return new Type[] { typeof (ProfileItem) };
	      }
	    }

	    public ICollection<IItem> Items {
	      get { return items; }
	    }

	    public ICollection<IItem> ChildrenOfItem (IItem parent)
	    {
	      return null;  
	    }

	    public void UpdateItems ()
	    {
			items.Clear ();

			string gconfBase = Do.Paths.Combine (Do.Paths.UserHome, ".gconf");
			string[] profiles = Directory.GetDirectories (gconfBase + GCONF_TERMINAL);
			foreach (string _profile in profiles) {
				string profile = Regex.Replace (_profile, gconfBase, "");
				if (profile.EndsWith ("template", StringComparison.CurrentCultureIgnoreCase))
					continue;
				items.Add (new ProfileItem (profile));
			}
		}
	}
}
