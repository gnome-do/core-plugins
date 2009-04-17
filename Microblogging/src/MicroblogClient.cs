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

		const int UpdateTimelineTimeout = 60 * 1000; // every 60 seconds
		const int UpdateContactsTimeout = 30 * 1000 * 60; // every 30 minutes
		const int CheckForMessagesTimeout = 5 * 1000 * 60; // every 5 minutes

		#endregion

		Twitter blog;
		string username;
		DateTime timeline_last_updated, messages_last_updated;

		Timer [] timers;

		public IEnumerable<FriendItem> Contacts { get; private set; }
		
		static MicroblogClient ()
		{
			PhotoDirectory = new [] { Services.Paths.UserDataDirectory, "Microblogging", "photos"}.Aggregate (Path.Combine);
		}
		
		public MicroblogClient (string username, string password, Service service)
		{
			timers = new Timer [3];
			this.username = username;
			Contacts = Enumerable.Empty<FriendItem> ();
			blog = new Twitter (username, password, service);
			timeline_last_updated = messages_last_updated = DateTime.UtcNow;

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
		
		public static readonly string PhotoDirectory;

		/// <summary>
		/// Update your miroblogging status
		/// </summary>
		/// <param name="status">
		/// A <see cref="System.String"/> status message
		/// </param>
		public void UpdateStatus (string status)
		{
			UpdateStatus (status, null);
		}
		
		public void UpdateStatus (string status, Nullable<int> inReplyToID)
		{
			string errorMessage = "";
			try {
				blog.Status.Update (status, inReplyToID);
			} catch (TwitterizerException e) {
				errorMessage = FailedPostMsg;
				Log<MicroblogClient>.Debug (GenericErrorMsg, "Post", e.Message);
			}

			OnStatusUpdated (status, errorMessage);
		}

		void UpdateContacts (object o)
		{
			FriendItem newContact;
			MicroblogStatus status;
			List<FriendItem> newContacts;
			TwitterUserCollection friends;
			
			try {
				newContacts = new List<FriendItem> ();
				friends = blog.User.Friends ();
			} catch (TwitterizerException e) {
				Log.Error("{0} {1}", e.RequestData.ResponseException.Message, e.RequestData.ResponseException.StackTrace);
				Log<MicroblogClient>.Debug (GenericErrorMsg, "UpdateContacts", e.Message);
				return;
			}
			
			foreach (TwitterUser friend in friends) {
				if (friend.Status != null) {
					status = new MicroblogStatus (friend.Status.ID, friend.Status.Text, friend.ScreenName, friend.Status.Created);
					newContact = new FriendItem (friend.ID, friend.ScreenName, status);
				} else {
					newContact = new FriendItem (friend.ID, friend.ScreenName);
				}			
				
				newContacts.Add (newContact);
			}
			
			Contacts = newContacts;
			Log<MicroblogClient>.Debug ("Found {0} contacts", Contacts.Count ());
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

				if (tweet.TwitterUser.ScreenName.Equals (username) || tweet.Created <= timeline_last_updated)
					return;
			
				icon = FindIconForUser (tweet.TwitterUser);
				timeline_last_updated = tweet.Created;
				
				OnTimelineUpdated (tweet.TwitterUser.ScreenName, tweet.Text, icon);

				Contacts.Where (contact => contact.Id == tweet.TwitterUser.ID)
					.First ()
					.AddStatus (new MicroblogStatus (tweet.ID, tweet.Text, tweet.TwitterUser.ScreenName, tweet.Created));
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

				messages_last_updated = message.Created;
				icon = FindIconForUser (message.TwitterUser);

				OnMessageFound (message.TwitterUser.ScreenName, message.Text, icon);
			} catch (Exception e) {
				Log<MicroblogClient>.Debug (GenericErrorMsg, "CheckForMessages", e.Message);
			}
		}
		
		string FindIconForUser (TwitterUser user)
		{
			string icon = "";			
			
			if (File.Exists (Path.Combine (PhotoDirectory, "" + user.ID)))
				icon = Path.Combine (PhotoDirectory, "" + user.ID);
			else
				DownloadBuddyIcon (user);

			return icon;
		}

		void DownloadBuddyIcon (TwitterUser user)
		{
			string imageDestination;
			imageDestination = Path.Combine (PhotoDirectory, "" + user.ID);
			
			if (File.Exists (imageDestination)) return;
			if (!Directory.Exists (PhotoDirectory)) Directory.CreateDirectory (PhotoDirectory);
			
			using (WebClient client = new WebClient ()) 
			{
				try {
					client.DownloadFile (user.ProfileImageUri.AbsoluteUri, imageDestination);
				} catch (Exception e) {
					Log.Error (string.Format (DownloadFailedMsg, user.ProfileImageUri.AbsoluteUri), e.Message);
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
		public event TimelineUpdatedDelegate MessageFound;
		public event TimelineUpdatedDelegate TimelineUpdated;
		
		public delegate void StatusUpdatedDelegate (object sender, StatusUpdatedEventArgs args);
		public delegate void TimelineUpdatedDelegate (object sender, TimelineUpdatedEventArgs args);
	}
}
