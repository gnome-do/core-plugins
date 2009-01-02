/* PingFMClient.cs
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

using Do.Platform;
using Do.Universe;

using PingFM.API;

namespace PingFM
{
	
	public class PingFMClient
	{
		readonly string ErrorInMethod = Catalog.GetString ("An error has occurred in {0}");
		readonly string MessagePosted = Catalog.GetString ("Message posted");
		
		PingFMApi pingfm;
		List<PingFMServiceItem> service_items;
		
		public PingFMClient (string appkey)
		{
			pingfm = new PingFMApi (appkey);
			service_items = new List<PingFMServiceItem> ();
		}
		
		public IEnumerable<PingFMServiceItem> Services {
			get { return service_items; }
		}
		
		public void UpdateServices ()
		{
			PingFMApi.ServicesResponse sr;			
			try {
				sr = pingfm.GetServices ();
			} catch (Exception e) { 
				sr = null;
				Log.Error (ErrorInMethod, "UpdateServices", e.Message);
				return;
			}
			
			service_items.Clear ();			
			service_items.Add (new PingFMServiceItem (Catalog.GetString ("Microblog"), "pingfm", "microblog", "http://ping.fm"));
			service_items.Add (new PingFMServiceItem (Catalog.GetString ("Status"), "pingfm", "status", "http://ping.fm"));
			
			// If a service has method "microblog" and/or "status", include it in the service_items list
			// when both methods are available, use "microblog", because to an individual service
			// either method is the same, while "microblog" has stricter limitation.			
			if (sr != null && sr.Status.Equals("OK")) {
				foreach (PingFMApi.ServiceMethods service in sr.Services) {
					string method;
					if (Regex.IsMatch (service.Methods, @".*(microblog|status).*")) {
						method = (service.Methods.Contains ("microblog")) ? "microblog" : "status";
						service_items.Add (new PingFMServiceItem (service.Name, service.ID, method, service.Url));		
					}
				}
			} else {
				Log.Error (ErrorInMethod, "UpdateServices", Catalog.GetString ("Error occurred in service response"));
			}
		}
		
		public void Post (string body, PingFMServiceItem service_item) 
		{
			string service = service_item.Id;
			if (service == "pingfm")
				service = null;
			
			PingFMApi.PingResponse pr; 

			try {
				// turn the last parameter true will switch to debug mode, 
				// message will be sent to the server but won't be published.
				pr = pingfm.Post (service_item.Method, null, body, service, false); 
			} catch (Exception e) {
				pr = null;
				Log.Error (ErrorInMethod, "Post", e.Message);
				return;
			}
			
			if (pr != null && pr.Status.Equals("OK")) {
				
				Do.Platform.Services.Notifications.Notify(GetSuccessfulNotification (service, service_item.Method, service_item.Icon));
			} else {
				Do.Platform.Services.Notifications.Notify(GetFailedNotification (service_item.Icon));
			}
		}
		
		Notification GetSuccessfulNotification (string service, string method, string icon)
		{
			if (service != null)
				return new Notification (MessagePosted,
				                         String.Format (Catalog.GetString ("Your {0} message has been successfully posted to {1}"), method, service),
				                         icon);
			else
				return new Notification (MessagePosted,
				                         String.Format (Catalog.GetString ("Your message has been successfully posted to all {0} services"), method),
				                         icon);
		}
		
		Notification GetFailedNotification (string icon)
		{
			return new Notification (Catalog.GetString ("Message posting failed"), 
			                         Catalog.GetString ("Cannot connect to the Ping.FM API server, or the server responds with an error."),
			                         icon);
		}
		
		public bool CheckLength (string message)
		{
			// If the url length >= 24, Ping.FM will replace it to a short one,
			// therefore the allowed length of the message can possibly be longer than 140.
			// we calculate here the length after the replacement
			const string LINK_PATTERN = @"https:\/\/[\S]{16,}|http:\/\/[\S]{17,}|ftp:\/\/[\S]{18,}";
			const string FAKE_PINGFM_LINK = "http://ping.fm/xxxxx";
			return Regex.Replace (message, LINK_PATTERN, FAKE_PINGFM_LINK).Length <= 140;
		}
	}
}
