
// ServiceItemSource.cs
// User: Karol Będkowski at 09:29 2008-10-24
//
//Copyright Karol Będkowski 2008
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;
using Do.Platform.Linux;

namespace SystemServices {
	
	/// <summary>
	/// Source for services.
	/// </summary>
	public class ServiceItemSource: ItemSource, IConfigurable {
	
		List<Item> items;
		
		public override string Name {
			get { return Catalog.GetString ("System Services"); }
		}

		public override string Description {
			get { return Catalog.GetString ("List of all System Services"); }
		}

		public override string Icon {
			get { return "applications-system";	}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (Service); }
		}

		public override IEnumerable<Item> Items {
			get { return items;	}
		}		
		
		public ServiceItemSource ()
		{
			items = new List<Item> ();
		}

		public override void UpdateItems ()
		{
			items = SystemServices.LoadServices ();
		}		

		public Gtk.Bin GetConfiguration ()
		{
			return new SystemServicesConfig ();
		}
	}
}
