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
using Mono.Unix;

using Do.Universe;

namespace GNOME.Session
{
	public class SessionCommandsItemSource : IItemSource
	{
		public SessionCommandsItemSource ()
		{
		}

		public IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (SessionCommandItem),
				};
			}
		}

		public string Name { 
			get { return Catalog.GetString ("GNOME Session Commands"); } 
		}
		
		public string Description { 
			get { return Catalog.GetString ("Log out, Shutdown, Restart, etc."); }
		}
		
		public string Icon { get { return "system-log-out"; } }

		public IEnumerable<IItem> Items
		{
			get {
				return new IItem[] {

					new SessionCommandItem (
						Catalog.GetString ("Log Out"),
						Catalog.GetString ("Close your session and return to the login screen."),
						"system-log-out",
						PowerManagement.Logout),

					new SessionCommandItem (
						Catalog.GetString ("Shutdown"),
						Catalog.GetString ("Turn your computer off."),
						"system-shutdown",
						PowerManagement.Shutdown),

					new SessionCommandItem (
						Catalog.GetString ("Hibernate"),
						Catalog.GetString ("Put your computer into hibernation mode."),
						"gnome-session-hibernate",
						PowerManagement.Hibernate),

					new SessionCommandItem (
						Catalog.GetString ("Suspend"),
						Catalog.GetString ("Put your computer into suspend mode."),
						"gnome-session-suspend",
						PowerManagement.Suspend),

					new SessionCommandItem (
						Catalog.GetString ("Restart"),
						Catalog.GetString ("Restart your computer."),
						"reload",
						PowerManagement.Reboot),

					new SessionCommandItem (
						Catalog.GetString ("Lock Screen"),
						Catalog.GetString ("Lock your screen."),
						"system-lock-screen",
						ScreenSaver.Lock),
				};
			}
		}

		public IEnumerable<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}

		public void UpdateItems ()
		{
		}
	}
}
