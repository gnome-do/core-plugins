/* UploadAction.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
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
using System.Threading;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;
using Do.Addins;

using FlickrNet;

namespace Flickr
{	
	public class UploadAction : IAction, IConfigurable
	{
		static object count_lock = new object ();
		static int upload_num;
		
		public string Name {
			get { return Catalog.GetString ("Upload photo"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Upload one or more photos to Flickr"); }
		}
		
		/*
		 * Thank you Jeremy Roux for the great icon
		 * http://www.soulvisual.com/blog/
		 */
		public string Icon {
			get { return "flickr.png@" + GetType ().Assembly.FullName; }
		}
		
		public Type [] SupportedItemTypes {
			get { 
				return new Type [] { typeof (IFileItem) };
			}
		}
		
		public Type [] SupportedModifierItemTypes {
			get {
				return new Type [] { typeof (ITextItem) };
			}
		}
		
		public bool ModifierItemsOptional {
			get { return true; }
		}
		
		public bool SupportsItem (IItem item)
		{
			return FileItem.IsDirectory (item as FileItem) || 
				FileIsPicture (item as FileItem);
		}
		
		public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return true;
		}
		
		public IItem[] DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}
		
		public IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			string tags;
			tags = AccountConfig.Tags + " ";
			
			if (modItems.Length > 0) {
				foreach (IItem modItem in modItems) {
					ITextItem tag = (modItem as ITextItem);
					tags += tag.Text + " ";
				}
			}
			
			//Build a list of all of the files to upload.
			List<FileItem> uploads = new List<FileItem> ();
			foreach (IItem item in items) {
				FileItem file = item as FileItem;
				if (FileItem.IsDirectory (file)) {
					DirectoryInfo dinfo = new DirectoryInfo (file.Path);
					FileInfo [] finfo = dinfo.GetFiles ();
					foreach (FileInfo f in finfo) {
						FileItem fi = new FileItem (f.FullName);
						if (FileIsPicture (fi))
							uploads.Add (fi);
					}
				} else {
					uploads.Add (file);
				}
				upload_num = 1;
				foreach (FileItem photo in uploads) {
					AsyncUploadToFlickr (photo, tags, uploads.Count);
				}
			}
			
			return null;
		}
		
		public static void AsyncUploadToFlickr (FileItem photo, string tags, int num)
		{			
			FlickrNet.Flickr flickr = new FlickrNet.Flickr (AccountConfig.ApiKey,
				AccountConfig.ApiSecret, AccountConfig.AuthToken);
				
			new Thread ((ThreadStart) delegate {
				try {
					int thisUpload;
					
					flickr.UploadPicture (photo.Path, photo.Name, "", tags,
						AccountConfig.IsPublic, AccountConfig.FamilyAllowed,
						AccountConfig.FriendsAllowed);
					
					lock (count_lock) {
						thisUpload = upload_num;
						upload_num++;
					}
					
					Do.Addins.NotificationBridge.ShowMessage ("Flickr",
						String.Format ("Uploaded {0}. ({1} of {2})", photo.Name,
						thisUpload, num), photo.Path); 
				} catch (FlickrNet.FlickrException e) {
					Console.Error.WriteLine (e.Message);
				}
			}).Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new UploadConfig ();
		}
		
		private bool FileIsPicture (FileItem item)
		{
			return item.MimeType.StartsWith ("image/");
		}
	}
}
