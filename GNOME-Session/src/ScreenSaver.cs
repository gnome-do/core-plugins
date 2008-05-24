/* ScreenSaver.cs
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
using System.Diagnostics;

using NDesk.DBus;
using org.freedesktop.DBus;

namespace GNOME.Session
{

	class ScreenSaver
	{
		private const string ObjectPath = "/org/gnome/ScreenSaver";
		private const string BusName = "org.gnome.ScreenSaver";

		[Interface ("org.gnome.ScreenSaver")]
		interface IScreenSaver
		{
			void Lock ();
			void SetActive (bool value);
		}

		static private IScreenSaver BusInstance
		{
			get {
				try {
					return Bus.Session.GetObject<IScreenSaver> (BusName,
							new ObjectPath (ObjectPath));
				} catch {
					return null;
				}
			}
		}

		public static void Lock ()
		{
			try {
				// XXX: 2008-01-12 statik
				// Testing on Ubuntu Hardy Alpha 3, calling Lock via DBUS
				// while inside of gnome-do locks up the screen in a bad way
				// but running gnome-screensaver-command works fine
				// BusInstance.Lock ();
				System.Diagnostics.Process.Start ("gnome-screensaver-command", "--lock");
			} catch {
				Console.Error.WriteLine ("Could not find ScreenSaver on D-Bus.");
			}
		}

		public static void SetActive (bool value)
		{
			try {
				BusInstance.SetActive (value);
			} catch {
				Console.Error.WriteLine ("Could not find ScreenSaver on D-Bus.");
			}
		}
	}
}
