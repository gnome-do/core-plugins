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
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace GNOME
{
	public class SessionCommandsItemSource : ItemSource
	{
	
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				yield return typeof (SessionCommandItem);
			}
		}

		public override string Name { 
			get { return AddinManager.CurrentLocalizer.GetString ("GNOME Session Commands"); } 
		}
		
		public override string Description { 
			get { return AddinManager.CurrentLocalizer.GetString ("Log out, Shutdown, Restart, etc."); }
		}
		
		public override string Icon { get { return "system-log-out"; } }

		public override IEnumerable<Item> Items
		{
			get {
				yield return new SessionCommandItem (
					AddinManager.CurrentLocalizer.GetString ("Log Out"),
					AddinManager.CurrentLocalizer.GetString ("Close your session and return to the login screen."),
					"system-log-out",
					PowerManagement.Logout);

				yield return new SessionCommandItem (
					AddinManager.CurrentLocalizer.GetString ("Shutdown"),
					AddinManager.CurrentLocalizer.GetString ("Turn your computer off."),
					"system-shutdown",
					SystemManagement.Shutdown);

				yield return new SessionCommandItem (
					AddinManager.CurrentLocalizer.GetString ("Hibernate"),
					AddinManager.CurrentLocalizer.GetString ("Put your computer into hibernation mode."),
					"gnome-session-hibernate",
					PowerManagement.Hibernate);

				yield return new SessionCommandItem (
					AddinManager.CurrentLocalizer.GetString ("Suspend"),
					AddinManager.CurrentLocalizer.GetString ("Put your computer into suspend mode."),
					"gnome-session-suspend",
					PowerManagement.Suspend);

				yield return new SessionCommandItem (
					AddinManager.CurrentLocalizer.GetString ("Restart"),
					AddinManager.CurrentLocalizer.GetString ("Restart your computer."),
					"reload",
					SystemManagement.Restart);

				yield return new SessionCommandItem (
					AddinManager.CurrentLocalizer.GetString ("Lock Screen"),
					AddinManager.CurrentLocalizer.GetString ("Lock your screen."),
					"system-lock-screen",
					ScreenSaver.Lock);
			}
		}

		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			yield break;
		}

		public override void UpdateItems ()
		{
		}
	}
}
