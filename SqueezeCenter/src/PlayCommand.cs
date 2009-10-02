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
using System.IO;
using System.Reflection;

using Do.Universe;

namespace SqueezeCenter
{

	public class Play : Act
	{
		
		public Play ()
		{
		}

		public override string Name {
			get { return "Play"; }
		}

		public override string Description {
			get { return "Play item with a SqueezeCenter player"; }
		}

		public override string Icon {
			get {
				return "gtk-media-play-ltr";
			}
		}		
				
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (MusicItem);
				yield return typeof (RadioSubItem);
			}	
		}
					
		
		public override bool ModifierItemsOptional
		{
			get {
				return !Player.GetAllConnectedPlayers ().Any (); 
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
			return (item is MusicItem || item is RadioSubItem) && (item is SqueezeCenterItem && ((SqueezeCenterItem)item).Available);			
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Player player;			
			
			if (modItems.Any ()) {
				player = modItems.First () as Player;
			}
				
			else {
				IList<Player> availablePlayers = Player.GetAllConnectedPlayers ();
				if (availablePlayers.Count > 0) {
					player = availablePlayers[0];				
				}
				else {
					throw new Exception("Could not play items. No player found");
				}
			}
				
			if (items.First () is MusicItem) {
					Server.Instance.LoadItemsToPlayer (player, items.OfType<MusicItem> ());
			}
			else if (items.First () is RadioSubItem) {
				Server.Instance.ExecuteCommand (string.Format ("{0} {1} playlist play item_id:{2}",
				                                                 player.Id,
				                                                 (items.First () as RadioSubItem).GetSuper ().Command,
				                                                 (items.First () as RadioSubItem).IdPath));
			}
				
			return null;
		}
	}
}
