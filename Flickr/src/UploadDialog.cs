using System;
using Mono.Unix;

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
			
			Pixbuf FlickrPix = new Pixbuf ("flickr.png@" + GetType ().Assembly.FullName);
			FlickrImage.Pixbuf = FlickrPix.ScaleSimple (50, 50, Gdk.InterpType.Bilinear);
			
			TextLabel.Text = Catalog.GetString ("Your images are being uploaded to Flickr.");
			
			HideButton.Clicked += OnHide;
			uploadProgress.Text = Catalog.GetString (string.Format ("Uploading {0} of {1}...", 
			                                                        this.CurrentUpload, 
			                                                        this.TotalUploads)
			                                         );
		}
		
		public int CurrentUpload { get; set; }
		public int TotalUploads {get; set; }
		
		protected void OnHide (object sender, EventArgs args)
		{
			Services.Notifications.Notify ("Flickr",
			                               String.Format ("Your images are still being uploaded."),
			                               "flickr.png@" + GetType ().Assembly.FullName
			                               );
		}
		
		public void IncrementProgress ()
		{
			this.CurrentUpload++;
			uploadProgress.Fraction = (double)CurrentUpload / (double)TotalUploads;
			uploadProgress.Text = Catalog.GetString (string.Format ("Uploading {0} of {1}...", 
			                                                        this.CurrentUpload, 
			                                                        this.TotalUploads)
			                                         );
		}
	}
}
