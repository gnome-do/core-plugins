//xmms2RepeatAction.cs
using System;
using System.Threading;
using System.Diagnostics;

using Do.Universe;

namespace Do.Addins.xmms2{
	
	public class xmms2RepeatAction : Act{
		public xmms2RepeatAction (){
		}

		public override string Name{
			get { return "Repeat"; }
		}

		public override string Description{
			get { return "Turn on repeat for selected xmms2 playlist"; }
		}

		public override string Icon{
			get { return "media-playlist-repeat"; }
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
				xmms2.Client(string.Format("coll attr Playlists/{0} jumplist {0}", items.First().Name));
				//unfortunately, there is no way to turn off repeat
			}).Start();
			return null;
		}
	}
}
