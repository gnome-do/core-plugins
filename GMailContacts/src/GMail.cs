/* GMail.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace GMail
{
	public static class GMail
	{
		static readonly string ConnectionErrorMessage = Catalog.GetString ("An error occurred connecting to google, "
			+ "are your credentials valid?");
			
		static readonly string MissingCredentialsMessage = Catalog.GetString ("Missing login credentials. Please set "
			+ "login information in plugin configuration.");
			
		static GMailClient client;
		
		static GMail()
		{
			Preferences = new Preferences ();
			client = new GMailClient (Preferences.Username, Preferences.Password);
		}
		
		public static IEnumerable<Item> Contacts { 
			get { return client.Contacts; }
		}
		
		public static Preferences Preferences { get; private set; }

		public static void UpdateContacts ()
		{
			client.UpdateContacts ();
		}
	
		public static bool TryConnect (string username, string password)
		{
			GMailClient test;

			try {
				test = new GMailClient (username, password);
				test.UpdateContacts ();
				Connect (username, password);
			} catch (Exception) {
				Log.Error (ConnectionErrorMessage);
				return false;
			}
			
			return true;
		}

		static void Connect (string username, string password) 
		{
			if (string.IsNullOrEmpty (username) || string.IsNullOrEmpty (password)) {
				Log.Error (MissingCredentialsMessage);
				return;
			}

			try {
				client = new GMailClient (username, password);
			} catch (Exception) {
				Log.Error (ConnectionErrorMessage);
			}
		}
	}
}
