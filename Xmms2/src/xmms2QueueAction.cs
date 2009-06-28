//xmms2QueueAction.cs

using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;


using Do.Universe;

namespace Do.Addins.xmms2{
	
	public class xmms2QueueAction : Act{
		
		public xmms2QueueAction(){
		}
		
		public override string Name {
			get { return "Queue"; }
		}

		public override string Description {
			get { return "Queue an item in an xmms2 playlist"; }
		}

		public override string Icon {
			get { return "gtk-add"; }
		}

		public  override IEnumerable<Type> SupportedItemTypes{
			get {
				return new Type[] {
					typeof (MusicItem),
					typeof (PlaylistItem),
				};
			}
		}
		public override IEnumerable<Type> SupportedModifierItemTypes{
			get{
				return new Type[]{
					typeof(PlaylistItem),
				};
			}
		}

		public override bool ModifierItemsOptional{
			get{
				return true;
			}
		}
		public override bool SupportsItem(Item item){
			return true;
		}
		public override bool SupportsModifierItemForItems(IEnumerable<Item> items, Item modItem){
			return true;
		}
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item){
			yield break;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems){
			//return playlist for loading, DO NOT AUTOLOAD.
			//use Default playlist by default, duh
			string playlist;
			
			new Thread ((ThreadStart) delegate {
				xmms2.StartIfNeccessary ();
				if(!modItems.Any ()){
					playlist = "Default";
				}else{
					playlist = modItems.First ().Name;
				}
				foreach (Item item in items) {
					string enqueue = "addid ";
					if(item is PlaylistItem){ 
						foreach (SongMusicItem song in xmms2.LoadSongsFor (item as PlaylistItem)){
							enqueue += string.Format("{0} {1} ", playlist, song.Id);
						}
					}else{
						foreach (SongMusicItem song in xmms2.LoadSongsFor (item as MusicItem)){
							enqueue += string.Format("{0} {1} ", playlist, song.Id);
						}
					}
					xmms2.Client (enqueue, true);
				}

			}).Start ();
			yield break;
		}

	}
}
