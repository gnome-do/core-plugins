//  EmpathySetStatusAction.cs
//
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//
//  Copyright © 2010
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;

using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;
using EmpathyPlugin;
using Telepathy;

namespace EmpathyPlugin
{
	public class EmpathySetStatusAction : Act
	{
		public EmpathySetStatusAction ()
		{
		}

		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Set status"); }
		}

		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Set empathy status message"); }
		}

		public override string Icon
		{
			get { return "empathy"; }
		}

		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				yield return typeof (EmpathyStatusItem);
				yield return typeof (ITextItem);
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes
		{
			get { 
				yield return typeof (ITextItem); 
				yield return typeof (EmpathyStatusItem);
			}
		}

		public override bool ModifierItemsOptional
		{
			get { return true; }
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (items.First () is EmpathySavedStatusItem || modItem is EmpathySavedStatusItem)
				return false;
			if (items.First () is EmpathyStatusItem && modItem is ITextItem)
				return true;
			if (items.First () is ITextItem && modItem is EmpathyStatusItem)
				return true;
			return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			ConnectionPresenceType status;
			string message = "";
			try
			{

				if (items.First () is EmpathySavedStatusItem)
				{
					status = (items.First () as EmpathySavedStatusItem).Status;
					message = (items.First () as EmpathySavedStatusItem).Message;
					EmpathyPlugin.SetAvailabilityStatus(status, message);
				} 
				else if (items.First () is EmpathyStatusItem)
				{
					status = (items.First () as EmpathyStatusItem).Status;
					if (modItems.Any ())
						message = (modItems.First () as ITextItem).Text;
					EmpathyPlugin.SetAvailabilityStatus(status, message);
				} 
				else if (items.First () is ITextItem)
				{
					if (modItems.Any ())
						status = (modItems.First () as EmpathyStatusItem).Status;
					else		
						status = ConnectionPresenceType.Available;
					message = (items.First () as ITextItem).Text;
					EmpathyPlugin.SetAvailabilityStatus(status, message);
				}
			}
			catch (Exception e)
			{
				Log<EmpathySetStatusAction>.Error ("Could not set Empathy status: {0}", e.Message);
				Log<EmpathySetStatusAction>.Debug (e.StackTrace);
			} 
			yield break;
		}
	}
}
