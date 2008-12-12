/*
 * Microblog.cs
 * 
 * GNOME Do is the legal property of its developers, whose names are too numerous
 * to list here.  Please refer to the COPYRIGHT file distributed with this
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

using Do.Platform;
using Do.Universe;

using Twitterizer.Framework;

namespace Microblogging
{
	public static class Microblog
	{
		static readonly string NotifyFail = Catalog.GetString ("Post failed");
		static readonly string NotifySuccess = Catalog.GetString ("Post Successful");
		static readonly string ContentPosted = Catlog.GetString ("'{0}' successfully posted.");
		static readonly string DownloadFailed = Catalog.Getstring ("Failed to fetch file from {0}");
		static readonly string NoUpdates = Catalog.GetString ("No new microblog status updates found.");
		static readonly string GenericError = Catalog.GetString ("Twitter encountered an error in {0}");
		static readonly string MissingCredentials = Catalog.GetString ("Missing login credentials. Please set login "
			+ "information in plugin configuration.");
		static readonly string FailedPost = Catalog.GetString ("Unable to post tweet. Check your login settings. If you "
			+ "are behind a proxy make sure that the settings in /system/http_proxy are correct.");
		
		const string ErrorIcon = "dialog-warning";
		
		static List<IItem> friends;
		static object friends_lock;
		
		static DateTime last_updated;
		static MicroblogPreferences prefs;
		static string username, password;
		static Twitter twitter;
		
		static readonly string  PhotoDirectory;
		
		static Microblog ()
		{
			friends = new List<IItem> ();
			friends_lock = new object ();
			prefs = new MicroblogPreferences ();

			GenConfig.ServiceChanged += ServiceChanged;
			
			// TODO: this will need updated when Secure preferences are implemented
			Configuration.GetAccountData (out username, out password, typeof (Configuration));
			
			if (string.IsNullOrEmpty (username) || string.IsNullOrEmpty (password)) {
				Log.Debug (MissingCredentials);
				username = password = "";
			}
			
			PhotoDirectory = new [] {Paths.UserData, "Microblogging", "photos"}.Aggregate (Path.Combine);
			
			Connect (username, password);
			last_updated = DateTime.UtcNow;
		}

		public static bool Connect (string username, string password)
		{
			Microblog.username = username;
			Microblog.password = password;
			
			twitter = new Twitter (username, password, ActiveService);
			return true;
		}
			
		public static IEnumerable<IItem> Friends {
			get { return friends; }
		}
		
		internal static MicroblogPreferences Preferences {
			get { return prefs; }
		}
		
		public static Service ActiveService {
			get { return (Service) Enum.Parse (typeof (Service), Preferences.MicroblogService, true); }
		}
		
		public static string ContactProperty {
			get { return "microblog.screenname"; }
		}
		
		public static void Post (object status)
		{
			string content;
			
			if (twitter == null) {
				Notifications.Notify (NotifyFail, MissingCredentials, ErrorIcon);					
				Log.Debug (MissingCredentials);
				
				return;
			}
			
			content = status.ToString ();
					
			try {
				twitter.Status.Update (content);
				Notifications.Notify (NotifySuccess, String.Format (ContentPosted, content));
			} catch (TwitterizerException e) {
				Notifications.Notify (NotifyFail, FailedPost, ErrorIcon);
					
				Log.Debug (string.Format (GenericError, "Post"), e.Message, e.StackTrace);
			}
		}
		
		public static void UpdateFriends ()
		{
			ContactItem tfriend;
			TwitterUserCollection myFriends;
			
			if (twitter == null) {
				Log.Debug (MissingCredentials);
				return;
			}
			
			myFriends = null;
			
			try {
				myFriends = twitter.User.Friends (); 
			} catch (TwitterizerException e) {
				Log.Debug (string.Format (GenericError, "UpdateFriends"), e.Message, e.StackTrace);
				return;
			}
			
			lock (friends_lock) {
				foreach (TwitterUser friend in myFriends) {
					tfriend = ContactItem.Create (friend.ScreenName);
					tfriend[ContactProperty] = friend.ScreenName;
					
					if (System.IO.File.Exists (Paths.Combine (PhotoDirectory, "" + friend.ID)))
						tfriend ["photo"] = Paths.Combine (PhotoDirectory,  "" + friend.ID);
					else
						DownloadBuddyIcon (friend.ProfileImageUri, friend.ID);
					
					friends.Add (tfriend);
				}
			}
		}
		
		public static void UpdateTimeline ()
		{
			int userId;
			Uri imageUri;
			string text, screenname;
			TwitterStatus tweet;
			
			if (!Preferences.ShowNotifications) return;
			
			try {
				 tweet = twitter.Status.FriendsTimeline () [0];
			} catch (TwitterizerException e) {
				Log.Debug (string.Format (GenericError, "UpdateTimeline"), e.Message, e.StackTrace);
				return;
			} catch (IndexOutOfRangeException) {
				Log.Info (NoUpdates);
				return;
			}
				
			if (tweet.TwitterUser.ScreenName.Equals (username)) return;
			if (DateTime.Compare (tweet.Created, last_updated) <= 0) return;
			
			text = tweet.Text;
			userId = tweet.TwitterUser.ID;
			imageUri = tweet.TwitterUser.ProfileImageUri;
			screenname = tweet.TwitterUser.ScreenName;
			
			if (System.IO.File.Exists (Path.Combine (PhotoDirectory, "" + userId)))
				Notifications.Notify (screenname, text, Path.Combine (PhotoDirectory, "" + userId));
			else {
				Notifications.Notify (screenname, text);
				DownloadBuddyIcon (imageUri, userId);
			}
			
			last_updated = tweet.Created;		
		}

		static void ServiceChanged (object o, EventArgs e)
		{
			Connect (username, password);
		}
				
		static void DownloadBuddyIcon (Uri imageUri, int userId)
		{
			WebClient client;
			string imageDestination;
			
			client = new WebClient ();
			imageDestination = Path.Combine (PhotoDirectory, "" + userId);
			
			if (!System.IO.Directory.Exists (PhotoDirectory))
				System.IO.Directory.CreateDirectory (PhotoDirectory);
				
			try {
				client.DownloadFile (imageUri.AbsoluteUri, imageDestination);
			} catch (Exception e) {
				Log.Debug (string.Format (DownloadFailed, imageUri.AbsoluteUri), e.Message, e.StackTrace);
			}
		}
	}
}
