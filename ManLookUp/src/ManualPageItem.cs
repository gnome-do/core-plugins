// ManualPageItem.cs
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;

namespace ManPages {

	/// <summary>
	/// 	ManualPageItem - Our custom manual page entries
	/// </summary>
	public class ManualPageItem : Item, IOpenableItem {

		string name, description;

		/// <summary>
		/// 	Initializes our item. 
		/// </summary>
		/// <param name="pagename">
		/// A <see cref="System.String"/> representing the command name
		/// </param>
		/// <param name="desc">
		/// A <see cref="System.String"/> representing the description given by man
		/// </param>
		public ManualPageItem (string pageName, string desc)
		{
			name = pageName;
			description = desc;
		}

		/// <value>
		/// 	The entry's name. Usually the actual man page name. 
		/// </value>
		public override string Name {
			get { return name; }
		}

		/// <value>
		/// 	The entry's description
		/// </value>
		public override string Description { 
			get { return description; }
		}

		/// <value>
		/// 	Our pretty icon
		/// </value>
		public override string Icon { 
			get { return "help-browser"; }
		}

		/// <value>
		/// 	The text associated with our custom entry. (we use the man page's name)
		/// </value>
		public string Text { 
			get { return name; }
		}

		/// <summary>
		/// 	 Once our custom item entry is selected, do! 
		/// </summary>
		public void Open ()
		{
			Process term = new Process ();
			term.StartInfo.FileName = "yelp";
			term.StartInfo.Arguments = " 'man:"+name+"' ";				
			term.Start ();
		}
	}
}
