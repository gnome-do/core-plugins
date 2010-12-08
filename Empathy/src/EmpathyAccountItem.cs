//  EmpathyAccountItem.cs
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

using System.IO;

using Do.Universe;
using Do.Platform;

namespace EmpathyPlugin
{
	public class EmpathyAccountItem : Item
	{
		public EmpathyAccountItem (Account account)
		{
			Account = account;
			Proto = Account.proto.ToLower();
		}

		public int Id { get; protected set; }
		public string Proto { get; protected set; }
		public Account Account { get; protected set; }
		
		public override string Name
		{
			get { return Account.name; }
		}

		public override string Description
		{
			get { return Proto; }
		}

		public override string Icon
		{
			get { return EmpathyPlugin.GetProtocolIcon (Account.GetIconName()); }
		}
	}
}
