// GMailContact.cs
// 
// Copyright (C) 2008 [name of author]
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
//

using System;
using Do.Universe;

namespace GContacts
{
	public class GMailContact : IContactDetailItem 
	{
		private string key, value;
		
		public GMailContact(string name, string email)
		{
			this.key = name;
			this.value = email;
		}
		
		public string Name { get { return key; } }
		public string Description { get { return value; } }
		public string Icon { get { return "stock_person"; } }
		
		public string Key {
			get { return Key; }
		}
		
		public string Value {
			get { return value; }
		}
	}
}
