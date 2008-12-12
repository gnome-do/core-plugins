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
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Microblogging
{
	public static class Microblog
	{
		static readonly string NotifyFailMsg = Catalog.GetString ("Post failed");
		static readonly string NotifySuccessMsg = Catalog.GetString ("Post Successful");
		static readonly string MissingCredentialsMsg = Catalog.GetString ("Missing login credentials. Please set login "
			+ "information in plugin configuration.");
		
		static MicroblogClient client;
		static MicroblogPreferences prefs;
		
		static Microblog ()
		{
			prefs = new MicroblogPreferences ();
			GenConfig.ServiceChanged += ServiceChanged;
		}

		public static bool Connect (string username, string password)
		{
			if (string.IsNullOrEmpty (username) || string.IsNullOrEmpty (password)) {
				Log.Error (MissingCredentialsMsg);
				return false;
			}
			
			client = new MicroblogClient (username, password, prefs.ActiveService);
			client.StatusUpdated += OnStatusUpdated;
			client.TimelineUpdated += OnTimelineUpdated;
			
			return true;
		}
			
		public static IEnumerable<IItem> Friends {
			get { return client.Contacts; }
		}

		public static void UpdateStatus (object status)
		{
			client.UpdateStatus (status.ToString ());
		}
		
		internal static MicroblogPreferences Preferences {
			get { return prefs; }
		}

		static void OnStatusUpdated (object sender, StatusUpdatedEventArgs args)
		{
			string title;

			title = string.IsNullOrEmpty (args.ErrorMessage) ? NotifySuccessMsg : NotifyFailMsg;
			Gtk.Application.Invoke ((o, e) => Notifications.Notify (title, args.Status));
		}

		static void OnTimelineUpdated (object sender, TimelineUpdatedEventArgs args)
		{
			Gtk.Application.Invoke ((o, e) => Notifications.Notify (args.Screenname, args.Status, args.Icon));
		}
		
		static void ServiceChanged (object sender, EventArgs args)
		{
			Connect (prefs.Username, prefs.Password);
		}
	}
}