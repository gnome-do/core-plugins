/* PingFMServiceItemSource.cs
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
using System.Threading;
using Mono.Unix;
using PingFM.API;

using Do.Platform.Linux;
using Do.Universe;

namespace Do.Addins.PingFM
{
	
	public sealed class PingFMServiceItemSource : ItemSource, IConfigurable
	{
		public override string Name {
			get { return Catalog.GetString ("Ping.FM Services");}
		}
		
		public override string Description {
			get { return Catalog.GetString ("Web services suppported by Ping.FM"); }
		}
		
		public override string Icon {
			get { return "pingfm.png@" + GetType ().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (PingFMServiceItem),
				};
			}
		}

		public override IEnumerable<Item> Items {
			get { return PingFM.ServiceItems; }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			return null;
		}

		public override void UpdateItems ()
		{
			Thread updateServices = new Thread (new ThreadStart (Do.Addins.PingFM.PingFM.UpdateServices));
			updateServices.IsBackground = true;
			updateServices.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}