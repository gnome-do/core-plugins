/* SessionCommandsItemSource.cs
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Threading;
using System.Collections.Generic;

using Do.Universe;

namespace GNOME.Session
{
	public class SessionCommandsItemSource : IItemSource
	{
		public SessionCommandsItemSource ()
		{
		}

		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (SessionCommandItem),
				};
			}
		}

		public string Name { get { return "GNOME Session Commands"; } }
		public string Description { get { return "Log out, Shutdown, Restart, etc."; } }
		public string Icon { get { return "system-log-out"; } }

		public ICollection<IItem> Items
		{
			get {
				return new IItem[] {

					new SessionCommandItem (
						"Log Out",
						"Close your session and return to the login screen.",
						"system-log-out",
						PowerManagement.Logout),

					new SessionCommandItem (
						"Shutdown",
						"Turn your computer off.",
						"system-shutdown",
						PowerManagement.Shutdown),

					new SessionCommandItem (
						"Hibernate",
						"Put your computer into hibernation mode.",
						"gnome-session-hibernate",
						PowerManagement.Hibernate),

					new SessionCommandItem (
						"Suspend",
						"Put your computer into suspend mode.",
						"gnome-session-suspend",
						PowerManagement.Suspend),

					new SessionCommandItem (
						"Restart",
						"Restart your computer.",
						"reload",
						PowerManagement.Reboot),

					new SessionCommandItem (
						"Lock Screen",
						"Lock your screen.",
						"system-lock-screen",
						ScreenSaver.Lock),

				};
			}
		}

		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}

		public void UpdateItems ()
		{
		}
	}
}
