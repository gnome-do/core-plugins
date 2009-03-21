/* RTMLocationItem.cs
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

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	public class RTMLocationItem : Item
	{
		string id;
		string longitude;
		string latitude;
		string name;
		string address;
		
		public RTMLocationItem(string id, string name, string address, string longitude, string latitude)
			//: base ("Location: "+name, address, "http://maps.google.com/maps?q="+latitude+","+longitude, "stock_internet")
		{
			this.id = id;
			this.name = name;
			this.address = address;
			this.latitude = latitude;
			this.longitude = longitude;
		}
		
		public string Id {
			get { return id;}
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return address; }
		}
		
		public override string Icon {
			get { return "stock_internet"; }
		}
			
		public string Longitude {
			get { return longitude; }
		}
			
		public string Latitude {
			get { return latitude; }
		}
	}
}
