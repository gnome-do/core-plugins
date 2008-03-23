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
using System.Text.RegularExpressions;

using Do.Universe;

namespace Do.Plugins.Google
{

	public class GoogleCalculatorAction : AbstractAction
	{

		const string BeginCalculator = "<img src=/images/calc_img.gif";
		const string BeginReply = "<td nowrap><h2 class=r><font size=+1><b>";
		const string EndReply = "</b>";

		public GoogleCalculatorAction ()
		{
		}

		public override string Name {
			get { return "GCalculate"; }
		}

		public override string Description {
			get { return "Perform a calculation using Google Calculator."; }
		}

		public override string Icon {
			get { return "accessories-calculator"; }
		}

		public override Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			string expression, url, page, reply;
			int beginCalculator, beginReply, endReply;
		 
			expression = (items[0] as ITextItem).Text;
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
				reply = "Google Calculator could not evaluate the expression.";
			}

			return new IItem[] { new TextItem (reply) };
		}

		string GoogleCalculatorURLWithExpression (string e)
		{
			return "http://www.google.com/search?&q=" + (e ?? "")
				.Replace ("+", "%2B")
				.Replace ("(", "%28")
				.Replace (")", "%29")
				.Replace ("/", "%2F")
				.Replace ("^", "%5E")
				.Replace (" ", "+");
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
