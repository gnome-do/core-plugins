/*
 * TwitterFriendsUpdater.cs
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

using LibTwitter = Twitterizer.Framework;

namespace Twitter
{
	public static class Twitter
	{
		const string ErrorIcon = "dialog-warning";
		
		static List<IItem> friends;
		static object friends_lock;
		
		static DateTime lastUpdated;
		static TwitterPreferences prefs;
		static string username, password;
		static LibTwitter.Twitter twitter;
		static Dictionary<string, int> availableServices;
		
		static readonly string  PhotoDirectory;
		
		static Twitter ()
		{
			friends = new List<IItem> ();
			friends_lock = new object ();
			prefs = new TwitterPreferences ();
			
			// this will need updated when Secure preferences are implemented
			Configuration.GetAccountData (out username, out password, typeof (Configuration));
				
			Connect (username, password);
			lastUpdated = DateTime.UtcNow;
			
			PhotoDirectory = new string[] {"Twitter", "photos"}.Aggregate (Path.Combine);
			SetupAvailableServices ();
		}
				
		public static Dictionary<string, int> AvailableServices {
			get { return availableServices; }
		}
				
		public static bool Connect (string username, string password)
		{
			return Connect (username, password, Preferences.MicroblogService);
		}
		
		public static bool Connect (string username, string password, string service)
		{
			int serv;
			
			serv = availableServices.TryGetValue (service, out serv) ? serv : availableServices[Preferences.MicroblogService];
			
			twitter = new LibTwitter.Twitter (username, password, (LibTwitter.Service) serv);
			return true;
		}
		
		public static void ChangeService (string service)
		{
			Connect (username, password, service);
		}
		
		public static IEnumerable<IItem> Friends {
			get { return friends; }
		}
		
		public static TwitterPreferences Preferences {
			get { return prefs ?? prefs = new TwitterPreferences (); }
		}
		
		public static void Tweet (object status)
		{
			string content;
			
			if (twitter == null) {
				Notifications.Notify (Catalog.GetString ("Tweet Failed"),
					Catalog.GetString ("Missing Login Credentials. Please set login information in plugin configuration."), 
					ErrorIcon);
					
				Log.Debug ("Missing login credentials in Twitter plugin");
				return;
			}
			
			content = status.ToString ();
					
			try {
				twitter.Status.Update (content);
				Notifications.Notify (Catalog.GetString ("Tweet Successful"), 
					Catalog.GetString (String.Format ("'{0}' successfully posted to Twitter.", content)));
			} catch (LibTwitter.TwitterizerException e) {
				Notifications.Notify (Catalog.GetString ("Tweet Failed"), 
					Catalog.GetString ("Unable to post tweet. Check your login settings. If you are behind a proxy "
						+ "make sure that the settings in /system/http_proxy are correct."), ErrorIcon);
					
				Log.Debug ("Encountered an error in Tweet: ", e.Message);
			}
		}
		
		public static void UpdateFriends ()
		{
			if (twitter == null) {
				Log.Debug ("Twitter credentials invalid, please check configuration");
				return;
			}
			
			ContactItem tfriend;
			LibTwitter.TwitterUserCollection myFriends;
			
			myFriends = null;
			
			try {
				myFriends = twitter.User.Friends (); 
			} catch (LibTwitter.TwitterizerException e) {
				Log.Debug ("Encountered an error in UpdateFriends: ", e.Message);
				return;
			}
			
			foreach (LibTwitter.TwitterUser friend in myFriends) {
				tfriend = ContactItem.Create (friend.ScreenName);
				
				if (System.IO.File.Exists (Paths.Combine (PhotoDirectory, "" + friend.ID)))
					tfriend ["photo"] = Paths.Combine (PhotoDirectory,  "" + friend.ID);
				else
					DownloadBuddyIcon (friend.ProfileImageUri, friend.ID);
				
				lock (friends_lock) {
					friends.Add (tfriend);
				}
			}
		}
		
		public static void UpdateTweets ()
		{
			int userId;
			Uri imageUri;
			string text, screenname;
			LibTwitter.TwitterStatus tweet;
			
			if (!Preferences.ShowNotifications) return;
			
			
			try {
				 tweet = twitter.Status.FriendsTimeline () [0];
			} catch (LibTwitter.TwitterizerException e) {
				Log.Debug ("An error occurred while retrieving public timeline: ", e.Message);
				return;
			} catch (IndexOutOfRangeException) {
				Log.Info ("No new status updates");
				return;
			}
				
			if (tweet.TwitterUser.ScreenName.Equals (username)) return;
			if (DateTime.Compare (tweet.Created, lastUpdated) <= 0) return;
			
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
			
			lastUpdated = tweet.Created;		
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
				Log.Debug ("Failed to fetch file from {0}", imageUri.AbsoluteUri, e.Message);
			}
		}
		
		static void SetupAvailableServices ()
		{
			availableServices = new Dictionary<string,int> ();
			availableServices.Add ("twitter", (int) LibTwitter.Service.Twitter);
			availableServices.Add ("indentica", (int) LibTwitter.Service.Identica);
		}
	}
}
