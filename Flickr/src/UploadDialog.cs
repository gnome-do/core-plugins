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
		readonly string FinishUploadLabel;
		readonly string ContinuationText;
		
		public UploadDialog()
		{
			Build();
			
			HideButton.Clicked += (sender, args) => Destroy ();
			OKButton.Clicked += (sender, args) => Destroy ();
			this.Destroyed += OnDestroy;
			OKButton.Hide ();
			
			this.CurrentUpload = 0;
			this.IsDestroyed = false;
			UploadingLabel = Catalog.GetString ("Uploaded {0} of {1}...");
			FinishUploadLabel = Catalog.GetString ("Finished uploading {0} images to Flickr.");
			ContinuationText = Catalog.GetString ("Your images are still being uploaded.");
		
			using (Pixbuf FlickrPix = Pixbuf.LoadFromResource ("flickr.png"))
				FlickrImage.Pixbuf = FlickrPix.ScaleSimple (75, 75, Gdk.InterpType.Bilinear);
			
			TextLabel.Text = Catalog.GetString ("Your images are being uploaded to Flickr.");
			uploadProgress.Text = Catalog.GetString (string.Format (UploadingLabel, CurrentUpload, TotalUploads));
		}
		
		public int TotalUploads {get; set; }
		public int CurrentUpload { get; set; }
		private bool IsDestroyed {get; set; }
		
		protected void OnDestroy (object sender, EventArgs args)
		{
			this.IsDestroyed = true;
			if (CurrentUpload < TotalUploads)
				ShowDialog (ContinuationText);
		}
		
		public void IncrementProgress ()
		{
			CurrentUpload++;
			uploadProgress.Text = string.Format (UploadingLabel, CurrentUpload, TotalUploads);
			
			uploadProgress.Fraction = Math.Min ((double) CurrentUpload / (double) TotalUploads, 1.0f);
		}
		
		private void ShowDialog (string text)
		{
			Services.Notifications.Notify ("Flickr", text, "flickr.png@" + GetType ().Assembly.FullName);
		}
		
		public void Finish ()
		{
			if (this.IsDestroyed)
				ShowDialog (string.Format (FinishUploadLabel, TotalUploads));
			HideButton.Visible = false;
			OKButton.Visible = true;
			uploadProgress.Text = "";
			TextLabel.Text = string.Format (FinishUploadLabel, TotalUploads);
		}
	}
}