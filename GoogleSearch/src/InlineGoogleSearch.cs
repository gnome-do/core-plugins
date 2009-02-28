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
// (at your option) any later version.
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
using System.Web;
using System.Collections.Generic;
using System.Linq;

using Do.Universe;
using Do.Universe.Common;
using Do.Platform;
using Do.Platform.Linux;

using Mono.Unix;

/// <summary>
/// Do plug-in that returns search results from google back to gnome-do for 
/// further processing
/// </summary>
namespace InlineGoogleSearch {

	
	// No longer in Do, have to subclass 
	public class BookmarkItem : Item, IBookmarkItem 
	{
		protected string name, url;
		public BookmarkItem (string name, string url)
		{
			this.name = name;
			this.url = url;
		}
		public override string Name
		{
			get { return name; }
		}
		public override string Description
		{
			get { return url; }
		}

		public override string Icon
		{
			get { return "www"; }
		}

		public string Url
		{
			get { return url; }
		}
	} 
	
	/// <summary>
	/// Class Definition
	/// </summary>
	public class InlineGoogleSearch : Act, IConfigurable {	
		
		/// <value>
		/// Search Google
		/// </value>
		public override string Name {
			get { 
				return Catalog.GetString ("Search Google"); 
			}
		}
		
		/// <value>
		/// Searches google and returns results to Do
		/// </value>
		public override string Description {
			get { 
				return Catalog.GetString ("Allows you to perform Google Searches from Do"); 
			}
		}
		
		/// <value>
		/// web-browser
		/// </value>
		public override string Icon {
			get { 
				return "web-browser"; 
			}
		}
		
		/// <value>
		/// ITextItem
		/// </value>
		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (ITextItem); 
			} 
		}
					
		
		/// <summary>
		/// Actual code performed when action is executed in Do
		/// </summary>
		/// <param name="items">
		/// Items. ITextItem <see cref="Item"/>
		/// </param>
		/// <param name="modItems">
		/// Modifier Items. None <see cref="Item"/>
		/// </param>
		/// <returns>
		/// Array of Bookmark Items. URLs to search results <see cref="Item"/>
		/// </returns>
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) 
		{
			string query = (items.First () as ITextItem).Text;
			string searchURL = "http://google.com/search?q=" + HttpUtility.UrlEncode (query);
			
			if (InlineGoogleSearchConfig.InheritSSL) {
				searchURL += "&safe=" + InlineGoogleSearchConfig.SearchRestrictions;
			}
			
			if (!InlineGoogleSearchConfig.ReturnResults) {
				Services.Environment.OpenUrl (searchURL);
				yield break;
			}

			if (InlineGoogleSearchConfig.ShowSearchFirst) {
				yield return new BookmarkItem ( query + " - Google Search",
				                               searchURL );
			}
			
			GoogleSearch googleSearch = new GoogleSearch ();
			googleSearch.setSafeSearchLevel (InlineGoogleSearchConfig.SearchRestrictions);
			googleSearch.setQuery ( query );
			IEnumerable<GoogleSearchResult> results = googleSearch.Search ();


			if (!results.Any ()) {
				Gtk.Application.Invoke ((o, e) => Services.Notifications.Notify (Name, "No Results Found"));
			}
			
			foreach (GoogleSearchResult result in results) {
				yield return new BookmarkItem (result.titleNoFormatting, HttpUtility.UrlDecode (result.url));
			}
		}

		/// <summary>
		/// Initializes InlineGoogleSearch
		/// </summary>
		public InlineGoogleSearch () 
		{
		}
		
		/// <summary>
		/// Calls config dialog
		/// </summary>
		/// <returns>
		/// InlineGoogleSearchConfig Widget <see cref="Gtk.Bin"/>
		/// </returns>
		public Gtk.Bin GetConfiguration () 
		{
			return new InlineGoogleSearchConfig ();
		}
	}
}
