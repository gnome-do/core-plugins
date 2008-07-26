//InlineGoogleSearch.cs created with MonoDevelop
//Brian Lucas (bcl1713@gmail.com)
//sacul@irc.ubuntu.com/#gnome-do
// 
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;

using Do.Universe;
using Do.Addins;
using Mono.Unix;

/// <summary>
/// Do plug-in that returns search results from google back to gnome-do for 
/// further processing
/// </summary>
namespace InlineGoogleSearch
{
	
	/// <summary>
	/// Class Definition
	/// </summary>
	public class InlineGoogleSearch : AbstractAction, IConfigurable
	{	
		/// <value>
		/// Search Google
		/// </value>
		public override string Name {
			get { return Catalog.GetString ("Search Google"); }
		}
		
		/// <value>
		/// Searches google and returns results to Do
		/// </value>
		public override string Description {
			get { return Catalog.GetString ("Searches google and returns results to Do"); }
		}
		
		/// <value>
		/// web-browser
		/// </value>
		public override string Icon {
			get { return "web-browser"; }
		}
		
		/// <value>
		/// ITextItem
		/// </value>
		public override Type[] SupportedItemTypes {
			get {
				return new Type[] {                             
					typeof (ITextItem),
				};
			}
		}
		
		/// <value>
		/// true
		/// </value>
		public override bool ModifierItemsOptional {
			get { return false; }
		}

		/// <summary>
		/// Actual code performed when action is executed in Do
		/// </summary>
		/// <param name="items">
		/// Items. ITextItem <see cref="IItem"/>
		/// </param>
		/// <param name="modItems">
		/// Modifier Items. None <see cref="IItem"/>
		/// </param>
		/// <returns>
		/// Array of Items. URLs to search results <see cref="IItem"/>
		/// </returns>
		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			List<IItem> retItems = new List<IItem> ();
			
			//Check searchRestrictions from InlineGoogleSearchConfig and set
			//safesearch string appropriately

			//API string sent to google
			String endpointURL =
				"http://www.google.com/uds/GwebSearch?"
					+ "callback=GwebSearch.RawCompletion"
					+ "&context=0&lstkp=0&rsz=large&h1=en&"
					+ "sig=8656f49c146c5220e273d16b4b6978b2&"
					+ InlineGoogleSearchConfig.SearchRestrictions
					+ "q=" + (items[0] as ITextItem).Text + "&"
					+ "v=1.0";
			WebRequest wrq = WebRequest.Create(endpointURL);
			WebResponse wrs = wrq.GetResponse();
			StreamReader sr = new StreamReader(wrs.GetResponseStream());
			string parseString = sr.ReadLine();
			//Parse the returned string to remove JSON markup and return only 
			//URLs
			retItems = Parse(parseString);
			
			return retItems.ToArray();
		}
		
		/// <summary>
		/// Parses Json String and returns List of strings containing URLs
		/// This code could use some further tweaking to extract more of the
		/// usefull information provided by google.  Right now it just strips
		/// out the URLs and nothing else.
		/// </summary>
		/// <param name="parseString">
		/// Input Json String from Google <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// List of Strings containing URLs <see cref="List`1"/>
		/// </returns>gconf
		private List<IItem> Parse(string parseString){
			List<IItem> items = new List<IItem> ();
			string[] array;
			array = parseString.Split(',');
			int ub = array.GetLength(0);
			for (int i=0; i<ub;i++){
				if (array[i].Contains("\"url\"")){
					array[i] = array[i].Remove(0,7);
					array[i] = array[i].TrimEnd('"');
					items.Add(new TextItem(array[i]));
				}
			}
			return items;
		}

		/// <summary>
		/// Initializes InlineGoogleSearch
		/// </summary>
		public InlineGoogleSearch(){
		}
		
		/// <summary>
		/// Calls config dialog
		/// </summary>
		/// <returns>
		/// InlineGoogleSearchConfig Widget <see cref="Gtk.Bin"/>
		/// </returns>
		public Gtk.Bin GetConfiguration ()
		{
			return new InlineGoogleSearchConfig();
		}

	}
}
