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
using System.Linq;
using Do.Universe;
using System.Collections.Generic;

namespace SqueezeCenter.PlayerCommands
{
	
	public abstract class PlayerCommand : Act
	{
		string name;
		string description;
		string icon;
		IEnumerable<PlayerStatus> requiredPlayerStatus;
		
		public PlayerCommand(string name, string description, string icon, IEnumerable<PlayerStatus> requiredPlayerStatus)
		{
			this.name = name;
			this.description = description;
			this.icon = icon;
			this.requiredPlayerStatus = requiredPlayerStatus;
		}
		
		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return description; }
		}

		public override string Icon {
			get { return icon; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (Player);
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			Player player = (Player) item;
			return player.Available && this.requiredPlayerStatus.Contains (player.Status);	
		}
		
		public abstract string GetCommand (Player player, Item modifierItem);
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			string command = GetCommand (items.First () as Player, modifierItems.FirstOrDefault ());
			Server.Instance.ExecuteCommand (command);
		
			return null;
		}
	}	
}
