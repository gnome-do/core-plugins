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
		readonly string PostSuccessTitle = Catalog.GetString ("Message posted");
		readonly string PostErrorTitle = Catalog.GetString ("Message posting failed");
		readonly string PostErrorMessage = Catalog.GetString ("Cannot connect to the Ping.FM API server, or the server responds with an error.");
		readonly string SinglePostSuccess = Catalog.GetString ("Your {0} message has been successfully posted to {1}");
		readonly string MultiPostSuccess = Catalog.GetString ("Your message has been successfully posted to all {0} services");
		
		PingFMApi pingfm;
		List<PingFMServiceItem> services;
		
		public PingFMClient (string appKey)
		{
			pingfm = new PingFMApi (appKey);
			services = new List<PingFMServiceItem> ();
		}
		
		public IEnumerable<PingFMServiceItem> Services {
			get { return services; }
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
			
			services.Clear ();			
			services.Add (new PingFMServiceItem (Catalog.GetString ("Microblog"), "pingfm", "microblog", "http://ping.fm"));
			services.Add (new PingFMServiceItem (Catalog.GetString ("Status"), "pingfm", "status", "http://ping.fm"));
			
			// If a service has method "microblog" and/or "status", include it in the service_items list
			// when both methods are available, use "microblog", because to an individual service
			// either method is the same, while "microblog" has stricter limitation.			
			if (sr != null && sr.Status.Equals("OK")) {
				foreach (PingFMApi.ServiceMethods service in sr.Services) {
					string method;
					if (Regex.IsMatch (service.Methods, @".*(microblog|status).*")) {
						method = (service.Methods.Contains ("microblog")) ? "microblog" : "status";
						services.Add (new PingFMServiceItem (service.Name, service.ID, method, service.Url));		
					}
				}
			} else {
				Log.Error (ErrorInMethod, "UpdateServices", Catalog.GetString ("Error occurred in service response"));
			}
		}
		
		public void Post (string body, PingFMServiceItem serviceItem) 
		{
			string service = serviceItem.Id;
			if (service == "pingfm")
				service = null;
			
			PingFMApi.PingResponse pr; 

			try {
				// turn the last parameter true will switch to debug mode, 
				// message will be sent to the server but won't be published.
				pr = pingfm.Post (serviceItem.Method, null, body, service, false); 
			} catch (Exception e) {
				pr = null;
				Log.Error (ErrorInMethod, "Post", e.Message);
				return;
			}
			
			if (pr != null && pr.Status.Equals("OK")) {
				
				Do.Platform.Services.Notifications.Notify (GetSuccessfulNotification (service, serviceItem.Method, serviceItem.Icon));
			} else {
				Do.Platform.Services.Notifications.Notify (GetFailedNotification (serviceItem.Icon));
			}
		}
		
		Notification GetSuccessfulNotification (string service, string method, string icon)
		{
			if (service != null)
				return new Notification (PostSuccessTitle, String.Format (SinglePostSuccess, method, service), icon);
			else
				return new Notification (PostSuccessTitle, String.Format (MultiPostSuccess, method), icon);
		}
		
		Notification GetFailedNotification (string icon)
		{
			return new Notification (PostErrorTitle, PostErrorMessage, icon);
		}
		
		public bool CheckLength (string message)
		{
			// If the url length >= 24, Ping.FM will replace it to a short one,
			// therefore the allowed length of the message can possibly be longer than 140.
			// we calculate here the length after the replacement
			const string LinkPattern = @"https:\/\/[\S]{16,}|http:\/\/[\S]{17,}|ftp:\/\/[\S]{18,}";
			const string FakePingFMLink = "http://ping.fm/xxxxx";
			return Regex.Replace (message, LinkPattern, FakePingFMLink).Length <= 140;
		}
	}
}
