//  Pastebin.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify it
//  under the terms of the GNU General Public License as published by the Free
//  Software Foundation, either version 3 of the License, or (at your option)
//  any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//  more details.
//
//  You should have received a copy of the GNU General Public License along with
//  this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Specialized;

namespace Pastebin
{
	public static class Pastebin
	{		
		public static string PostUsing (IPastebinProvider pastebin)
		{
			string url = null;
			try
			{
				string postQueryString = CreateQueryString (pastebin.Parameters);

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (pastebin.BaseUrl);
				request.Timeout = 15000;
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.AllowAutoRedirect = pastebin.ShouldAllowAutoRedirect;

				UTF8Encoding encoding = new UTF8Encoding ();
				byte[] data = encoding.GetBytes (postQueryString);
				request.ContentLength = data.Length;
				
				using (Stream newStream = request.GetRequestStream ())
				{
					newStream.Write(data, 0, data.Length);
				}
				
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{			
					url = pastebin.GetPasteUrlFromResponse (response);
				}
			}
			catch
			{
				url = "An error occured while pasting.";
			}
			
			return url;
		}
		
		private static string CreateQueryString (NameValueCollection query)
		{
			StringBuilder queryString = new StringBuilder ();
			foreach (string key in query.Keys)
			{
				queryString.Append (HttpUtility.UrlEncode(key));
				queryString.Append ("=");
				queryString.Append (HttpUtility.UrlEncode(query[key]));
				queryString.Append ("&");
			}
			queryString.Length--;
			return queryString.ToString ();
		}
	}
}
