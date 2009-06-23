// PingFM.cs
// 
// Copyright (C) 2009 GNOME Do
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace PingFM
{	
	public static class PingFM
	{		
		static readonly string ConnectionErrorMessage = 
			AddinManager.CurrentLocalizer.GetString ("Failed to connect to Ping.FM service");
		
		static PingFMClient client;
		
		static PingFM ()
		{
			Preferences = new PingFMPreferences ();
			Connect (Preferences.AppKey);
		}
		
		public static PingFMPreferences Preferences { get ; private set; }
		
		public static bool TryConnect (string appKey)
		{
			PingFMClient test;
			
			try {
				test = new PingFMClient (appKey);
				test.UpdateServices ();
				Connect (appKey);
			} catch (Exception e) {
				Log.Error (ConnectionErrorMessage, e.Message);
				return false;
			}
			
			return true;
		}
		
		public static IEnumerable<Item> Services {
			get { return client.Services; }
		}
		
		public static void UpdateServices ()
		{
			client.UpdateServices ();
		}
		
		public static void Post (string method, string body, string service, 
		                         string media, string icon)
		{
			client.Post (method, body, service, media, icon);
		}
		
		static void Connect (string appKey) 
		{
			try {
				client = new PingFMClient (appKey);
			} catch (Exception e) {
				Log.Error (ConnectionErrorMessage, e.Message);
			}
		}
	}
}
