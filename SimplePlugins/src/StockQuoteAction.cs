/* StockQuoteAction.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using Do.Universe;
using Do.Addins;

namespace Do.Plugins.Universe
{
	/// <summary>
	/// Given an ITextItem, StockQuoteAction will query Google Finance
	/// for the quote, then scrape out the price, daily movement and percent move
	/// </summary>
	public class StockQuoteAction : AbstractAction
	{

		// String indicating the tags surronding the pertinent info
		const string BeginPrice = "_l\">";
		const string BeginMove = "_c\">";
		const string BeginPercent =  "_cp\">";
		const string EndResult = "</span>";

		public StockQuoteAction ()
		{
		}
		
		public override string Name {
			get { return "Stock Quote"; }
		}
		
		public override string Description
		{
			get { return "Get Quotes From Google Finance."; }
		}
		
		public override string Icon
		{
			get { return "stock_chart-autoformat"; }
		}
		
		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			string word;

			word = null;
			if (item is ITextItem) {
				word = (item as ITextItem).Text;
			}
			return !string.IsNullOrEmpty (word);
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			string expression, url, reply, priceString, moveString, percentString, page;
			string pagePrice, pageMove, pagePer;
			int beginPrice, beginMove, beginPercent, endIndex;
			expression = (items[0] as ITextItem).Text;
			url = GoogleFinanceURL (expression);
			try {
				page = GetWebpageContents (url);
				beginPrice = page.IndexOf (BeginPrice);
				beginMove = page.IndexOf (BeginMove);
				beginPercent = page.IndexOf (BeginPercent);
				
				// Grab price
				if (beginPrice < 0 | beginMove < 0 | beginPercent < 0)
					throw new Exception ();
				pagePrice = page.Substring (beginPrice);
				endIndex = pagePrice.IndexOf (EndResult);
				if (endIndex < 0)
					throw new Exception ();
				priceString = pagePrice.Substring (BeginPrice.Length, endIndex-BeginPrice.Length);
				
				// Grab daily move
				pageMove = page.Substring (beginMove);
				endIndex = pageMove.IndexOf (EndResult);
				if (endIndex < 0)
					throw new Exception ();
				moveString = pageMove.Substring (BeginMove.Length, endIndex-BeginMove.Length);
				
				// Grab percent move
				pagePer = page.Substring (beginPercent);
				endIndex = pagePer.IndexOf (EndResult);
				if (endIndex < 0)
					throw new Exception ();
				percentString = pagePer.Substring (BeginPercent.Length, endIndex-BeginPercent.Length);
				
				reply = priceString + " " + moveString + " " + percentString;
			} catch {
				reply = "Google Finance could not process your request";
			}
			return new IItem[] { new TextItem (reply) };
		}
		
		string GoogleFinanceURL (string e)
		{
			return "http://finance.google.com/finance?q=" + (e ?? "");
		}
		
		string GetWebpageContents (string url)
		{
			HttpWebRequest request;
			WebResponse response;
			Stream stream;
			StreamReader reader;
			string content;
			 
			request	= HttpWebRequest.Create (url) as HttpWebRequest;
			response = request.GetResponse ();
			stream = response.GetResponseStream ();
			reader = new StreamReader (stream);
			content = reader.ReadToEnd ();
		
			reader.Close ();
			stream.Close ();
			response.Close ();
		 
			return content;
		}

	}
}
