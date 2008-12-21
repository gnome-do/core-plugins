/* ManLookUpAction.cs is part of ManLookUp, a Gnome-Do plugin
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
 */
using System;
using Do.Universe;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using GConf;
using Mono.Unix;


namespace GnomeDoManLookUp {

	/// <summary>
	/// 	ManLookUpAction - the main action for our plugin. 
	///		Allows us to hook up to Gnome-Do as a command
	///		that can be applied to raw text or our own man page items. 
	/// </summary>
	public class ManLookUpAction : Act {
		
		/// <value>
		/// 	The name of the action
		/// </value>
		public override string Name {
			get { return Catalog.GetString ("Read help documentation (man)"); }
		}
		
		/// <value>
		/// 	Action's description
		/// </value>
		public override string Description {
			get { return Catalog.GetString ("Look up and read a manual page (man)"); }
		}
		
		/// <value>
		/// 	The pretty icon
		/// </value>	
		public override string Icon { 
			get { return "applications-office"; }
		}
		
		/// <value>
		/// 	List of supported items (ITextItem , ManLookUpItem)
		/// </value>
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof(ITextItem),typeof(ManLookUpItem),typeof(ApplicationItem)
				};
			}
           
		}
		
		/// <summary>
		/// 	Do we support the item?
		/// </summary>
		/// <param name="item">
		/// A <see cref="Item"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool SupportsItem (Item item) 
		{
			
			bool rc  = false;
			string execStr;

			if (!(item is ApplicationItem)) {			
				rc = true;				
			} else {
				
				ApplicationItem appItem = item as ApplicationItem;

				//grab the parameter we'll be using for whatis command from the Exec string
				//note, we use whatis becuase redirecting its output is a lot more efficient
				//than that of a regular 'man page' which can contain thousands of lines. 
				//man and whatis should use the same db anyways..
				execStr = this.getExecutableName (appItem);
				
				//is there a matching man page?
				if (execStr.Length > 0) {
					Process term 				= new Process ();
					term.StartInfo.FileName 		= "whatis";
					term.StartInfo.Arguments 		= "'"+execStr+"'";
					term.StartInfo.UseShellExecute 		= false;
					term.StartInfo.RedirectStandardOutput 	= true;
					term.StartInfo.RedirectStandardError 	= true;
					term.Start ();
					
					//if we don't put these, WaitForExit () blocks
					//on big buffers or ExitCode is not avalable yet..
					term.StandardOutput.ReadToEnd ();
					term.WaitForExit ();
					
					//if we didn't find a man page, we don't support it. 
					rc = term.ExitCode == 0;
				}
				
				
			}
			
			return rc;
			
		}
		
		/// <summary>
		///    Parse the <see cref="ApplicationItem"/> Exec string to attempt to obtain
		///    name of the binary being executed so we may use it as a parameter for man
		/// </summary>
		/// <param name="appItem">
		/// A <see cref="ApplicationItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private static string getExecutableName (ApplicationItem appItem) 
		{			
			string execStr;
			Match m;
			Regex r;
			int i;		
			
			// Now we parse the execute string to attempt to find a binary 
			// name that we can look up a man page for. Ideally this should
			// be given to us rather than guess the format but this
			// will have to do for now.
			// 1. if being invoked with gksu, ignore gksu itself and grab its parameter
			// 2. remove any arguments sent to the call
			// 3. reduce absolute paths to just the filename. 
			//
			r = new Regex ("^(gksu\\s+)?([^ ]+)\\s?.*$");
			m = r.Match (appItem.Exec);			
			execStr = !m.Success ? appItem.Exec : execStr = m.Groups [2].ToString ();
			
			//grab base
			i = execStr.LastIndexOf ('/');
			if (i != -1)
				execStr = execStr.Substring (i+1);
			
			return execStr;
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
			
            		string keyword = null;

			//ok, was it plain text, an application item, or one of our own?
			if (items.First () is ApplicationItem) {
				keyword = this.getExecutableName (items.First () as ApplicationItem);
			} else if (items.First () is ManLookUpItem) {
				ManLookUpItem keyworditem = items.First () as ManLookUpItem;
				keyword = keyworditem.Text;							
			} else {
				ITextItem textitem = items.First () as ITextItem;
				keyword = textitem.Text;				 
			}
			 
			
			if (keyword != null && keyword.Length > 0) {
				Process term = new Process ();
				term.StartInfo.FileName = "yelp";
				term.StartInfo.Arguments = " 'man:"+keyword+"' ";				
				term.Start ();
			}
			
			return null;
		}
	}
}
