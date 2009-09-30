// Prev.cs created with MonoDevelop
// User: anders at 18:14 18-01-2009
//
////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 3 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Do.Universe;

namespace SqueezeCenter.PlayerCommands
{
	
	public class Prev : PlayerCommand 
	{
		public Prev () : base(
		                      "Previous", "Previous track", "gtk-media-previous-ltr",
		                      new PlayerStatus[] {PlayerStatus.Playing, PlayerStatus.Paused, PlayerStatus.Stopped}
		) {}
		
		public override string GetCommand (Player player, Item modifierItem)
		{
			return string.Format ("{0} playlist index -1", player.Id);
		}
		
	}
}
