//xmms2PlaylistActions.cs
using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Do.Universe;

namespace Do.Addins.xmms2{
	public class xmms2LoadAction : Act{
		
		public xmms2LoadAction(){
		}
		public override string Name{
			get{
				return "Load Playlist";
			}
		}
		public override string Description{
			get{
				return "Switch current xmms2 playlist";
			}
		}
		public override string Icon{
			get{
				return "edit-redo";
			}
		}
		
		public override IEnumerable<Type> SupportedItemTypes{
			get{
				return new Type[] {
					typeof (PlaylistItem),
				};
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems){
			string command = string.Format("playlist load {0}", items.First ().Name);	
			xmms2.Client(command, true);
			yield break;
		}
	}
	
	public class xmms2ShuffleAction : Act{
		public xmms2ShuffleAction (){
		}

		public override string Name{
			get { return "Shuffle Playlist"; }
		}

		public override string Description{
			get { return "Shuffle selected xmms2 Playlist"; }
		}

		public override string Icon{
			get { return "media-playlist-shuffle"; }
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
				xmms2.StartIfNeccessary ();
				foreach (Item item in items) {
					xmms2.Client (string.Format ("shuffle {0}", item.Name));
				}
			}).Start ();
			yield break;
		}
	}
	
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
				foreach (Item item in items) {
					xmms2.Client(string.Format("coll attr Playlists/{0} jumplist {0}", item.Name));
				}//unfortunately, there is no way to turn off repeat
			}).Start();
			yield break;
		}
	}
	
	public class xmms2CreatePlaylistAction : Act{
		public xmms2CreatePlaylistAction (){
		}

		public override string Name{
			get { return "Create Playlist"; }
		}

		public override string Description{
			get { return "Create new xmms2 playlist"; }
		}

		public override string Icon{
			get { return "document-new"; }
		}

		public override IEnumerable<Type> SupportedItemTypes{ 
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems){
			new Thread ((ThreadStart) delegate {
				xmms2.StartIfNeccessary();
				foreach (Item item in items) { // why not?
					xmms2.Client(string.Format("playlist create {0}", item.Name));
				}
			}).Start();
			yield break;
		}
	}
	public class xmms2RemovePlaylistAction : Act{
		public xmms2RemovePlaylistAction (){
		}

		public override string Name{
			get { return "Remove Playlist"; }
		}

		public override string Description{
			get { return "Remove selected xmms2 playlist"; }
		}

		public override string Icon{
			get { return "gtk-remove"; }
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
				foreach (Item item in items) {
					xmms2.Client(string.Format("playlist remove {0}", item.Name));
				}
			}).Start();
			yield break;
		}
	}
}
