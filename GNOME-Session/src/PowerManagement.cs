// PowerManagement.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Diagnostics;

using NDesk.DBus;
using org.freedesktop.DBus;

using Do.Platform;

namespace GNOME
{

	class PowerManagement
	{
		[Interface ("org.freedesktop.PowerManagement")]
		interface IPowerManagement
		{
			void Shutdown ();
			void Reboot ();
			void Hibernate ();
			void Suspend ();
		}

		private const string ObjectPath = "/org/freedesktop/PowerManagement";
		private const string BusName = "org.freedesktop.PowerManagement";

		private static IPowerManagement BusInstance
		{
			get {
				try {
					return Bus.Session.GetObject<IPowerManagement> (BusName,
							new ObjectPath (ObjectPath));
				} catch (Exception e) {
					Log<PowerManagement>.Error ("Could not get PowerManagement bus object: {0}", e.Message);
					Log<PowerManagement>.Debug (e.StackTrace);
					return null;
				}
			}
		}

		public static void Shutdown ()
		{
			try {
				BusInstance.Shutdown ();
			} catch (Exception e) {
				Log<PowerManagement>.Error ("Could not shutdown: {0}", e.Message);
				Log<PowerManagement>.Debug (e.StackTrace);
			}
		}

		public static void Hibernate ()
		{
			try {
				BusInstance.Hibernate ();
			} catch (Exception e) {
				Log<PowerManagement>.Error ("Could not hibernate: {0}", e.Message);
				Log<PowerManagement>.Debug (e.StackTrace);
			}
		}

		public static void Reboot ()
		{
			try {
				BusInstance.Reboot ();
			} catch (Exception e) {
				Log<PowerManagement>.Error ("Could not reboot: {0}", e.Message);
				Log<PowerManagement>.Debug (e.StackTrace);
			}
		}

		public static void Suspend ()
		{
			try {
				BusInstance.Suspend ();
			} catch (Exception e) {
				Log<PowerManagement>.Error ("Could not suspend: {0}", e.Message);
				Log<PowerManagement>.Debug (e.StackTrace);
			}
		}

		public static void Logout ()
		{
			try {
				Process.Start ("gnome-session-save", "--kill --silent");
			} catch (Exception e) {
				Log<PowerManagement>.Error ("Could not logout: {0}", e.Message);
				Log<PowerManagement>.Debug (e.StackTrace);
			}
		}
	}
}
