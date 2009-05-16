//  Reminder.cs
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
	
	
	public class Reminder : Notification
	{
		
		public Reminder (string message, TimeSpan timeout) : base ("RemindMe", message, "")
		{
			base.Body = message;
			this.ReminderDelay = timeout;
		}
		
		public override string Icon {
			get {
				return "alarm.png@" + GetType ().Assembly.FullName;
			}
		}

		TimeSpan ReminderDelay { get; set; }
	}
}