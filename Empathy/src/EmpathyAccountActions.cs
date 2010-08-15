//  EmpathyAccountActions.cs
//  
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//  
//  Copyright (c) 2010 
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

namespace EmpathyPlugin
{

	public class EmpathyEnableAccount : Act
	{
		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Sign on"); }
		}

		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Enable empathy account"); }
		}

		public override string Icon
		{
			get { return "empathy"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return	typeof (EmpathyAccountItem); }
		}
		
		public override bool SupportsItem (Item item)
		{
			if(! (item is EmpathyAccountItem)) 
			{
				return false;
			}
			EmpathyAccountItem accountItem = (item as EmpathyAccountItem);
			return ! accountItem.Account.IsConnected();
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			EmpathyAccountItem account = items.First () as EmpathyAccountItem;
			account.Account.EnableAccount();
			yield break;
		}		
	}
	
	public class EmpathyDisableAccount : Act
	{
		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Sign off"); }
		}

		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Disable empathy account"); }
		}

		public override string Icon
		{
			get { return "empathy"; }
		}

		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (EmpathyAccountItem); }
		}

		public override bool SupportsItem (Item item)
		{
			if(! (item is EmpathyAccountItem)) 
			{
				return false;
			}
			EmpathyAccountItem accountItem = (item as EmpathyAccountItem);
			return accountItem.Account.IsConnected();

		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			EmpathyAccountItem account = items.First () as EmpathyAccountItem;
			account.Account.DisableAccount();
			
			yield break;
		}
	}
}
