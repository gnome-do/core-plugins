using System;
using Mono.Unix;

using Gtk;
using Gdk;
using GLib;

using Do.Platform;

namespace Flickr
{
	
	
	public partial class UploadDialog : Gtk.Dialog
	{

		readonly string UploadingLabel;
		
		public UploadDialog()
		{
			Build();
			
			CurrentUpload = 0;
			UploadingLabel = Catalog.GetString ("Uploading {0} of {1}...");
		
			/*Pixbuf FlickrPix = Pixbuf.LoadFromResource ("flickr@" + GetType ().Assembly.FullName);
			FlickrImage.Pixbuf = FlickrPix.ScaleSimple (50, 50, Gdk.InterpType.Bilinear);*/
			
			TextLabel.Text = Catalog.GetString ("Your images are being uploaded to Flickr.");
			
			HideButton.Clicked += OnHide;
			uploadProgress.Text = Catalog.GetString (string.Format (UploadingLabel, CurrentUpload, TotalUploads));
			GLib.Timeout.Add (100, () => { uploadProgress.Pulse (); return true; });
		}
		
		public int TotalUploads {get; set; }
		public int CurrentUpload { get; set; }
		
		protected void OnHide (object sender, EventArgs args)
		{
			Services.Notifications.Notify ("Flickr", String.Format ("Your images are still being uploaded."),
				"flickr.png@" + GetType ().Assembly.FullName);
			Destroy ();
		}
		
		public void IncrementProgress ()
		{
			CurrentUpload++;
			uploadProgress.Text = Catalog.GetString (string.Format (UploadingLabel, CurrentUpload, TotalUploads));
			
			if (CurrentUpload == TotalUploads)
				Destroy ();
		}
	}
}
