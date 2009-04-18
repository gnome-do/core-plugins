using System;
using Mono.Unix;

using Gtk;
using Gdk;

using Do.Platform;

namespace Flickr
{
	
	
	public partial class UploadDialog : Gtk.Dialog
	{

		public UploadDialog()
		{
			this.Build();
			
			this.CurrentUpload = 0;
		
			/*Pixbuf FlickrPix = Pixbuf.LoadFromResource ("flickr@" + GetType ().Assembly.FullName);
			FlickrImage.Pixbuf = FlickrPix.ScaleSimple (50, 50, Gdk.InterpType.Bilinear);*/
			
			TextLabel.Text = Catalog.GetString ("Your images are being uploaded to Flickr.");
			
			HideButton.Clicked += OnHide;
			uploadProgress.Text = Catalog.GetString (string.Format ("Uploading {0} of {1}...", this.CurrentUpload, this.TotalUploads));
		}
		
		public int CurrentUpload { get; set; }
		public int TotalUploads {get; set; }
		
		protected void OnHide (object sender, EventArgs args)
		{
			Services.Notifications.Notify ("Flickr", String.Format ("Your images are still being uploaded."),
				"flickr.png@" + GetType ().Assembly.FullName);
			this.Destroy ();
		}
		
		public void IncrementProgress ()
		{
			Log.Debug ("Uploading {0} of {1}", CurrentUpload, TotalUploads);
			CurrentUpload++;
			uploadProgress.Fraction = (double)CurrentUpload / (double)TotalUploads;
			uploadProgress.Text = Catalog.GetString (string.Format ("Uploading {0} of {1}...", CurrentUpload, TotalUploads));
			
			if (CurrentUpload == TotalUploads)
				this.Destroy ();
		}
	}
}
