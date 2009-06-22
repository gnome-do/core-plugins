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
using Mono.Addins;
using Do.Universe;

namespace GMail
{
	public class GMailContactDetailItem : Item, IContactDetailItem
	{
		string type, detail;
		
		public GMailContactDetailItem (string type, string detail)
		{
			this.type = type;
			this.detail = detail;
		}		
		
		public override string Name {
			get {
				switch (type.ToLower ()) {
				case "email.gmail": return AddinManager.CurrentLocalizer.GetString ("Primary Email");
				case "phone.gmail": return AddinManager.CurrentLocalizer.GetString ("Primary Phone");
				case "email.gmail.home": return AddinManager.CurrentLocalizer.GetString ("Home Email");
				case "email.gmail.work": return AddinManager.CurrentLocalizer.GetString ("Work Email");
				case "phone.gmail.home": return AddinManager.CurrentLocalizer.GetString ("Home Phone");
				case "phone.gmail.work": return AddinManager.CurrentLocalizer.GetString ("Work Phone");
				case "address.gmail": return AddinManager.CurrentLocalizer.GetString ("Primary Address");
				case "address.gmail.home": return AddinManager.CurrentLocalizer.GetString ("Home Address");
				case "address.gmail.work": return AddinManager.CurrentLocalizer.GetString ("Work Address");
				default: return "Other " + DetailRoot (type);
				}
			}
		}
		
		public override string Description {
			get { return detail; }
		}
		
		public override string Icon {
			get {			
				switch (DetailRoot (type)) {
				case "email": return "gmail-logo.png@" + GetType ().Assembly.FullName;
				case "address": return "go-home";
				case "phone": return "phone.png@" + GetType ().Assembly.FullName;
				default: return "stock_person";
				}
			}
		}
		
		public string Key {
			get { return type; }
		}
		
		public string Value {
			get { return detail; }
		}

		string DetailRoot (string detail)
		{
			// details are strings like detail.provider.extra, this chops off .provider.extra
			return detail.Substring (0, type.IndexOf ("."));
		}
	}
}
