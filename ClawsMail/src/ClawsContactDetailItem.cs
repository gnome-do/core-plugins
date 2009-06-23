/* ClawsContactDetailItem.cs 
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
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

namespace Claws {
	
	/// <summary>
	/// Claws contact.
	/// </summary>
	public class ClawsContactDetailItem: Item, IContactDetailItem {

		const string IconForOthers = "stock_person";
		const string IconForMail = "stock_mail-compose";
		
		string type, detail, name, icon;

		#region std properties
		
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

		#endregion
		
		public ClawsContactDetailItem (string type, string detail) 
		{
			this.type = type.ToLower ();
			this.detail = detail;

			if (type.StartsWith ("email.")) {
				icon = IconForMail;
				
				string remark = type.Substring (type.LastIndexOf (".") + 1);
				if (type.StartsWith (ClawsContactsItemSource.ClawsPrimaryEmailPrefix)) {
					name = AddinManager.CurrentLocalizer.GetString ("Primary Email") + " " + remark;
				} else {
					if (remark.Length > 0) {
						name = AddinManager.CurrentLocalizer.GetString ("Email") + " " + remark;
					} else {
						name = AddinManager.CurrentLocalizer.GetString ("Other email");
					}
				}
				
			} else {				
				icon = IconForOthers;
				name = AddinManager.CurrentLocalizer.GetString ("Other");
			}
		}


	}
}
