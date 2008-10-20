/* ManLookUpEntries.cs is part of ManLookUp, a Gnome-Do plugin
 *
 * Copyright 2008 J. Carlos Navea
 * loconet@gmail.com
 *
 * Man Look Up is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * Contributors:
 * J. Carlos Navea <loconet@gmail.com>
 *
 * Based on the excellent work of sample plugins included with Gnome-Do 
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Do.Universe;
using Mono.Unix;


namespace GnomeDoManLookUp {

	/// <summary>
	/// 	ManLookUpItem - Our custom manual page entries
	/// </summary>
	public class ManLookUpItem : IOpenableItem {
		
		private string name;
		private string description;

		/// <summary>
		/// 	Initializes our item. 
		/// </summary>
		/// <param name="pagename">
		/// A <see cref="System.String"/> representing the command name
		/// </param>
		/// <param name="desc">
		/// A <see cref="System.String"/> representing the description given by man
		/// </param>
		public ManLookUpItem (string pageName, string desc)
		{
			name = pageName;
			description = desc;
		}

		/// <value>
		/// 	The entry's name. Usually the actual man page name. 
		/// </value>
		public string Name {
			get { return name; }
		}
		
		/// <value>
		/// 	The entry's description
		/// </value>
		public string Description { 
			get { return description; }
		}
		

		/// <value>
		/// 	Our pretty icon
		/// </value>
		public string Icon { 
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

	/// <summary>
	/// 	ManItemSource - Our "indexer" or data source of man items.. 
	/// </summary>
	public class ManLookUpItemSource : IItemSource { 
	
		List<IItem> items;
		
		/// <summary>
		/// 	Initialize the object and update the list of items
		/// </summary>
		public ManLookUpItemSource () 
		{
			items = new List<IItem> ();
			UpdateItems ();
		}


		/// <value>
		/// 	Name of data source
		/// </value>
		public string Name { 
			get { return Catalog.GetString ("Documentation help entries (man)"); }
		}

		/// <value>
		/// 	Description of data source
		/// </value>	
		public string Description { 
			get { return Catalog.GetString ("Search and read help documentation (man)"); }
		}
		

		/// <value>
		/// 	Our pretty icon
		/// </value>
		public string Icon { 
			get { return "applications-office"; }
		}

		/// <value>
		/// 	What type of items do we support
		/// </value>
		public Type[] SupportedItemTypes {
			get {
				return new Type[] { typeof (ManLookUpItem) };
			}
		}

		/// <value>
		/// 	Our collection of items
		/// </value>
		public ICollection<IItem> Items {
			get { return items; }
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent">
		/// A <see cref="IItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="ICollection`1"/>
		/// </returns>
		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;  
		}


		/// <summary>
		/// 	Update the items we are keeping track of
		/// </summary>
		public void UpdateItems ()
		{
			items.Clear ();

			try {
				
				//
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
				
				System.IO.StreamReader oReader2 = term.StandardOutput;

				//man -k output format: command (section-int) - description
				Regex r = new Regex ("^([^ ]+)\\s\\([1-9]+\\)\\s+-\\s(.*)$");
				
				//probably not the best way of reading the lines...but..
				char[] charArray = new char [] {'\n'};
				foreach (string line in oReader2.ReadToEnd ().Split (charArray)) {
					Match m = r.Match (line);
					if (m.Success) 
						items.Add (new ManLookUpItem(m.Groups [1].ToString (),m.Groups [2].ToString ()));
				}

				
			} catch { }
		}
	}
}
