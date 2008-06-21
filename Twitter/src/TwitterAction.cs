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
using System.Threading;
using System.Collections.Generic;

using Twitterizer.Framework;
using Do.Addins;
using Do.Universe;

namespace DoTwitter
{
	public static class TwitterAction
	{
		private static List<IItem> items;
		private static string status, username, password;
		
		static TwitterAction ()
		{
			GLib.Timeout.Add (600000, delegate { ClearFriends(null); return true; });
			items = new List<IItem> ();
			Configuration.GetAccountData (out username, out password,
				typeof (Configuration));
		} 
		
		public static List<IItem> Friends {
			get { return items; }
		}
		
		private static string Username {
			get {
				return username;
			}
		}
		
		private static string Password {
			get {
				return password;
			}
		}
		
		public static string Status {
			set { status = value; }
		}
		
		public static void Tweet ()
		{
			Twitter t;
			
			if (!(String.IsNullOrEmpty (username) || String.IsNullOrEmpty (password))) {
				t = new Twitter (username, password);
				try {
					t.Update (status);
					Do.Addins.NotificationBridge.ShowMessage ("Tweet Successful", 
						String.Format ("'{0}' successfully posted to Twitter.",
						status));
				} catch (TwitterizerException e) {
					Do.Addins.NotificationBridge.ShowMessage ("Tweet Failed", 
						"Unable to post tweet. Check your login settings. " 
						+ "If you are behind a proxy, also make sure that the "
						+ "settings in /system/http_proxy are correct.\n\nDetails:\n" 
                        + e.ToString ());
				}
			} else {
				Do.Addins.NotificationBridge.ShowMessage ("Tweet Failed",
					"Missing Login Credentials. Please set login information "
					+ "in plugin configuration.");
			}
		}
		
		public static void UpdateFriends ()
		{
			if (!Monitor.TryEnter (items)) return;

			if ((String.IsNullOrEmpty (username) || String.IsNullOrEmpty (password))) {
				Monitor.Exit (items);
				return;
			}
			
			Twitter t;
			TwitterUserCollection friends;
			
			t = new Twitter (Username, Password);
			
			try {
				friends = t.Friends ();
			} catch (TwitterizerException e) {
				Console.Error.WriteLine (e.Message);
				Monitor.Exit (items);
				return;
			} 
			
			ContactItem tfriend;
			
			foreach (TwitterUser friend in friends) {
				tfriend = ContactItem.Create (friend.ScreenName);
				tfriend ["twitter.screenname"] = friend.ScreenName;
				tfriend ["description"] = friend.ScreenName;
				items.Add (tfriend);
				
				tfriend = ContactItem.Create (friend.UserName);
				tfriend ["twitter.screenname"] = friend.ScreenName;
				tfriend ["description"] = friend.ScreenName;
				items.Add (tfriend);
			}
			
			Monitor.Exit (items);
		}
				
		public static bool TryConnect (string username, string password)
		{
			Twitter test = new Twitter (username, password);
			try {
				test.Friends ();
			} catch {
				return false;
			}
			return true;
		}
		
		private static void ClearFriends (object state)
		{
			lock (items) {
				items.Clear ();
			}
		}
	}
}
