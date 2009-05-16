//  RemindMe.cs
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
using System.Linq;
using System.Collections.Generic;
using Mono.Unix;

using GLib;

using Do.Universe;
using Do.Platform;

namespace RemindMe
{

	public class RemindMe : Act
	{
		
		readonly string remindMessageHourMin;
		readonly string remindMessageMin;
		readonly string [] timeKeyWords;
		
		class AllowSnoozeItem : Item
		{
			
			public override string Name {
				get { return Catalog.GetString ("Allow Snooze"); }
			}
			
			public override string Description {
				get { return Catalog.GetString ("Allows this reminder to be snoozed."); }
			}
			
			public override string Icon {
			get { return "snooze.png@" + GetType ().Assembly.FullName; }
			}			
		}
		
		class NoSnoozeItem : Item
		{
			
			public override string Name {
				get { return Catalog.GetString ("No Snooze Allowed"); }
			}
			
			public override string Description {
				get { return Catalog.GetString ("This reminder cannot be snoozed."); }
			}
			
			public override string Icon {
			get { return "alarm.png@" + GetType ().Assembly.FullName; }
			}			
		}
		
		public RemindMe ()
		{
			remindMessageHourMin = Catalog.GetString ("You will be reminded in {0} hours, {0} minutes.");
			remindMessageMin = Catalog.GetString ("You will be reminded in {0} minutes");
			timeKeyWords = new string[] {Catalog.GetString ("in"), Catalog.GetString ("at")};
		}

		public override string Name {
			get { return Catalog.GetString ("Remind Me"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Simple Reminders"); }
		}

		public override string Icon {
			get { return "alarm.png@" + GetType ().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
			}
		}

		public override bool SupportsItem (Item item) {
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get {
				yield return typeof (AllowSnoozeItem);
				yield return typeof (NoSnoozeItem);
			}
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			yield return new AllowSnoozeItem ();
			yield return new NoSnoozeItem ();
		}
		
		public override bool ModifierItemsOptional
		{
			get { return true; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string message, matchedWord;
			TimeSpan timeout;
			message = (items.First () as ITextItem).Text;
			
			//bad time string, bail
			if (!message.ContainsAny (timeKeyWords))
			    yield break;
			
			timeout = ParseTimeString (message.Substring (message.LastIndexOfAny (timeKeyWords, out matchedWord) + matchedWord.Length));
			message = message.Substring (0, message.LastIndexOfAny (timeKeyWords)).Trim ();
			
			if (timeout.TotalMilliseconds == 0)
				yield break;
			         			
			MaybeShowMessage (timeout);
			
			Notification reminder;
			
			if (modItems.Any ()) {
				if (modItems.First () is AllowSnoozeItem)
					reminder = new SnoozeableReminder (message, timeout);
				else
					reminder = new Reminder (message, timeout);
			} else {
				reminder = new Reminder (message, timeout);
			}
			
			GLib.Timeout.Add ((uint) timeout.TotalMilliseconds, () => { 
				Services.Notifications.Notify (reminder);
				return false; 
			});
			yield break;
		}
		
		TimeSpan ParseTimeString (string timeStr)
		{
			DateTime t;
			TimeSpan notificationTimeout = new TimeSpan (0,0,0);
			
			//this will catch strings like 10:00 PM or 14:12
			if (TryConvert (timeStr, out t)) {
				notificationTimeout = t - DateTime.Now;
				return notificationTimeout;
			} else {
				//this will catch strings like 2m2s or 2 h 2 m
				int hours, minutes, seconds;
				hours = minutes = seconds = 0;
				
				timeStr = timeStr.Replace (" ", null);
				//I'm not quite sure how to make these translateable...
				//There could be a case where the first letter for two time units are the same,
				//for example, german Stunde = hour, Sekund = second.
				foreach (string match in timeStr.Matches (new [] {"h", "m", "s"})) {
					try {
						switch (match) {
						case "h":
							hours = GetTimeUnit (timeStr, "h", out timeStr);
							break;
						case "m":
							minutes = GetTimeUnit (timeStr, "m", out timeStr);
							break;
						case "s":
							seconds = GetTimeUnit (timeStr, "s", out timeStr);
							break;
						}
					} catch {
						//bad time string
						return new TimeSpan ();
					}
				}
				notificationTimeout = notificationTimeout.Add (
				    new TimeSpan (hours, minutes, seconds));
				return notificationTimeout;
			}
		}
		
		bool TryConvert (string timeString, out DateTime time)
		{			
			try {
				time = Convert.ToDateTime (timeString);
			} catch {
				time = default (DateTime);
				return false;
			}
			return true;
		}
		
		void MaybeShowMessage (TimeSpan timeout)
		{
			if (timeout.TotalMinutes < 2)
				return;
			else if (timeout.TotalMinutes < 60) {
				Services.Notifications.Notify ("RemindMe", 
			        string.Format (remindMessageMin, timeout.Minutes));
			} else {
				Services.Notifications.Notify ("RemindMe", 
			        string.Format (remindMessageHourMin, timeout.Hours, timeout.Minutes));
			}
			return;
		}
		
		int GetTimeUnit (string input, string match, out string remainingString)
		{
			int val = int.Parse (input.Substring (0, input.IndexOf (match)));
			remainingString = input.Substring (input.IndexOf (match)+1);
			return val;
		}
	}
}
