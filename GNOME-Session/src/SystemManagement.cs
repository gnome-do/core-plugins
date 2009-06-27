// SystemManagement.cs
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

	class SystemManagement
	{
		[Interface ("org.freedesktop.ConsoleKit.Manager")]
		interface ISystemManagementProxy
		{
			void Stop ();
			void Restart ();
		}

		const string BusName = "org.freedesktop.ConsoleKit";
		const string ObjectPath = "/org/freedesktop/ConsoleKit/Manager";

		static ISystemManagementProxy BusInstance
		{
			get {
				try {
					return Bus.System.GetObject<ISystemManagementProxy> (BusName, new ObjectPath (ObjectPath));
				} catch (Exception e) {
					Log<SystemManagement>.Error ("Could not get ConsoleKit bus object: {0}", e.Message);
					Log<SystemManagement>.Debug (e.StackTrace);
				}
				return null;
			}
		}

		public static void Shutdown ()
		{
			try {
				BusInstance.Stop ();
			} catch (Exception e) {
				Log<SystemManagement>.Error ("Could not shutdown: {0}", e.Message);
				Log<SystemManagement>.Debug (e.StackTrace);
			}
		}

		public static void Restart ()
		{
			try {
				BusInstance.Restart ();
			} catch (Exception e) {
				Log<SystemManagement>.Error ("Could not reboot: {0}", e.Message);
				Log<SystemManagement>.Debug (e.StackTrace);
			}
		}

	}
}
