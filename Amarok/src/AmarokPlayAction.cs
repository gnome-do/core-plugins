//  AmarokPlayAction.cs
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

using Do.Universe;
using Mono.Unix;

namespace Do.Plugins.Amarok
{

	public class AmarokPlayAction : AbstractAction
	{

		public AmarokPlayAction ()
		{
		}

		public override string Name {
			get { return Catalog.GetString ("Play"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Play an item in Amarok."); }
		}

		public override string Icon {
			get { return "amarok"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (MusicItem);
			}
		}

		public override IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modifierItems)
		{
			new Thread ((ThreadStart) delegate {
				string songs = "";
				Amarok.StartIfNeccessary ();

				Amarok.Client ("--pause");
				foreach (IItem item in items) {
					foreach (SongMusicItem song in Amarok.LoadSongsFor (item as MusicItem))
						songs = string.Format ("{0} \"{1}\"", songs, song.File);
					Amarok.Client ("--load " + songs, true);
				}
				Amarok.Client ("--next", true);
				Amarok.Client ("--play");
			}).Start ();
			return null;
		}
	}
}
