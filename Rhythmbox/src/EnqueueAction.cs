//  RhythmboxEnqueueAction.cs
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
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Do.Rhythmbox
{

	public class EnqueueAction : Act
	{

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Add to Play Queue"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Add an item to Rhythmbox's play queue."); }
		}

		public override string Icon {
			get { return "add"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (MusicItem); }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			new Thread ((ThreadStart) delegate {
				Rhythmbox.StartIfNeccessary ();

				foreach (Item item in items) {
					string enqueue;

					enqueue = "--no-present ";
					foreach (SongMusicItem song in
						Rhythmbox.LoadSongsFor (item as MusicItem)) {
						enqueue += string.Format ("--enqueue \"{0}\" ", song.File);
					}
					Rhythmbox.Client (enqueue);
				}
			}).Start ();
			return null;
		}
	}
}
