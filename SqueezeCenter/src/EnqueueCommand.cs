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
				return new Type[] {
					typeof (MusicItem),
				};
			}
		}
		
		public override bool ModifierItemsOptional
		{
			get {
				return Server.Instance.GetConnectedPlayers ().Count == 0; 
			}
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem(Item item) 
		{
			// this converts converts the list of players to an array of Item
			return Server.Instance.GetConnectedPlayersAsItem ().ToArray ();
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modifier)
		{
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes 
		{
			get {
				return new Type[] {
					typeof (Player),
				};		
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			return (item is MusicItem);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Player player;			
						
			if (modItems.Any ()) {
				player = modItems.First () as Player;
			}
			else {
				List<Player> availablePlayers = Server.Instance.GetConnectedPlayers ();
				if (availablePlayers.Count > 0) {
					player = availablePlayers[0];				
				}
				else {
					throw new Exception("Could not enqueue items. No player found");
				}
			}

			Server.Instance.AddItemsToPlayer (player, Util.Cast<Item, MusicItem>(items));			
			return null;
		}
	}
}

