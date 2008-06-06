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

using Do.Universe;
using Do.Addins;

using FlickrNet;
using GConf;

namespace Flickr
{	
	public class UploadAction : IAction, IConfigurable
	{
		static string API_KEY = "aa645b69c14422e095dee81dda21385b";
		static string API_SECRET = "a266af1a63024d3d";
		static string GCONF_KEY_TOKEN = "/apps/gnome-do/plugins/flickr/token";
		
		public string Name {
			get { return "Upload photo"; }
		}
		
		public string Description {
			get { return "Upload one or more photos to Flickr"; }
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
			GConf.Client gconf = new GConf.Client ();
			string tags;
			
			//Test to make sure we have a token in gconf
			try {
				gconf.Get (GCONF_KEY_TOKEN);
			} catch (GConf.NoSuchKeyException) {
				Console.Error.WriteLine ("Please authorize in configuration!");
				return null;
			}
			tags = "";
			
			if (modItems.Length > 0) {
				foreach (IItem modItem in modItems) {
					ITextItem tag = (modItem as ITextItem);
					tags += tag.Text + ", ";
				}
			}
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
			}
			AsyncUploadToFlickr (uploads, tags);
			return null;
		}
		
		public static void AsyncUploadToFlickr (List<FileItem> uploads, string tags)
		{
			GConf.Client gconf = new GConf.Client ();
			string token = "";
			try {
				token = gconf.Get (GCONF_KEY_TOKEN) as string;
			} catch (GConf.NoSuchKeyException) {
				return;
			}
			
			FlickrNet.Flickr flickr = new FlickrNet.Flickr (API_KEY, API_SECRET, token);
			new Thread ((ThreadStart) delegate {
				foreach (FileItem file in uploads) {
					flickr.UploadPicture (file.Path, file.Name, "", tags, true, true, true);
				}
			}).Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
		
		private bool FileIsPicture (FileItem item)
		{
			return item.MimeType.StartsWith ("image/");
		}
	}
}