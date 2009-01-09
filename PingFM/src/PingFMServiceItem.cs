/* PingFMServiceItem.cs
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
using Mono.Unix;


using Do.Universe;
using PingFM.API;

namespace PingFM
{	
	public class PingFMServiceItem : Item, IUrlItem
	{		
		private string service_name;
		private string service_id;
		private string service_method;
		private string service_url;
		
		public PingFMServiceItem (string name, string id, string method, string url)
		{
			service_name = name;
			service_id = id;
			service_method = method;
			service_url = url;
		}
		
		public override string Name {
			get { return service_name;}
		}
		
		public override string Description {
			get { 
				return (service_id == "pingfm") ? Catalog.GetString ("Web service group supported by Ping.FM") : 
					Catalog.GetString ("Web service supported by Ping.FM");
			}
		}
		
		public override string Icon {
			get { return service_id.Replace (".", "") + ".png@" + GetType().Assembly.FullName; }
		}
		
		public string Id {
			get { return service_id;}
		}
		
		public string Method {
			get { return service_method;}
		}
		
		public string Url {
			get { return service_url; }
		}
	}
}
