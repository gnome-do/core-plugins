/* MicroblogClient.cs 
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
using System.Net;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Twitterizer.Framework;

using Do.Platform;
using Do.Universe;
	
namespace Microblogging
{

	public class MicroblogClient
	{
		#region Class constants, error messages
		
		readonly string DownloadFailedMsg = Catalog.GetString ("Failed to fetch file from {0}");
		readonly string GenericErrorMsg = Catalog.GetString ("Twitter encountered an error in {0}. {1}");
		
		readonly string FailedPostMsg = Catalog.GetString ("Unable to post tweet. Check your login settings. If you "
			+ "are behind a proxy make sure that the settings in /system/http_proxy are correct.");

		const int UpdateTimelineTimeout = 30 * 1000;
		const int UpdateContactsTimeout = 10 * 60 * 1000;
		const int CheckForMessagesTimeout = 5 * 60 * 1000;

		#endregion

		Twitter blog;
		string username;
		DateTime timeline_last_updated, messages_last_updated;

		Timer [] timers;

		readonly string PhotoDirectory;

		public IEnumerable<Item> Contacts { get; private set; }
		
		public MicroblogClient (string username, string password, Service service)
		{
			timers = new Timer [3];
			this.username = username;
			Contacts = Enumerable.Empty<Item> ();
			blog = new Twitter (username, password, service);
			timeline_last_updated = messages_last_updated = DateTime.UtcNow;
			PhotoDirectory = new [] { Services.Paths.UserDataDirectory, "Microblogging", "photos"}.Aggregate (Path.Combine);

			timers [0] = new Timer (UpdateContacts, null, 1 * 1000, UpdateContactsTimeout);
			timers [1] = new Timer (UpdateTimeline, null, 60 *1000, UpdateTimelineTimeout);
			timers [2] = new Timer (CheckForMessages, null, 120 * 1000, CheckForMessagesTimeout);
		}

		/// <value>
		/// The ContactItem key that contains the user's microblogging screen name
		/// </value>
		public static string ContactKeyName {
			get { return "microblog.screenname"; }
		}

		/// <summary>
		/// Update your miroblogging status
		/// </summary>
		/// <param name="status">
		/// A <see cref="System.String"/> status message
		/// </param>
		public void UpdateStatus (string status)
		{
			string errorMessage = "";
			try {
				blog.Status.Update (status);
				
			} catch (TwitterizerException e) {
				errorMessage = FailedPostMsg;
				Log<MicroblogClient>.Debug (GenericErrorMsg, "Post", e.Message);
			}

			OnStatusUpdated (status, errorMessage);
		}

		void UpdateContacts (object o)
		{
			Log.Debug ("Microblogging: Updating contacts");
			
			ContactItem newContact;
			List<Item> newContacts;
			TwitterUserCollection friends;
			
			try {
				newContacts = new List<Item> ();
				friends = blog.User.Friends ();
			} catch (TwitterizerException e) {
				Log<MicroblogClient>.Debug (GenericErrorMsg, "UpdateContacts", e.Message);
				return;
			}
				
			foreach (TwitterUser friend in friends) {
				newContact = ContactItem.Create (friend.ScreenName);
				newContact[ContactKeyName] = friend.ScreenName;
				
				if (File.Exists (Path.Combine (PhotoDirectory, "" + friend.ID)))
					newContact["photo"] = Path.Combine (PhotoDirectory,  "" + friend.ID);
				else
					DownloadBuddyIcon (friend.ProfileImageUri, friend.ID);
				
				newContacts.Add (newContact);
			}

			Contacts = newContacts;
			Log.Debug ("Microblogging: Found {0} contacts", Contacts.Count ());
			return;
		}

		void UpdateTimeline (object o)
		{
			string icon = "";
			TwitterStatus tweet;
			TwitterParameters parameters;

			try {
				// get the most recent update
				parameters = new TwitterParameters ();
				parameters.Add (TwitterParameterNames.Count, 1);
				tweet = blog.Status.FriendsTimeline (parameters) [0];

				if (tweet.TwitterUser.ScreenName.Equals (username) || tweet.Created <= timeline_last_updated) return;
			
				icon = FindIconForUser (tweet.TwitterUser.ProfileImageUri, tweet.TwitterUser.ID);
				timeline_last_updated = tweet.Created;
			
				OnTimelineUpdated (tweet.TwitterUser.ScreenName, tweet.Text, icon);
			} catch (Exception e) {
				Log<MicroblogClient>.Debug (GenericErrorMsg, "UpdateTimeline", e.Message);
			}
		}

		void CheckForMessages (object o)
		{
			string icon = "";
			TwitterStatus message;
			TwitterParameters parameters;
		
			try {
				// get the most recent update
				parameters = new TwitterParameters ();
				parameters.Add (TwitterParameterNames.Count, 1);
				message = blog.DirectMessages.DirectMessages (parameters) [0];
				
				if (message.Created <= messages_last_updated) return;

				icon = FindIconForUser (message.TwitterUser.ProfileImageUri, message.TwitterUser.ID);
				messages_last_updated = message.Created;

				OnMessageFound (message.TwitterUser.ScreenName, message.Text, icon);
			} catch (Exception e) {
				Log<MicroblogClient>.Debug (GenericErrorMsg, "CheckForMessages", e.Message);
			}
		}
		
		string FindIconForUser (Uri profileUri, int userId)
		{
			string icon = "";			
			
			if (File.Exists (Path.Combine (PhotoDirectory, "" + userId)))
				icon = Path.Combine (PhotoDirectory, "" + userId);
			else
				DownloadBuddyIcon (profileUri, userId);

			return icon;
		}

		void DownloadBuddyIcon (Uri imageUri, int userId)
		{
			string imageDestination;
			imageDestination = Path.Combine (PhotoDirectory, "" + userId);
			if (!Directory.Exists (PhotoDirectory)) Directory.CreateDirectory (PhotoDirectory);
			
			using (WebClient client = new WebClient ()) 
			{
				try {
					client.DownloadFile (imageUri.AbsoluteUri, imageDestination);
				} catch (Exception e) {
					Log.Error (string.Format (DownloadFailedMsg, imageUri.AbsoluteUri), e.Message);
					Log.Debug (e.StackTrace);
				}
			}
		}

		void OnStatusUpdated (string status, string errorMessage)
		{
			if (StatusUpdated != null)
				Gtk.Application.Invoke ((o, e) => StatusUpdated (this, new StatusUpdatedEventArgs (status, errorMessage)));
		}

		void OnTimelineUpdated (string screenname, string status, string icon)
		{
			if (TimelineUpdated != null)
				Gtk.Application.Invoke ((o, e) => TimelineUpdated (this, new TimelineUpdatedEventArgs (screenname, status, icon)));
		}

		void OnMessageFound (string screenname, string status, string icon)
		{
			if (MessageFound != null)
				Gtk.Application.Invoke ((o, e) => MessageFound (this, new TimelineUpdatedEventArgs (screenname, status, icon)));
		}
		
		public event StatusUpdatedDelegate StatusUpdated;
		public event TimelineUpdatedDelegate TimelineUpdated;
		public event TimelineUpdatedDelegate MessageFound;
		
		public delegate void StatusUpdatedDelegate (object sender, StatusUpdatedEventArgs args);
		public delegate void TimelineUpdatedDelegate (object sender, TimelineUpdatedEventArgs args);
	}
}
