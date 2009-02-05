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
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Do.Universe;

namespace SqueezeCenter
{
	
	public class Player : Item
	{
		string id, name, model;
		bool connected, poweredOn, canPowerOff;
		List<Player> syncedWith;
		string syncedWithStr;
		
		public Player(string id, string name, string model, bool connected, bool poweredOn, bool canPowerOff)
		{
			this.id = id;
			this.name = name;
			this.model = model;
			this.connected = connected;
			this.poweredOn = poweredOn;
			this.canPowerOff = canPowerOff;
			this.syncedWith = new List<Player> ();
			//Console.WriteLine( "Created: " + name);
		}
		
		public string Id 
		{
			get {
				return id;
			}
		}
		
		public override string Name 
		{
			get {
				return name;
			}
		}
		
		
		public override string Icon 
		{
			get {
				return (this.poweredOn ? "SB_on" : "SB_off") + ".png@" + this.GetType ().Assembly.FullName;				
			}
		}		
		
		public override string Description 
		{
			get {
				// make local copy of synchedWithStr, as it is set from a thread
				string syncStr = this.syncedWithStr;
				
				return string.Format("{0} ({1}){2}", 
				                     this.model, 
				                     this.poweredOn ? "On" : "Off", 
				                     syncStr == null ? string.Empty : " synced with " + syncStr);
			}
		}
		
		public bool PoweredOn 
		{
			get {				
				return this.poweredOn;
			}
			set {
				this.poweredOn = value;				
			}
		}
		
		public bool CanPowerOff
		{
			get {
				return this.canPowerOff;
			}
			set {
				this.canPowerOff = value;
			}
		}
		
		public bool Connected
		{
			get {
				return this.connected;
			}
			set {
				this.connected = value;
			}
		}
		
		public Player[] SyncedPlayers
		{
			get {
				lock (this.syncedWith) {
					return this.syncedWith.ToArray ();
				}
			}
		}
		
		public void SetSynchedPlayers (IEnumerable<Player> players) {
			
			StringBuilder syncStr = new StringBuilder ();
			
			lock (this.syncedWith) {
				this.syncedWith.Clear ();
				if (players != null)
					this.syncedWith.AddRange (players);
								
				foreach (Player p in this.syncedWith) {
					syncStr.AppendFormat ("{0}, ", p.name);
				}
				
				System.Threading.Interlocked.Exchange<string> (ref this.syncedWithStr, 
				                                               syncStr.Length == 0 ? null : syncStr.ToString (0, syncStr.Length-2));
			}
		}
		
		public bool IsSynced
		{
			get {
				lock (this.syncedWith) 
					return this.syncedWith.Count > 0;
			}
		}	
	}
}
