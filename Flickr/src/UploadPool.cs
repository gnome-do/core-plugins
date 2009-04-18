// UploadPool.cs
// 
// Copyright (C) 2008 Chris Szikszoy
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Do.Platform;
using Do.Universe;

using FlickrNet;

namespace Flickr
{
	
	public class UploadPool : IDisposable 
	{
		object locker;
		const int WorkerCount = 4;
		
		Thread[] uploaders;
		UploadDialog dialog;
		Queue<IFileItem> taskQ;
		FlickrNet.Flickr flickr;
		
		public UploadPool (string tags) 
		{
			UploadTags = tags;
			locker = new object ();
			taskQ = new Queue<IFileItem> ();
			uploaders = new Thread [WorkerCount];
			flickr = new FlickrNet.Flickr (AccountConfig.ApiKey, AccountConfig.ApiSecret, AccountConfig.AuthToken);
		}
		
		public void EnqueueUpload (IFileItem file)
		{
			lock (locker)
			{
				taskQ.Enqueue (file);
				Monitor.PulseAll (locker);
			}
		}
		
		public void BeginUploads ()
		{
			Log.Debug ("Queue has {0} items", QueueLength);
			dialog = new UploadDialog ();
			dialog.TotalUploads = QueueLength;
			
			// Create and start a separate thread for each worker
			for (int i = 0; i < WorkerCount; i++) {
				uploaders [i] = new Thread (Consume);
				uploaders [i].IsBackground = true;
				uploaders [i].Start();
			}
			
			dialog.Show ();
		}
		
		public string UploadTags { get; private set; }
		
		public int QueueLength { 
			get { return taskQ.Count; } 
		}

		void Consume() 
		{
			IFileItem photo;
			
			do {
				lock (locker) {
					while (taskQ.Count == 0)
						Monitor.Wait (locker);
		
					photo = taskQ.Dequeue();
				}
				
				Thread.Sleep (4000);
				
				if (dialog != null)
					Services.Application.RunOnMainThread ( () => dialog.IncrementProgress ());
				
				try {
					flickr.UploadPicture (photo.Path, photo.Name, "", UploadTags, AccountConfig.IsPublic, AccountConfig.FamilyAllowed,
						AccountConfig.FriendsAllowed);
				} catch (FlickrApiException e) {
					Log.Error ("Cannot upload photos, please grant permissions in configuration dialog");
				}
				                      
			} while (photo != null);
		}
		
#region IDisposable
		public void Dispose() 
		{
			// Enqueue one null task per worker to make each exit.
			foreach (Thread uploader in uploaders)
				EnqueueUpload (null);
			foreach (Thread uploader in uploaders)
				uploader.Join();
			
			Services.Notifications.Notify ("Flickr",
			                               String.Format ("Finished uploading pictures."),
			                               "flickr.png@" + GetType ().Assembly.FullName
			                               );
		}
#endregion
	}
}