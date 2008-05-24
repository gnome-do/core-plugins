/* GoogleMapAction.cs
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

using Do.Universe;
using Do.Addins;

namespace SimplePlugins.GoogleMaps
{
	/// <summary>
	/// Given an ITextItem, GoogleMapAction will plot its location,
	/// with a modifier item it will plot the route from the item location
	/// to the modifier location
	/// </summary>
	public class GoogleMapAction : AbstractAction
	{
		public GoogleMapAction ()
		{ }
		
		public override string Name {
			get { return "Map"; }
		}
		
		public override string Description
		{
			get { return "Map a location or route in Google maps."; }
		}
		
		public override string Icon
		{
			get { return "applications-internet"; }
		}
		
		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ContactItem),
					typeof (ITextItem),
				};
			}
		}

		public override Type[] SupportedModifierItemTypes
		{
			get {
				return new Type[] {
					typeof (ContactItem),
					typeof (ITextItem),
				};
			}		
		}
				
		public override bool SupportsItem (IItem item)
		{
			if (item is ITextItem) return true;
			return ContactItemSupportsAddress (item as ContactItem);
		}
		
		public override bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			if (modItem is ITextItem) return true;
			return ContactItemSupportsAddress (modItem as ContactItem);
		}

		
		public override bool ModifierItemsOptional {
            get { return true; }
        }
		
		public override IItem [] Perform (IItem [] items, IItem [] modifierItems)
		{
			string expression, url, start, end;
			start = end = String.Empty;
			
			if (items [0] is ITextItem)
				start = (items [0] as ITextItem).Text;
			else
				start = (items [0] as ContactItem)["address"];
			
			if (modifierItems.Length > 0) {
				if (modifierItems [0] is ITextItem)
					end = (modifierItems [0] as ITextItem).Text;
				else
					end = (modifierItems [0] as ContactItem)["address"];
			}
						
			expression = String.Format ("from: {0}", start);
			if (!String.IsNullOrEmpty (end))
				expression += String.Format (" to: {0}",end);

			url = GoogleMapsURLWithExpression (expression);
			
			Util.Environment.Open (url);
			
			return null;
		}
		
		string GoogleMapsURLWithExpression (string e)
		{
			return "http://maps.google.com/maps?q=" + (e ?? "")
				.Replace (" ", "+");
		}
		
		private bool ContactItemSupportsAddress (ContactItem item)
		{
			if (!String.IsNullOrEmpty(item["address"]))
					return true;
			return false;
		}
	}
}
