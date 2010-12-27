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

namespace SqueezeCenter
{
	public abstract class PlayerCommand : Act
	{
		string name;
		string description;
		string icon;
		PlayerStatus requiredPlayerStatus;
		
		public PlayerCommand(string name, string description, string icon, PlayerStatus requiredPlayerStatus)
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
				return new Type[] {
					typeof (Player),
				};
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			return (((item as Player).PoweredOn ? 1 : 2) & (int)requiredPlayerStatus) > 0;			
		}
		
		public abstract string GetCommand (Player player, Item modifierItem);
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			string command = GetCommand (items.First () as Player, modifierItems.FirstOrDefault ());
			Server.Instance.ExecuteCommand (command);
		
			return null;
		}
	}
	
	public class TurnOn : PlayerCommand 
	{
		public TurnOn () : base("Turn on", "Turn on the player", "sunny", PlayerStatus.Off) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} power 1", player.Id);
		}
	}
	
	public class TurnOff : PlayerCommand 
	{
		public TurnOff () : base("Turn off", "Turn off the player", "gnome-shutdown", PlayerStatus.On) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} power 0", player.Id);
		}
	}
	
	public class Pause : PlayerCommand 
	{
		public Pause () : base ("Pause", "Toggle pause on the player", "gtk-media-pause", PlayerStatus.On) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} pause", player.Id);
		}
	}
	
	public class Sync : PlayerCommand 
	{
		public Sync () : base("Sync with", "Synchronize this player with another", 
		                      "sync.png@" + typeof (Sync).Assembly.FullName,		
		                      PlayerStatus.Any) {}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { 
				return new Type[] {
					typeof (Player),
				};
			}
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			// all players except the current one
			return !items.Contains (modItem);		
		}
		
		public override bool ModifierItemsOptional
		{
			get {
				return false;
			}
		}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{1} sync {0}", player.Id, (modifierItem as Player).Id);
		}
	}
	
	public class Unsync : PlayerCommand		
	{
		public Unsync () : base("Unsync", "Stop synchronizing this player with another", 
		                        "unsync.png@" + typeof (Unsync).Assembly.FullName, 
		                        //"unsync.png@SqueezeCenterPlugin",
		                        PlayerStatus.Any) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} sync -", player.Id);
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{			
			return false;
		}
		
		public override bool SupportsItem (Item item)
		{
			return base.SupportsItem (item) && (item as Player).IsSynced;
		}
		
		public override bool ModifierItemsOptional
		{
			get {
				return true;
			}
		}		
	}
	
	public class Prev : PlayerCommand 
	{
		public Prev () : base("Previous", "Previous track", "previous", PlayerStatus.On) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} playlist index -1", player.Id);
		}
	}
	
	public class Next : PlayerCommand 
	{
		public Next () : base("Next", "Next track", "next", PlayerStatus.On) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} playlist index +1", player.Id);
		}
	}	
	
	public enum PlayerStatus : int 
	{
		On = 1,
		Off = 2,
		Any = On | Off
	}
}
