//  EmailContactDetailItem.cs
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Do.Universe;
using Mono.Unix;

namespace Evolution
{
	class AddressContactDetailItem : ContactDetailItem {
		public AddressContactDetailItem (ContactItem owner, string detail) :
			base (owner, detail)
		{
		}

		public override string Name {
			get {
				return Catalog.GetString ("Address");
				
				/* // The home/other/work tags are not exact.
				string desc = "";
				if (Key.Contains (".work"))
					desc += "Work Email";
				else if (Key.Contains (".home"))
					desc += "Home Email";
				else if (Key.Contains (".other"))
					desc += "Other Email";
				return desc;
				*/
			}
		}
		
		public override string Description {
			get { return Value.Replace ('\n', ' '); }
		}


		public override string Icon {
			get { return "house.png@" + GetType ().Assembly.FullName; }
		}
	}
}
