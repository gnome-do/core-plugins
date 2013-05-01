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

#if USE_DBUS_SHARP
using DBus;
#else
using NDesk.DBus;
#endif

using org.freedesktop.DBus;

using Do.Platform;

namespace GNOME
{

	class SystemManagement
	{
		[Interface ("org.freedesktop.login1.Manager")]
		interface ILogind
		{
			void PowerOff (bool interactive);
			void Reboot (bool interactive);
		}

		[Interface ("org.freedesktop.ConsoleKit.Manager")]
		interface IConsoleKit
		{
			void Stop ();
			void Restart ();
		}

		const string LogindName = "org.freedesktop.login1";
		const string LogindPath = "/org/freedesktop/login1";
		const string ConsoleKitName = "org.freedesktop.ConsoleKit";
		const string ConsoleKitPath = "/org/freedesktop/ConsoleKit/Manager";

		static SystemManagement ()
		{
			try {
				BusG.Init ();
			} catch (Exception e) {
				Log<SystemManagement>.Error ("Could not initialize the bus: {0}", e.Message);
				Log<SystemManagement>.Debug (e.StackTrace);
			}
		}

		static object BusInstance {
			get {
				try {
					if (Bus.System.NameHasOwner (LogindName)) {
						return Bus.System.GetObject<ILogind> (LogindName, new ObjectPath (LogindPath));
					} else if (Bus.System.NameHasOwner (ConsoleKitName)) {
						return Bus.System.GetObject<IConsoleKit> (ConsoleKitName, new ObjectPath (ConsoleKitPath));
					}
				} catch (Exception e) {
					Log<SystemManagement>.Error ("Could not get SystemManagement bus object: {0}", e.Message);
					Log<SystemManagement>.Debug (e.StackTrace);
				}

				return null;
			}
		}

		public static void Shutdown ()
		{
			try {
				object instance = BusInstance;
				if (instance is ILogind) {
					(instance as ILogind).PowerOff (true);
				} else if (instance is IConsoleKit) {
					(instance as IConsoleKit).Stop ();
				}
			} catch (Exception e) {
				Log<SystemManagement>.Error ("Could not shutdown: {0}", e.Message);
				Log<SystemManagement>.Debug (e.StackTrace);
			}
		}

		public static void Restart ()
		{
			try {
				object instance = BusInstance;
				if (instance is ILogind) {
					(instance as ILogind).Reboot (true);
				} else if (instance is IConsoleKit) {
					(instance as IConsoleKit).Restart ();
				}
			} catch (Exception e) {
				Log<SystemManagement>.Error ("Could not reboot: {0}", e.Message);
				Log<SystemManagement>.Debug (e.StackTrace);
			}
		}

	}
}
