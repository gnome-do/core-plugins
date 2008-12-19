/* Preferences.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *  
 * This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
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
using System.Collections.Generic;

using Twitterizer.Framework;

using Do.Platform;

namespace Microblogging
{
	
	public class MicroblogPreferences
	{
		#region contants and default values

		const string UsernameKey = "{0}/Username";
		const string PasswordKey = "{0}/Password";
		const string MicroblogServiceKey = "Service";
		const string ShowNotificationsKey = "ShowFriendUpdates";
		
		const string MicroblogServiceDefault = "twitter";
		const bool ShowNotificationsDefault = true;
		
		#endregion
	
		IPreferences prefs;
		string active_service;

		public MicroblogPreferences()
		{
			prefs = Services.Preferences.Get <MicroblogPreferences> ();
			active_service = prefs.Get<string> (MicroblogServiceKey, MicroblogServiceDefault);
		}

		public string Username { 
			get { return prefs.Get<string> (string.Format (UsernameKey, Microblog.Preferences.MicroblogService), ""); }
			set { prefs.Set<string> (string.Format (UsernameKey, Microblog.Preferences.MicroblogService), value); }
		}
		
		public string Password { 
			get { return prefs.SecureGet (string.Format (PasswordKey, Microblog.Preferences.MicroblogService), ""); }
			set { prefs.SecureSet (string.Format (PasswordKey, Microblog.Preferences.MicroblogService), value); }
		}
		
		public bool ShowNotifications {
			get { return prefs.Get<bool> (ShowNotificationsKey, ShowNotificationsDefault); }
			set { prefs.Set<bool> (ShowNotificationsKey, value); }
		}
		
		public string MicroblogService {
			get { return active_service; }
			set {
				active_service = value; 
				prefs.Set<string> (MicroblogServiceKey, value); 
			}
		}

		public Service ActiveService {
			get { return (Service) Enum.Parse (typeof (Service), MicroblogService, true); }
		}

		public string GetServiceName (Service service)
		{
			return Enum.GetName (typeof (Service), service);
		}

		public IEnumerable<string> AvailableServices {
			get { return Enum.GetNames (typeof (Service)); }
		}
	}
}
