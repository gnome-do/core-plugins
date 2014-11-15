//  RhythmboxNextAction.cs
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
	public class NextAction : AbstractPlaybackAction
	{
		public NextAction ()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Next"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Skip to the next track in Rhythmbox."); }
		}

		public override string Icon {
			get { return "media-skip-forward"; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			new Thread ((ThreadStart) delegate {
				Rhythmbox.Client ("--next --no-start");
			}).Start ();
			return null;
		}
	}
}
