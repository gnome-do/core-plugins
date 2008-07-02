/*
 * GMailContactDetailItem.cs
 * 
 * GNOME Do is the legal property of its developers, whose names are too numerous
 * to list here.  Please refer to the COPYRIGHT file distributed with this
 * source distribution.
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
using Mono.Unix;

namespace GMailContacts
{
	public class GMailContactDetailItem : IContactDetailItem 
	{
		private string type, detail;
		
		
		public GMailContactDetailItem (string type, string detail)
		{
			this.type = type;
			this.detail = detail;
		}		
		
		public string Name {
			get {
				switch (type.ToLower ()) {
				case "email.gmail": return Catalog.GetString ("Primary Email");
				case "email.gmail.home": return Catalog.GetString ("Home Email");
				case "email.gmail.work": return Catalog.GetString ("Work Email");
				case "address.gmail": return Catalog.GetString ("Primary Address");
				case "address.gmail.home": return Catalog.GetString ("Home Address");
				case "address.gmail.work": return Catalog.GetString ("Work Address");
				case "phone.gmail": return Catalog.GetString ("Primary Phone");
				case "phone.gmail.home": return Catalog.GetString ("Home Phone");
				case "phone.gmail.work": return Catalog.GetString ("Work Phone");
				default:
					return "Other " + type.Substring (0, type.IndexOf ("."));
				}
			}
		}
		
		public string Description {
			get { return detail; }
		}
		
		public string Icon {
			get {
				switch (type.Substring (0,type.IndexOf ("."))) {
				case "email": return "gmail-logo.png@" + GetType ().Assembly.FullName;
				case "address": return "go-home";
				case "phone": return "phone.png@" + GetType ().Assembly.FullName;
				default: return "stock_person";
				}
			}
		}
		
		public string Key {
			get {
				return type;
			}
		}
		
		public string Value {
			get { 
				return detail;
			}
		}
	}
}
