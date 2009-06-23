// PingFMServiceItem.cs
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

using PingFM.API;

namespace PingFM
{	
	public class PingFMServiceItem : Item, IUrlItem
	{		
		string name;
		string id;
		string method;
		string url;
		string trigger;
		
		public PingFMServiceItem (string name, string id, string method, string url, string trigger)
		{
			this.name = name;
			this.id = id;
			this.method = method;
			this.url = url;
			this.trigger = trigger;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { 
				string desc = (id == "pingfm") ? 
					String.Format (Catalog.GetString ("Post message to multiple {0} services."), method) :
					AddinManager.CurrentLocalizer.GetString ("Service supported by Ping.FM");
				if (!String.IsNullOrEmpty (trigger))
				    desc += " (" + trigger + ")";
				return desc;
			}
		}
		
		public override string Icon {
			get { return id.Replace (".", "") + ".png@" + GetType().Assembly.FullName; }
		}
		
		public string Id {
			get { return id;}
		}
		
		public string Method {
			get { return method;}
		}
		
		public string Url {
			get { return url; }
		}

		public string Trigger {
			get { return trigger; }
		}
	}
}
