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
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Mono.Unix;

using Twitterizer.Framework;
using Do.Universe;

namespace DoTwitter
{
	public static class TwitterAction
	{
		private static List<IItem> items;
		private static object friends_lock;
		private static Twitter twitter;
		private static string status, username, password;
		private static DateTime lastUpdated;
		static readonly string photo_directory;
		
		static TwitterAction ()
		{
			string home;
			
			items = new List<IItem> ();
			friends_lock = new object ();
			
			Configuration.GetAccountData (out username, out password,
				typeof (Configuration));
			Connect (username, password);
			lastUpdated = DateTime.UtcNow;
			home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			photo_directory = "~/.local/share/gnome-do/Twitter/photos/".Replace ("~", home);
		} 
		
		public static bool Connect (string username, string password)
		{
			twitter = new Twitter (username, password);
			return true;
		}
		
		public static List<IItem> Friends {
			get { return items; }
		}
		
		public static string Status {
			set { status = value; }
		}
		
		public static void Tweet ()
		{
			if (!(String.IsNullOrEmpty (username) || String.IsNullOrEmpty (password))) {
				try {
					twitter.Update (status);
					Do.Addins.NotificationBridge.ShowMessage (Catalog.GetString (
						"Tweet Successful"), Catalog.GetString (String.Format (
						"'{0}' successfully posted to Twitter.",
						status)));
				} catch (TwitterizerException e) {
					Do.Addins.NotificationBridge.ShowMessage (Catalog.GetString (
						"Tweet Failed"), Catalog.GetString ("Unable to post tweet."
						+ " Check your login settings. If you are behind a proxy, "
						+ "also make sure that the settings in /system/http_proxy "
						+ "are correct.\n\nDetails:\n") + e.ToString ());
				}
			} else {
				Do.Addins.NotificationBridge.ShowMessage (Catalog.GetString (
					"Tweet Failed"), Catalog.GetString ("Missing Login Credentials. "
					+ "Please set login information in plugin configuration."));
			}
		}
		
		public static void UpdateFriends ()
		{
			if ((String.IsNullOrEmpty (username) || String.IsNullOrEmpty (password)))
				return;
			
			if (!Monitor.TryEnter (friends_lock)) return;

			TwitterUserCollection friends;
			
			try {
				friends = twitter.Friends (); 
			
				ContactItem tfriend;
				
				foreach (TwitterUser friend in friends) {
					for (int i = 0; i <= 1; i++) {
						if (i == 0)
							tfriend = ContactItem.Create (friend.ScreenName);
						else
							tfriend = ContactItem.Create (friend.UserName);
						tfriend ["twitter.screenname"] = friend.ScreenName;
						if (System.IO.File.Exists (photo_directory + friend.ID))
							tfriend ["photo"] = photo_directory + friend.ID;
						else
							DownloadBuddyIcon (friend.ProfileImageUri,
								photo_directory + friend.ID);
						items.Add (tfriend);
					}
				}
			} catch (TwitterizerException e) {
				Console.Error.WriteLine (e.Message);
			} finally {
				Monitor.Exit (friends_lock);
			} 
		}
		
		public static void UpdateTweets ()
		{
			if (!GenConfig.ShowFriendStatuses) return;
			
			try {
				string text, screenname;
				Uri imageuri;
				int userid;
				foreach (TwitterStatus tweet in twitter.FriendsTimeline ()) {
					if (tweet.TwitterUser.ScreenName.Equals (username)) return;
					if (DateTime.Compare (tweet.Created, lastUpdated) > 0) {
						text = tweet.Text;
						userid = tweet.TwitterUser.ID;
						imageuri = tweet.TwitterUser.ProfileImageUri;
						screenname = tweet.TwitterUser.ScreenName;
						Gtk.Application.Invoke (delegate {
							if (System.IO.File.Exists (photo_directory + userid))
								Do.Addins.NotificationBridge.ShowMessage (
									screenname, text,
									photo_directory + userid);
							else {
								Do.Addins.NotificationBridge.ShowMessage (
									screenname, text);
								DownloadBuddyIcon (imageuri,
									photo_directory + userid);
							}
						});
						lastUpdated = tweet.Created;
						break;
					}
				}
			} catch { }
		}
				
		private static void DownloadBuddyIcon (Uri imageUri, string location)
		{
			WebClient client = new WebClient ();
			if (!System.IO.Directory.Exists (photo_directory))
				System.IO.Directory.CreateDirectory (photo_directory);
			try {
				client.DownloadFile (imageUri.AbsoluteUri, location);
			} catch {
				Console.Error.WriteLine ("{0} does not exist!",location);
			}
		}
			
	}
}
