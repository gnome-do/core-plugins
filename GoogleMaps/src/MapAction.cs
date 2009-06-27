/* MapAction.cs
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
using System.Linq;
using System.Web;
using Mono.Addins;

using Do.Universe;
using Do.Platform;


namespace Google
{
	/// <summary>
	/// Given an ITextItem, ContactItem, or ContactDetailItem GoogleMapAction 
	/// will plot its location, with a modifier item it will plot the route 
	/// from the item location to the modifier location.
	/// </summary>
	public class MapAction : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Map"); }
		}
		
		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Map a location or route in Google maps."); }
		}
		
		public override string Icon
		{
			get { return "applications-internet"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				yield return typeof (IContactDetailItem);
				yield return typeof (ContactItem);
				yield return typeof (ITextItem);
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get {
				yield return typeof (IContactDetailItem);
				yield return typeof (ContactItem);
				yield return typeof (ITextItem);
			}		
		}
				
		public override bool SupportsItem (Item item)
		{
			if (item is ContactItem)
				return ContactSupportsAddress (item as ContactItem);
			else if (item is IContactDetailItem)
				return (item as IContactDetailItem).Key.StartsWith ("address");
			else if (item is ITextItem)
				return true;
			
			return false;
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return SupportsItem (modItem);
		}
		
		public override bool ModifierItemsOptional {
            		get { return true; }
        	}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			string expression, url, start, end;
			start = end = "";
			
			foreach (Item item in items) {
				start = AddressFromItem (item);
				
				if (modifierItems.Any ())
					end = AddressFromItem (modifierItems.First ());
							
				expression = String.IsNullOrEmpty (end) ?
					start :
					String.Format ("from: {0} to: {1}", start, end);

				url = GoogleMapsURLWithExpression (expression);
				
				Services.Environment.Execute (url);
			}
			return null;
		}
		
		string AddressFromItem (Item item)
		{
			if (item is IContactDetailItem)
				return (item as IContactDetailItem).Value;
			if (item is ContactItem) {
				foreach (string detail in (item as ContactItem).Details) {
					if (detail.StartsWith ("address"))
						return (item as ContactItem) [detail];
				}
			}			
			return (item as ITextItem).Text;
		}
		
		string GoogleMapsURLWithExpression (string e)
		{
			return "http://maps.google.com/maps?q=" + HttpUtility.UrlEncode (e ?? "");
		}
		
		bool ContactSupportsAddress (ContactItem item)
		{
			foreach (string detail in item.Details) {
				if (detail.StartsWith ("address"))
					return true;
			}
			return false;
		}
	}
}
