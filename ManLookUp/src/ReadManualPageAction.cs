// ReadManualPageAction.cs
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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Unix;

using Do.Universe;

namespace ManPages {

	/// <summary>
	/// 	ReadManualPageAction - the main action for our plugin. 
	///		Allows us to hook up to Gnome-Do as a command
	///		that can be applied to raw text or our own man page items. 
	/// </summary>
	public class ReadManualPageAction : Act {

		/// <value>
		/// 	The name of the action
		/// </value>
		public override string Name {
			get { return Catalog.GetString ("Read manual page (man)"); }
		}

		/// <value>
		/// 	Action's description
		/// </value>
		public override string Description {
			get { return Catalog.GetString ("Look up and read a manual page."); }
		}

		/// <value>
		/// 	The pretty icon
		/// </value>	
		public override string Icon { 
			get { return "applications-office"; }
		}

		/// <value>
		/// 	List of supported items (ITextItem)
		/// </value>
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
			}
		}

		/// <summary>
		/// 	Called by Gnome-Do in order to perform our action.
		/// </summary>
		/// <param name="items">
		/// List of <see cref="Item"/> objects, either raw text or custom look up items
		/// </param>
		/// <param name="modItems">
		/// List of <see cref="Item"/> objects, action modifiers
		/// </param>
		/// <returns>
		/// List of <see cref="Item"/>
		/// </returns>
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) 
		{
			string keyword;
			foreach (Item i in items)
			{
				keyword = (i as ITextItem).Text;
				if (!string.IsNullOrEmpty (keyword)) {
					Process term = new Process ();
					term.StartInfo.FileName = "yelp";
					term.StartInfo.Arguments = " 'man:"+keyword+"' ";				
					term.Start ();
				}
			}
			yield break;
		}
	}
}
