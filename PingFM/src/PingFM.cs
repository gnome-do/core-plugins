/* PingFM.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Unix;
using PingFM.API;

using Do.Universe;
using Do.Platform;

namespace Do.Addins.PingFM
{	
	
	public static class PingFM
	{
		private static PingFMApi pingfm;
		private static List<Item> service_items;
		
		
		static PingFM ()
		{
			string application_key;			
			service_items = new List<Item> ();
			Configuration.GetAccountData(out application_key, typeof (Configuration));
			pingfm = new PingFMApi(application_key);
		}
		
		public static bool Validate (string application_key) 
		{
			PingFMApi.PingResponse pr;
			pingfm.user_application_key = application_key;
			try {				
				pr = pingfm.Validate ();
			} catch (Exception e) {
				pr = null;
				Console.Error.WriteLine (e.Message);
				return false;
			}
			
			return	(pr == null) ? false : (pr.Status == "OK");
		}
		
		public static List<Item> ServiceItems {
			get { return service_items;}
		}
		
		public static void UpdateServices ()
		{
			PingFMApi.ServicesResponse sr;			
			try {
				sr = pingfm.GetServices ();
			} catch (Exception e) { 
				sr = null;
				Console.Error.WriteLine (e.Message);
				return;
			}
			
			service_items.Clear ();			
			service_items.Add (new PingFMServiceItem (Catalog.GetString ("Micro Blog"), "pingfm", "microblog"));
			service_items.Add (new PingFMServiceItem (Catalog.GetString ("Status"), "pingfm", "status"));
			
			// If a service has method "microblog" and/or "status", include it in the service_items list
			// when both methods are available, use "microblog", because to an individual service
			// either method is the same, while "microblog" has stricter limitation.			
			if (sr != null && sr.Status.Equals("OK")) {
				foreach (PingFMApi.ServiceMethods service in sr.Services) {
					string method;
					if (Regex.IsMatch (service.Methods, @".*(microblog|status).*")) {
						method = (service.Methods.Contains ("microblog")) ? "microblog" : "status";
						service_items.Add (new PingFMServiceItem (service.Name, service.ID, method));		
					}
				}
			} else {
				Console.Error.WriteLine (Catalog.GetString ("Fail to update Ping.FM services"));
			}
		}
		
		public static bool CheckLength (string message)
		{
			// If the url length >= 24, Ping.FM will replace it to a short one,
			// therefore the allowed length of the message can possibly be longer than 140.
			// we calculate here the length after the replacement
			const string LINK_PATTERN = @"https:\/\/[\S]{16,}|http:\/\/[\S]{17,}|ftp:\/\/[\S]{18,}";
			const string FAKE_PINGFM_LINK = "http://ping.fm/xxxxx";
			return Regex.Replace (message, LINK_PATTERN, FAKE_PINGFM_LINK).Length <= 140;
		}
		
		public static void Post (string service, string body, string method) 
		{
			if (service == "pingfm")
				service = null;
			
			PingFMApi.PingResponse pr; 

			try {
				// turn the last parameter true will switch to debug mode, 
				// message will be sent to the server but won't be published.
				pr = pingfm.Post (method, null, body, service, false); 
			} catch (Exception e) {
				pr = null;
				Console.Error.WriteLine(e.Message);
				return;
			}
			
			if (pr != null && pr.Status.Equals("OK")) {
				
				Do.Platform.Services.Notifications.Notify(Catalog.GetString ("Message posted"), 
				    Catalog.GetString (String.Format ("Your {0} message has been successfully posted via Ping.FM.", method)));
			} else {
				Do.Platform.Services.Notifications.Notify(Catalog.GetString ("Message posting failed"), 
				    Catalog.GetString ("Cannot connect to the Ping.FM API server, or the server responds with an error."));
			}
		}
	}
}
