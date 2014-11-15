//  RhythmboxPlayItemAction.cs
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

using Mono.Addins;

using Do.Universe;

namespace Do.Rhythmbox
{
	public class PlayItemAction : Act
	{
		public PlayItemAction ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Play"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Play an item in Rhythmbox."); }
		}

		public override string Icon {
			get { return "rhythmbox"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (MusicItem);
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			new Thread ((ThreadStart) delegate {
				Rhythmbox.StartIfNeccessary ();

				Rhythmbox.Client ("--pause --no-present");
				Rhythmbox.Client ("--clear-queue --no-present", true);

				if (items.Count() == 1 && items.ElementAt(0) is PlaylistMusicItem) {
					// Because playlists are not enqueued, but played as is, playing
					// more than one does not make any sense. Neither does playing e.g.
					// one playlist and two songs.
					// Therefore, only the case where the lone item selected is a playlist is supported.

					RhythmboxDBus.PlayPlaylist((PlaylistMusicItem)items.ElementAt(0));
				}
				else {
					foreach (Item item in items) {
						if (item is MusicItem && !(item is PlaylistMusicItem)) {
							string enqueue = "--no-present ";
							foreach (SongMusicItem song in Rhythmbox.LoadSongsFor (item as MusicItem))
								enqueue = string.Format ("{0} --enqueue \"{1}\" ", enqueue, song.File);
							Rhythmbox.Client (enqueue, true);
						}
					}
				}

				Rhythmbox.Client ("--next --no-present");
				Rhythmbox.Client ("--play --no-present");
			}).Start ();
			return null;
		}
	}
}
