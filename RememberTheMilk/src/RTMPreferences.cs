// RTMPreferences.cs
// 
// Copyright (C) 2009 GNOME Do
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

using Do.Platform;

namespace RememberTheMilk
{
	/// <summary>
	/// All the Remember The Milk related preferences.
	/// </summary>
	internal class RTMPreferences
	{
		const string TokenKey = "Token";
		const string UsernameKey = "Username";
		const string FilterKey = "Filter";
		const string OverdueNotificationKey = "OverdueNotification";
		const string OverdueIntervalKey = "OverdueInterval";
		const string ActionNotificationKey = "ActionNotification";
		const string ReturnNewTaskKey = "ReturnNewTask";
		
		static IPreferences prefs = Services.Preferences.Get <RTMPreferences> ();
		
		/// <value>
		/// Indicates the RTM account has been changed.
		/// </value>
		public static event EventHandler AccountChanged;
		
		/// <value> 
		/// Called when account has been changed.
		/// </value>
		public static void OnAccountChanged ()
		{
			if (AccountChanged != null)
				AccountChanged (null, EventArgs.Empty);
		}
		
		/// <value>
		/// The current authenticated token
		/// </value>
		public static string Token {
			get { return prefs.GetSecure (TokenKey, ""); }
			set { prefs.SetSecure (TokenKey, value); }
		}

		/// <value>
		/// The username of the currently used RTM account
		/// </value>
		public static string Username {
			get { return prefs.Get(UsernameKey, ""); }
			set { prefs.Set(UsernameKey, value); OnAccountChanged ();}
		}
		
		/// <value>
		/// Indicates the filter preference has been changed.
		/// </value>
		public static event EventHandler FilterChanged;
		
		/// <value>
		/// Called when the filter preference has been changed.
		/// </value>
		public static void OnFilterChanged ()
		{
			if (FilterChanged != null)
				FilterChanged (null, EventArgs.Empty);
		}
		
		/// <value>
		/// The current filter used when retrieving task lists
		/// </value>
		public static string Filter {
			get { return prefs.Get<string> (FilterKey, "status:incomplete"); }
			set { prefs.Set<string> (FilterKey, value); OnFilterChanged (); }
		}
		
		/// <value>
		/// Indicates the interval to notify of overdue tasks have been changed.
		/// </value>
		public static event EventHandler OverdueIntervalChanged;
		
		/// <value>
		/// Called when the overdue task notifcation interval has been changed.
		/// </value>
		public static void OnOverdueIntervalChanged ()
		{
			if (OverdueIntervalChanged != null)
				OverdueIntervalChanged (null, EventArgs.Empty);
		}
		
		/// <value>
		/// The interval to display the notification of overdue tasks
		/// </value>
		public static double OverdueInterval {
			get { return prefs.Get<double> (OverdueIntervalKey, 15); }
			set { prefs.Set<double> (OverdueIntervalKey, value); OnOverdueIntervalChanged (); }
		}
		
		/// <value>
		/// Indicates the show overdue task notification preference has been changed.
		/// </value>
		public static event EventHandler OverdueNotificationChanged;
		
		/// <value>
		/// Called when the show overdue task notification preference has been changed.
		/// </value>
		public static void OnOverdueNotificationChanged ()
		{
			if (OverdueNotificationChanged != null)
				OverdueNotificationChanged (null, EventArgs.Empty);
		}
		
		/// <value>
		/// If to show a notification when there is any overdue task.
		/// </value>
		public static bool OverdueNotification {
			get { return prefs.Get<bool> (OverdueNotificationKey, true); }
			set { prefs.Set<bool> (OverdueNotificationKey, value); OnOverdueNotificationChanged (); }
		}
		
		/// <value>
		/// If to show a notification when an action is finshed.
		/// </value>
		public static bool ActionNotification {
			get { return prefs.Get<bool> (ActionNotificationKey, true); }
			set { prefs.Set<bool> (ActionNotificationKey, value); }
		}
		
		/// <value>
		/// If to return the new task item right after it is created.
		/// </value>
		public static bool ReturnNewTask {
			get { return prefs.Get<bool> (ReturnNewTaskKey, true); }
			set { prefs.Set<bool> (ReturnNewTaskKey, value); }
		}
	}
}
