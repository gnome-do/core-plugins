/* RTMPreferences.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using Do.Platform;

namespace RememberTheMilk
{	
	public class RTMPreferences
	{
		const string TokenKey = "Token";
		const string UsernameKey = "Username";
		const string FilterKey = "Filter";
		const string OverdueNotificationKey = "OverdueNotification";
		const string ActionNotificationKey = "ActionNotification";
		const string ReturnNewTaskKey = "ReturnNewTask";
		
		IPreferences prefs;
		
		public RTMPreferences()
		{
			prefs = Services.Preferences.Get <RTMPreferences> ();
		}
		
		public string Token {
			get { return prefs.GetSecure (TokenKey, ""); }
			set { prefs.SetSecure (TokenKey, value); }
		}
		
		public string Username {
			get { return prefs.Get(UsernameKey, ""); }
			set { prefs.Set(UsernameKey, value); }
		}
		
		public string Filter {
			get { return prefs.Get<string> (FilterKey, "status:incomplete"); }
			set { prefs.Set<string> (FilterKey, value); }
		}
		
		public bool OverdueNotification {
			get { return prefs.Get<bool> (OverdueNotificationKey, true); }
			set { prefs.Set<bool> (OverdueNotificationKey, value); }
		}
		
		public bool ActionNotification {
			get { return prefs.Get<bool> (ActionNotificationKey, true); }
			set { prefs.Set<bool> (ActionNotificationKey, value); }
		}
		
		public bool ReturnNewTask {
			get { return prefs.Get<bool> (ReturnNewTaskKey, true); }
			set { prefs.Set<bool> (ReturnNewTaskKey, value); }
		}
	}
}
