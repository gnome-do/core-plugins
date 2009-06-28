//ImFeelingLuckyAction.cs created with MonoDevelop
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
using Do.Platform;
using Mono.Addins;

/// <summary>
/// Action that immediately takes you to the first result provided by Google
/// </summary>
namespace InlineGoogleSearch {
	
	/// <summary>
	/// Class Definition
	/// </summary>
	public class ImFeelingLucky : Act {	
		/// <value>
		/// I'm Feeling Lucky
		/// </value>
		public override string Name {
			get { 
				return AddinManager.CurrentLocalizer.GetString ("I'm Feeling Lucky!"); 
			}
		}
		
		/// <value>
		/// Searches google and takes you to the first returned result
		/// </value>
		public override string Description {
			get { 
				return AddinManager.CurrentLocalizer.GetString ("Searches Google and takes you to the first result"); 
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
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) 
		{
			GoogleSearch googleSearch = new GoogleSearch ();
			googleSearch.setSafeSearchLevel
				 (InlineGoogleSearchConfig.SearchRestrictions);
			googleSearch.setQuery ( (items.First () as ITextItem).Text);
			IEnumerable<GoogleSearchResult> results = googleSearch.Search ();
			
			if (!results.Any ()) {
				Gtk.Application.Invoke ((o, e) => Services.Notifications.Notify (Name, "No Results Found"));
			} else {
				Services.Environment.OpenUrl (HttpUtility.UrlDecode (results.First ().url));
			}
			yield break;
		}

		/// <summary>
		/// Initializes ImFeelingLucky
		/// </summary>
		public ImFeelingLucky () 
		{
		}
		
	}
}
