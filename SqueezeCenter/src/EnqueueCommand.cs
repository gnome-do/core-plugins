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
using System.Collections.Generic;

using Do.Universe;

namespace SqueezeCenter
{
	public class Enqueue : Act
	{
		public Enqueue ()
		{
		}

		public override string Name {
			get { return "Add to Play Queue"; }
		}

		public override string Description {
			get { return "Add an item to the SqueezeCenter play queue."; }
		}

		public override string Icon {
			get { return "add"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (MusicItem);
			}
		}
		
		public override bool ModifierItemsOptional
		{
			get {
				return !Player.GetAllConnectedPlayers().Any (); 
			}
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modifier)
		{
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes 
		{
			get {
				yield return typeof (Player);	
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			return item is MusicItem && ((MusicItem)item).Available;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			SqueezeCenter.Player player;
			
			if (modItems.Any ()) {
				player = modItems.First () as Player;
			} else {
				Player[] availablePlayers = Player.GetAllConnectedPlayers ();
				if (availablePlayers.Length > 0)
					player = availablePlayers[0];				
				else
					throw new Exception("Could not enqueue items. No player found");
			}

			Server.Instance.AddItemsToPlayer (player, items.Cast<MusicItem>());			
			return null;
		}
	}
}

