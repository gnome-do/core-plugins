//xmms2ClearAction.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Do.Universe;

namespace Do.Addins.xmms2{
	
	public class xmms2ClearAction : Act{
		public xmms2ClearAction (){
		}

		public override string Name{
			get { return "Clear Playlist"; }
		}

		public override string Description{
			get { return "Clear selected xmms2 Playlist"; }
		}

		public override string Icon{
			get { return "edit-clear"; }
		}

		public override IEnumerable<Type> SupportedItemTypes{ 
			get {
				return new Type[] {
					typeof (PlaylistItem),
				};
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems){
			new Thread ((ThreadStart) delegate {
				xmms2.StartIfNeccessary();
				foreach (Item item in items){
					xmms2.Client(string.Format("clear {0}", item.Name));
				}
			}).Start();
			return null;
		}
	}
}
