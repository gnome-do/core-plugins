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
using System.Net;
using System.IO;
using System.Collections.Generic;

using Do.Platform;

namespace SqueezeCenter
{
		
	public static class IconDownloader
	{
		const string couldNotDownloadIcon = "gtk-cdrom";
		private static Dictionary<string, string> downloadedIcons = new Dictionary<string,string> ();
		
		public static string GetIcon (string name)
		{	
			if (downloadedIcons.ContainsKey (name))
				return downloadedIcons[name];
			
			string result = couldNotDownloadIcon;			
			WebRequest request;
			byte[] buffer = null;
			int position, bytesRead;
			
			request = WebRequest.Create (name);
			try {				
				HttpWebResponse response;
				Stream stream;

				response = request.GetResponse () as HttpWebResponse;			
				try {
					if (response.StatusCode == HttpStatusCode.OK && 
					   response.ContentType.StartsWith ("image/")) {
						
						stream = response.GetResponseStream ();
						buffer = new byte[response.ContentLength];
						position = 0;					
						do {
							bytesRead = stream.Read (buffer, position, buffer.Length - position);
							position += bytesRead;						
						} while (bytesRead > 0);
					}
				} finally {
					response.Close ();
				}
				
				if (buffer != null) {
					result = Services.Paths.GetTemporaryFilePath();
					File.WriteAllBytes (result, buffer);				
				}
			} catch (Exception) {				
			}
			
			downloadedIcons.Add (name, result);
			return result;
		}
		
	}
}
