//  xmms2PlayAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
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
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Do.Universe;

namespace Do.Addins.xmms2{

	public class xmms2PlayAction : Act{

		public xmms2PlayAction (){
		}

		public override string Name{
			get { return "Play"; }
		}

		public override string Description{
			get { return "Play an item in xmms2"; }
		}

		public override string Icon{
			get { return "player_play"; }
		}

		public override IEnumerable<Type> SupportedItemTypes{ 
			get {
				return new Type[] {
					typeof (MusicItem),
					typeof (PlaylistItem),
				};
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems){
			new Thread ((ThreadStart) delegate {
				xmms2.StartIfNeccessary ();
				if(items.First () is PlaylistItem){
					xmms2.Client("stop");
					xmms2.Client(string.Format("playlist load {0}", items.First ().Name));
					xmms2.Client("play");
				}else{ //is a music item
					xmms2.Client("playlist load Default", true);
					xmms2.Client ("stop");
					xmms2.Client ("clear", true);
					foreach (Item item in items) {
						string enqueue = "addid ";
						foreach (SongMusicItem song in xmms2.LoadSongsFor (item as MusicItem)){
							enqueue += string.Format("{0} ", song.Id);
						}
						xmms2.Client (enqueue, true);
					}
					xmms2.Client ("next");
					xmms2.Client ("play");
				}
			}).Start ();
			return null;
		}
	}
}
