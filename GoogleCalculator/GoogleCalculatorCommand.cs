//  GoogleCalculatorCommand.cs
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

	public class GoogleCalculatorCommand : AbstractCommand
	{

		const string BeginReply = "<img src=/images/calc_img.gif alt=\"\"></td><td>&nbsp;</td><td nowrap><h2 class=r><font size=+1><b>";
		const string EndReply = "</b>";

		public GoogleCalculatorCommand ()
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
			int beginReply, endReply;
		 
			expression = (items[0] as ITextItem).Text;
			url	= GoogleCalculatorURLWithExpression (expression);
			try {
				page = GetWebpageContents (url);
				beginReply = page.IndexOf (BeginReply);
				if (beginReply < 0) {
					throw new Exception ("Google Calculator could not " +
						 "evaluate the expression.");
				}
				reply = page.Substring (beginReply + BeginReply.Length);
				endReply = reply.IndexOf (EndReply); 
				reply = reply.Substring (0, endReply);
				// Strip HTML tags.
				reply = Regex.Replace (reply, @"<[^>]+>", "");
			} catch (Exception e) {
				reply = e.Message;
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
