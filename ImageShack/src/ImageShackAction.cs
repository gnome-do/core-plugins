//  ImageShackAction.cs
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Do.Addins;
using Do.Universe;

using Mono.Unix;

namespace ImageShack
{
	public class ImageShackAction : AbstractAction, IConfigurable
	{
		public override string Name
		{
			get { return Catalog.GetString ("Upload to ImageShack"); }
		}
		
		public override string Description
		{
			get { return Catalog.GetString ("Uploads the image to ImageShack."); }
		}
		
		public override string Icon 
		{
			get { return "imageshack.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (IFileItem)
				};
			}
		}
			
		public override bool SupportsItem (IItem item)
		{	
			if (item is FileItem) {	
				if ((item as FileItem).MimeType.StartsWith ("image/")) {
					return true;
				}
			}
			return false;
		}
			
		public Gtk.Bin GetConfiguration ()
		{
			return new ImageShackConfig();
		}
		
		public override IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modifierItems)
		{								
			try {
				List<IItem> returnItems = new List<IItem> ();	
				StringBuilder notificationUrlList = new StringBuilder();
				notificationUrlList.AppendLine (Catalog.GetString ("Image urls: "));
				foreach (IItem item in items) {	
					FileItem fileItem = item as FileItem;
						
					string fileValidationError = ValidateFileForUpload (fileItem.Path);	
					if (fileValidationError != null) {
						returnItems.Add (new TextItem (fileValidationError + " - " + fileItem.Path ));
						continue;
					}
					
					NotificationBridge.ShowMessage (
						    Catalog.GetString ("ImageShack"), 
						    Catalog.GetString ("Do is uploading your image... Please wait a moment..."), 
						    fileItem.Path);
					string url = PostToImageShack (fileItem.Path, fileItem.MimeType);
					notificationUrlList.AppendLine (url);
					returnItems.Add (new TextItem (url));
				}
				
				/*
				NotificationBridge.ShowMessage (
					Catalog.GetString ("ImageShack"), 
					notificationUrlList.ToString ());                               
				*/
					
				return returnItems.ToArray ();
			}
			catch (Exception e) {
				Console.Error.WriteLine (Catalog.GetString ("ImageShack exception: ") + e.Message);
				return new IItem[] { new TextItem (Catalog.GetString ("An error occured while uploading to ImageShack.")) };
			}					
		}		
			
		private static string ValidateFileForUpload (string file)
		{
			FileInfo fi = new FileInfo(file);
			long fileSize = fi.Length;	
			// 1.5MB limit
			if (fileSize > 1572864) {
				return Catalog.GetString ("File size exceeds ImageShack's 1.5MB limit.");
			}
				
			return null;
		}
			
		private static string PostToImageShack (string file, string contentType)
		{		
			string boundary = "----------" + DateTime.Now.Ticks.ToString ("x");
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create ("http://www.imageshack.us/index.php");
			request.Method = "POST";
			request.ContentType = "multipart/form-data ; boundary=" + boundary;
			
			if (!String.IsNullOrEmpty (ImageShackConfig.RegistrationCode)) {
				request.CookieContainer = new CookieContainer ();
				Cookie imageshackCookie = new Cookie ("myimages", ImageShackConfig.RegistrationCode, "/", ".imageshack.us");
				request.CookieContainer.Add (imageshackCookie);
			}

            StringBuilder sb = new StringBuilder ();
            sb.Append ("--");
            sb.Append (boundary);
            sb.Append ("\r\n");
            sb.Append ("Content-Disposition: form-data; name=\"");
            sb.Append ("fileupload");
            sb.Append ("\"; filename=\"");
            sb.Append (Path.GetFileName(file));
            sb.Append ("\"");
            sb.Append ("\r\n");
            sb.Append ("Content-Type: ");
            sb.Append (contentType);
            sb.Append ("\r\n");
            sb.Append ("\r\n");

            string header = sb.ToString ();
            byte[] headerBytes = Encoding.UTF8.GetBytes (header);
            byte[] boundaryBytes = Encoding.ASCII.GetBytes ("\r\n--" + boundary + "\r\n");
            using (FileStream fileStream = new FileStream (file, FileMode.Open, FileAccess.Read)) {
	            long length = headerBytes.Length + fileStream.Length + boundaryBytes.Length;
	            request.ContentLength = length;				
					
				using (Stream requestStream = request.GetRequestStream ()) {
					requestStream.Write (headerBytes, 0, headerBytes.Length);
					byte[] buffer = new Byte[checked((uint)Math.Min (4096, (int)fileStream.Length))];
					int bytesRead = 0;
					while ((bytesRead = fileStream.Read (buffer, 0, buffer.Length)) != 0) {
						requestStream.Write (buffer, 0, bytesRead);
					}
					requestStream.Write (boundaryBytes, 0, boundaryBytes.Length);
				}
			}
			
		    string responseText = "";
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse ()) {			
				using (Stream responseStream = response.GetResponseStream ()) {
					using (StreamReader reader = new StreamReader (responseStream)) {
						responseText = reader.ReadToEnd ();
					}
				}
			}
						
			return GetUrlFromResponseText (responseText);
		}
			
		private static string GetUrlFromResponseText (string responseText) 
		{
			Regex directUrlPattern = new Regex ("<input type=\"text\" onClick=\"track\\('direct'\\).*? value=\"(.*?)\"/>"); 
			Match directUrl = directUrlPattern.Match (responseText);
				
			string url = directUrl.Groups[1].Value;
				
			if (url == string.Empty) {
				throw new Exception (Catalog.GetString ("Parsed url was empty. ImageShack has probably changed its format."));
			}
				
			return url;
		}
	}
}


