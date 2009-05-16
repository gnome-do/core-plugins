using System;
using Mono.Unix;

using Gtk;
using Gdk;

using Do.Platform;

namespace Flickr
{
	
	
	public partial class UploadDialog : Gtk.Dialog
	{
		readonly string ProgressLabel;
		readonly string UploadingLabel;
		readonly string ContinuationText;
		readonly string FinishedUploadLabel;
		readonly string FinishedProgressLabel;
		
		public UploadDialog()
		{
			Build();
			
			HideButton.Clicked += (sender, args) => Destroy ();
			OKButton.Clicked += (sender, args) => Destroy ();
			this.Destroyed += OnDestroy;
			OKButton.Hide ();
			
			this.CurrentUpload = 0;
			this.IsDestroyed = false;
			UploadingLabel = Catalog.GetString ("Uploading {0}...");
			ProgressLabel = Catalog.GetString ("Uploading {0} of {1}...");
			FinishedUploadLabel = Catalog.GetString ("Finished uploading images to Flickr.");
			FinishedProgressLabel = Catalog.GetString ("Uploaded {0} images");
			ContinuationText = Catalog.GetString ("Your images are still being uploaded.");
		
			using (Pixbuf FlickrPix = Pixbuf.LoadFromResource ("flickr.png"))
				FlickrImage.Pixbuf = FlickrPix.ScaleSimple (75, 75, Gdk.InterpType.Bilinear);
			
			TextLabel.Text = Catalog.GetString ("Your images are being uploaded to Flickr.");
			uploadProgress.Text = Catalog.GetString (string.Format (ProgressLabel, CurrentUpload, TotalUploads));
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
		
		public void IncrementProgress (string text)
		{
			CurrentUpload++;
			
			uploadProgress.Text = string.Format (ProgressLabel, CurrentUpload, TotalUploads);
			TextLabel.Text = string.Format (UploadingLabel, text);
			
			uploadProgress.Fraction = Math.Min ((double) CurrentUpload / (double) TotalUploads, 1.0f);
		}
		
		private void ShowDialog (string text)
		{
			Services.Notifications.Notify ("Flickr", text, "flickr.png@" + GetType ().Assembly.FullName);
		}
		
		public void Finish ()
		{
			if (this.IsDestroyed)
				ShowDialog (string.Format (FinishedUploadLabel, TotalUploads));
			HideButton.Visible = false;
			OKButton.Visible = true;
			uploadProgress.Text = string.Format (FinishedProgressLabel, TotalUploads);
			TextLabel.Text = FinishedUploadLabel;
		}
	}
}