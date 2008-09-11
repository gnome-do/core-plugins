//GoogleSearch.cs created with MonoDevelop
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
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// InlineGoogleSearch namespace
/// </summary>
namespace InlineGoogleSearch
{
	
	/// <summary>
	/// GoogleSearch Class
	/// </summary>
	public class GoogleSearch
	{

		private string safeSearchLevel = "moderate";
		private string RSZ = "large";
		private string query = "";
		private List<GoogleSearchResult> resultsList = new List<GoogleSearchResult> ();
		
		/// <summary>
		/// initializes Google Search with no parameters
		/// </summary>
		public GoogleSearch(){
		}
		
		/// <summary>
		/// Initializes Google Search with a clean query as parameter
		/// </summary>
		/// <param name="inQuery">
		/// string <see cref="System.String"/>
		/// </param>
		public GoogleSearch(string inQuery){
			this.query = inQuery;
		}
		
		/// <summary>
		/// Sets SafeSearch Level for google query. Legal values are "moderate"
		/// "active" and "off"
		/// </summary>
		/// <param name="ssl">
		/// string <see cref="System.String"/>
		/// </param>
		public void setSafeSearchLevel(string ssl){
			if (ssl == "moderate" || ssl == "active" || ssl == "off"){
				this.safeSearchLevel = ssl;
			}
			else {
				System.Console.WriteLine("Error in Google Search: Invalid " +
				                         "SafeSearch level specified! Default" +
				                         " value assigned!");
				this.safeSearchLevel = "moderate";
			}
		}
		
		/// <summary>
		/// Sets RSZ(return size?) for google query.  Legal values are "small" 
		/// and "large"
		/// </summary>
		/// <param name="rsz">
		/// string <see cref="System.String"/>
		/// </param>
		public void setRSZ(string rsz){
			if (rsz == "large" || rsz == "small"){
				this.RSZ = rsz;
			}
			else {
				System.Console.WriteLine("Error in GoogleSearch: Invalid RSZ " +
				                         "specified! Default value assigned!");
				this.RSZ = "large";
			}
		}
		
		/// <summary>
		/// Set query string
		/// </summary>
		/// <param name="inQuery">
		/// string <see cref="System.String"/>
		/// </param>
		public void setQuery(string inQuery){
			this.query = inQuery;
		}
		
		/// <summary>
		/// Preforms the actual Search
		/// </summary>
		/// <returns>
		/// A <see cref="GoogleSearchResult"/>
		/// </returns>
		public GoogleSearchResult[] search(){
			this.query = HttpUtility.UrlEncode(this.query);
			string endpointURL =
				"http://www.google.com/uds/GwebSearch?"
					+ "callback=GwebSearch.RawCompletion"
					+ "&context=0"
					+ "&lstkp=0"
					+ "&rsz=" + this.RSZ
					+ "&h1=en"
					+ "&sig=8656f49c146c5220e273d16b4b6978b2&"
					+ "&safe=" + this.safeSearchLevel
					+ "&q=" + this.query 
					+ "&v=1.0";
			WebRequest wrq = WebRequest.Create(endpointURL);
			WebResponse wrs = wrq.GetResponse();
			StreamReader sr = new StreamReader(wrs.GetResponseStream());
			string parseString = sr.ReadLine();
			this.parse(parseString);
			return resultsList.ToArray();
		}
		
		/// <summary>
		/// Parses the returned string from Google and initializes resultsList
		/// </summary>
		/// <param name="ps">
		/// string <see cref="System.String"/>
		/// </param>
		private void parse(string ps){
			string[] array;
			string[] temp;
			//remove leading unused information
			ps = ps.Remove(0,42);
			//remove trailing unused information
			temp = Regex.Split(ps,"}]");
			//split the used string into individual results
			array = Regex.Split(temp[0], "},{");
			int ub = array.GetLength(0);
			for (int i=0; i<ub;i++){
				GoogleSearchResult result = new GoogleSearchResult(array[i]);
				this.resultsList.Add(result);
			}
		}
	}
}
