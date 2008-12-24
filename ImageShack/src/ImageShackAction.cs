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
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Mono.Unix;

using Do.Platform;
using Do.Platform.Linux;
using Do.Universe;
using Do.Universe.Common;

namespace ImageShack
{
	public class ImageShackAction : Act, IConfigurable
	{
		Dictionary<string, string> image_mime_type_mapping;
			
		public ImageShackAction()
		{
			image_mime_type_mapping = new Dictionary<string,string>
			{
				{".jpg", "image/jpeg"},
				{".jpeg", "image/jpeg"},
				{".png", "image/png"}, 
				{".gif", "image/gif"}, 
				{".bmp", "image/bmp"}, 
				{".tif", "image/tiff"}, 
				{".tiff", "image/tiff"}
			};
		}
		
		public override string Name {
			get { return Catalog.GetString ("Upload to ImageShack"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Uploads the image to ImageShack."); }
		}
		
		public override string Icon {
			get { return "imageshack.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}
			
		public override bool SupportsItem (Item item)
		{	
			if (item is IFileItem) {
				return IsImageFile((item as IFileItem));
			}
			return false;
		}
			
		public Gtk.Bin GetConfiguration ()
		{
			return new ImageShackConfig();
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{								
			try {
				List<Item> imageUrls = new List<Item> ();	

				foreach (Item item in items) {	
					IFileItem imageFile = item as IFileItem;
						
					if (!FileIsValidForUpload (imageFile.Path)) continue;
					    
					UploadNotification notification = new UploadNotification(imageFile.Path);
					Services.Notifications.Notify (notification);
					
					string mimeType = image_mime_type_mapping[Path.GetExtension (imageFile.Path)];
					string url = PostToImageShack (imageFile.Path, mimeType);
					imageUrls.Add (new TextItem (url));
				}
				
				return imageUrls;
			}
			catch (Exception e) {
				Console.Error.WriteLine (Catalog.GetString ("ImageShack exception: ") + e.Message);			
				GeneralErrorNotification notification = new GeneralErrorNotification();
				Services.Notifications.Notify (notification);				
			}				
			
			return null;
		}		
			
		private static bool FileIsValidForUpload (string file)
		{
			string fileSizeError = Catalog.GetString ("File size exceeds ImageShack's 1.5MB limit.");
			
			FileInfo fi = new FileInfo(file);
			long fileSize = fi.Length;	
			// 1.5MB limit
			if (fileSize > 1572864) {
				InvalidFileNotification notification = new InvalidFileNotification (fileSizeError);
				Services.Notifications.Notify (notification);			
				return false;
			}
				
			return true;
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
		
		private bool IsImageFile (IFileItem file)
		{
			return image_mime_type_mapping.ContainsKey (Path.GetExtension (file.Path));
		}
	}
}


