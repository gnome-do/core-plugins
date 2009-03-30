//  GoogleCalculatorAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using Mono.Unix;

using Do.Platform;
using Do.Universe;
using Do.Universe.Common;

namespace Do.Plugins.Google
{

	public class GoogleCalculatorAction : Act
	{

		const string BeginCalculator = "<img src=/images/calc_img.gif";
		const string BeginReply = "<td nowrap dir=ltr><h2 class=r style=\"font-size:138%\"><b>";
		const string EndReply = "</b>";

		public GoogleCalculatorAction ()
		{
		}

		public override string Name {
			get { return "GCalculate"; }
		}

		public override string Description {
			get { return Catalog.GetString ("Perform a calculation using Google Calculator."); }
		}

		public override string Icon {
			get { return "accessories-calculator"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			string expression, url, page, reply;
			int beginCalculator, beginReply, endReply;
		 
			expression = (items.First () as ITextItem).Text;
			url	= GoogleCalculatorURLWithExpression (expression);
			try {
				page = GetWebpageContents (url);

				// Make sure the page contains a calculation result by
				// checking for the presence of the Calculator image:
				beginCalculator = page.IndexOf (BeginCalculator);
				if (beginCalculator < 0) {
					throw new Exception ();
				}
				page = page.Substring (beginCalculator);
				
				// Try to extract the reply:
				beginReply = page.IndexOf (BeginReply);
				if (beginReply < 0) {
					throw new Exception ();
				}
				reply = page.Substring (beginReply + BeginReply.Length);
				endReply = reply.IndexOf (EndReply); 
				reply = reply.Substring (0, endReply);
				// Strip HTML tags:
				reply = Regex.Replace (reply, @"<[^>]+>", "");
			} catch {
				reply = Catalog.GetString ("Google Calculator could not evaluate the expression.");
			}

			yield return new TextItem (HttpUtility.HtmlDecode(reply));
		}

		string GoogleCalculatorURLWithExpression (string e)
		{
			return "http://www.google.com/search?&q=" + HttpUtility.UrlEncode (e ?? "");
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
