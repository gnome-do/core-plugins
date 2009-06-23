// PingFMServiceItemSource.cs
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
using System.Linq;
using System.Threading;
using Mono.Addins;

using Do.Platform.Linux;
using Do.Universe;

namespace PingFM
{
	
	public sealed class PingFMServiceItemSource : ItemSource, IConfigurable
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Ping.FM Services");}
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Web services suppported by Ping.FM"); }
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
			get { return PingFM.Services.Cast<Item> (); }
		}

		public override void UpdateItems ()
		{
			Thread updateServices = new Thread (new ThreadStart (PingFM.UpdateServices));
			updateServices.IsBackground = true;
			updateServices.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}