//  SnoozeableReminder.cs
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;

using GLib;

using Do.Platform;
namespace RemindMe
{
	
	
	public class SnoozeableReminder : ActionableNotification
	{
		
		public SnoozeableReminder(string message, TimeSpan timeout) : base ("RemindMe", message, "", "Snooze")
		{
			base.Body = message;
			this.ReminderDelay = timeout;
		}
		
		TimeSpan ReminderDelay { get; set; }
		
		TimeSpan SnoozeTime {
			get 
			{
				//the snooze time is the lesser of (TotalReminderSeconds / 4) and 180 (3 minutes
				//ie, for all reminders > 12 minutes, the snooze time is still 3 minutes.
				//however, we still set the floor for a snooze time to be 30 seconds
				int snoozeTime = (int) Math.Max (Math.Min (ReminderDelay.TotalSeconds / 4, 180), 30);
				return new TimeSpan (0,0,snoozeTime);
			}
		}
		
		public override void PerformAction ()
		{
			GLib.Timeout.Add ((uint) 
this.SnoozeTime.TotalMilliseconds, () => {
				Services.Notifications.Notify (new SnoozeableReminder (this.Body, this.SnoozeTime));
				return false; 
			});
		}

	}
}
