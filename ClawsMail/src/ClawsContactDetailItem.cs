// ClawsContactDetailItem.cs created with MonoDevelop
// User: Karol Będkowski at 22:49 2008-10-10

// Copyright Karol Będkowski 2008

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
using Do.Universe;
using Mono.Unix;

namespace Claws {
	
	/// <summary>
	/// Claws contact.
	/// </summary>
	public class ClawsContactDetailItem: Item, IContactDetailItem {
		private string type, detail, name, icon;

		public ClawsContactDetailItem (string type, string detail) {
			this.type = type.ToLower ();
			this.detail = detail;

			// name
			if (type == "email.claws") {
				name = Catalog.GetString ("Claws Primary Email");
			} else {
				name = Catalog.GetString ("Claws Other") + " " + type.Substring (0, type.IndexOf ("."));
			}

			// icon
			if (type.StartsWith ("email.")) {
				icon = "stock_mail-compose";
			} else {
				icon = "stock_person";
			}
		}
		
		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return detail; } 
		}

		public string Key {
			get { return type; } 
		}
		
		public string Value {
			get { return detail; } 
		}

		public override string Icon {
			get { return icon; }
		}
	}
}
