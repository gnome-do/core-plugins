// ManualPageItemSource.cs
//
// J. Carlos Navea <loconet@gmail.com>
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
using System.Text.RegularExpressions;

using Mono.Unix;

using Do.Universe;

namespace ManPages {

	/// <summary>
	/// 	ManItemSource - Our "indexer" or data source of man items.. 
	/// </summary>
	public class ManualPageItemSource : ItemSource { 

		List<Item> items;

		/// <summary>
		/// 	Initialize the object and update the list of items
		/// </summary>
		public ManualPageItemSource () 
		{
			items = new List<Item> ();
		}

		/// <value>
		/// 	Name of data source
		/// </value>
		public override string Name { 
			get { return Catalog.GetString ("Manual pages (man)"); }
		}

		/// <value>
		/// 	Description of data source
		/// </value>	
		public override string Description { 
			get { return Catalog.GetString ("Search and read help documentation (man)"); }
		}

		/// <value>
		/// 	Our pretty icon
		/// </value>
		public override string Icon { 
			get { return "applications-office"; }
		}

		/// <value>
		/// 	What type of items do we support
		/// </value>
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ManualPageItem); }
		}

		/// <value>
		/// 	Our collection of items
		/// </value>
		public override IEnumerable<Item> Items {
			get { return items; }
		}

		/// <summary>
		/// 	Update the items we are keeping track of
		/// </summary>
		public override void UpdateItems ()
		{
			items.Clear ();

			try {
				//We'll use man -k with a very hungry wildcard to get
				//the whole list of manual pages. This *should* be nicer 
				//than accessing the mandb directly so that localization is taken
				//care of. We also don't have to worry about file location.
				//
				Process term = new Process ();
				term.StartInfo.FileName = "man";
				term.StartInfo.Arguments = " -k '.' ";
				term.StartInfo.RedirectStandardOutput = true;
				term.StartInfo.UseShellExecute = false;

				term.Start ();

				StreamReader oReader2 = term.StandardOutput;

				//man -k output format: command (section-int) - description
				Regex r = new Regex ("^([^ ]+)\\s\\([1-9]+\\)\\s+-\\s(.*)$");

				//probably not the best way of reading the lines...but..
				char[] charArray = new char [] {'\n'};
				foreach (string line in oReader2.ReadToEnd ().Split (charArray)) {
					Match m = r.Match (line);
					if (m.Success) 
						items.Add (new ManualPageItem(m.Groups [1].ToString (),m.Groups [2].ToString ()));
				}
			} catch { }
		}
	}
}
