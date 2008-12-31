/* Notifications.cs 
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

using Mono.Unix;

using Do.Platform;

namespace Microblogging
{
	public class TimelineNotification : Notification
	{
		public TimelineNotification (string user, string body, string icon)
			: base (user, body, icon)
		{
		}
	}

	public class DirectMessageNotification : Notification
	{
		readonly string NewMessageFormat = Catalog.GetString ("New direct message from {0}");

		string user;

		public DirectMessageNotification (string user, string message, string icon)
			: base ("", message, icon)
		{
			this.user = user;
		}

		public override string Title {
			get { return string.Format (NewMessageFormat, user); }
		}
	}

	public class StatusUpdatedNotification : Notification
	{
		readonly string FailedPostTitle = Catalog.GetString ("Post failed");
		readonly string SuccessfulPostTitle = Catalog.GetString ("Post Successful");
		readonly string FailFormat = Catalog.GetString ("Failed to post '{0}' to {1}");
		readonly string SuccessFormat = Catalog.GetString ("'{0}' sucessfully posted to {1}");

		bool success;
		string status;
		
		public StatusUpdatedNotification (bool success, string status)
		{
			Icon = "";
			this.status = status;
			this.success = success;
		}

		public override string Title {
			get { return success ? SuccessfulPostTitle : FailedPostTitle; }
		}

		public override string Body {
			get { return success 
				? string.Format (SuccessFormat, status, Microblog.Preferences.ActiveService)
				: string.Format (FailFormat, status, Microblog.Preferences.ActiveService);
			}
		}
	}

	public class Notifications
	{
		
		public Notifications ()
		{
		}

		public void Notify (Notification notification)
		{
			Gtk.Application.Invoke ((o, e) => Services.Notifications.Notify (notification));
		}
	}
}