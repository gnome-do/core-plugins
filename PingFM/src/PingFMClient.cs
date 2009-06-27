// PingFMClient.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

using PingFM.API;

namespace PingFM
{	
	public class PingFMClient
	{
		readonly string ErrorInMethod = AddinManager.CurrentLocalizer.GetString ("An error has occurred in {0}");
		readonly string PostSuccessTitle = AddinManager.CurrentLocalizer.GetString ("Message posted");
		readonly string PostErrorTitle = AddinManager.CurrentLocalizer.GetString ("Message posting failed");
		readonly string PostErrorMessage = AddinManager.CurrentLocalizer.GetString ("Cannot connect to the Ping.FM API server, or the server responds with an error.");
		readonly string SinglePostSuccess = AddinManager.CurrentLocalizer.GetString ("Your {0} message has been successfully posted to {1}");
		readonly string MultiPostSuccess = AddinManager.CurrentLocalizer.GetString ("Your message has been successfully posted to all {0} services");
		
		PingFMApi pingfm;
		List<Item> services;
		
		public PingFMClient (string appKey)
		{
			pingfm = new PingFMApi (appKey);
			services = new List<Item> ();
		}
		
		public IEnumerable<Item> Services {
			get { return services; }
		}
		
		public void UpdateServices ()
		{
			PingFMApi.ServicesResponse sr;
			try {
				sr = pingfm.GetServices ();
			} catch (Exception e) { 
				sr = null;
				Log<PingFMClient>.Error (ErrorInMethod, "UpdateServices", e.Message);
				return;
			}
			
			services.Clear ();
			services.Add (new PingFMServiceItem (AddinManager.CurrentLocalizer.GetString ("Microblog"), 
					"pingfm", "microblog", "http://ping.fm", "@m"));
			services.Add (new PingFMServiceItem (AddinManager.CurrentLocalizer.GetString ("Status"),
					"pingfm", "status", "http://ping.fm", "@s"));
			
			// If a service has method "microblog" and/or "status", include it in the service_items list
			// when both methods are available, use "microblog", because to an individual service
			// either method is the same, while "microblog" has stricter limitation.			
			if (sr != null && sr.Status.Equals("OK")) {
				foreach (PingFMApi.ServiceMethods service in sr.Services) {
					if (Regex.IsMatch (service.Methods, @".*(microblog|status).*")) {
						services.Add (new PingFMServiceItem (service.Name, service.ID, 
								service.Methods, service.Url, service.Trigger));
					}
				}
			} else {
				Log<PingFMClient>.Error (ErrorInMethod, "UpdateServices", 
					AddinManager.CurrentLocalizer.GetString ("Error occurred in service response"));
			}
			Log<PingFMClient>.Debug ("Retrieved {0} Ping.FM services", services.Capacity);
		}
		
		public void Post (string method, string body, string service, string media, string icon) 
		{
			if (service == "pingfm")
				service = null;
			
			PingFMApi.PingResponse pr; 

			try {
				// turn the last parameter true will switch to debug mode, 
				// message will be sent to the server but won't be published.
				pr = pingfm.Post (method, null, body, service, media, false); 
			} catch (Exception e) {
				pr = null;
				Log<PingFMClient>.Error (ErrorInMethod, "Post", e.Message);
				return;
			}
			
			if (pr != null && pr.Status.Equals("OK"))
				Do.Platform.Services.Notifications.Notify (GetSuccessfulNotification (service, method, icon));
			else
				Do.Platform.Services.Notifications.Notify (GetFailedNotification (icon));
		}
		
		Notification GetSuccessfulNotification (string service, string method, string icon)
		{
			if (service != null)
				return new Notification (PostSuccessTitle, 
					String.Format (SinglePostSuccess, method, service), icon);
			else
				return new Notification (PostSuccessTitle, 
					String.Format (MultiPostSuccess, method), icon);
		}
		
		Notification GetFailedNotification (string icon)
		{
			return new Notification (PostErrorTitle, PostErrorMessage, icon);
		}
	}
}
