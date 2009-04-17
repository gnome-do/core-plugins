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
		object locker = new object();
		const int workerCount = 4;
		Thread[] uploaders;
		Queue<IFileItem> taskQ = new Queue<IFileItem>();
		UploadDialog dialog;
		FlickrNet.Flickr flickr;

		public UploadPool (string tags, int totalUploads) 
		{
			uploaders = new Thread [workerCount];
			UploadTags = tags;
			
			flickr = new FlickrNet.Flickr (AccountConfig.ApiKey,
			                               AccountConfig.ApiSecret, 
			                               AccountConfig.AuthToken
			                               );
			
			
				dialog = new UploadDialog ();
				dialog.TotalUploads = totalUploads;
			Services.Application.RunOnMainThread ( () => {
				dialog.Show ();
			});
		}
		
		public void BeginUploads ()
		{
			// Create and start a separate thread for each worker
			for (int i = 0; i < workerCount; i++)
				(uploaders [i] = new Thread (Consume)).Start();
		}

		public void Dispose() 
		{
			// Enqueue one null task per worker to make each exit.
			foreach (Thread uploader in uploaders)
				QueueUpload (null);
			foreach (Thread uploader in uploaders)
				uploader.Join();
			
			Services.Notifications.Notify ("Flickr",
			                               String.Format ("Finished uploading pictures."),
			                               "flickr.png@" + GetType ().Assembly.FullName
			                               );
		}
		
		public string UploadTags { get; private set; }
		public int UploadsInQueue {get { return taskQ.Count; } }
		
		public void QueueUpload (IFileItem file)
		{
			lock (locker)
			{
				taskQ.Enqueue (file);
				Monitor.PulseAll (locker);
			}
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
				if (photo != null) {
					Thread.Sleep (4000);
					Services.Application.RunOnMainThread ( () => dialog.IncrementProgress ());
					/*
					flickr.UploadPicture (photo.Path, 
					                      photo.Name, 
					                      "", 
					                      UploadTags,
					                      AccountConfig.IsPublic, 
					                      AccountConfig.FamilyAllowed,
					                      AccountConfig.FriendsAllowed
					                      );
					 */
				}
			} while (photo != null);
		}
	}
}