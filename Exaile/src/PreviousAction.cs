//  PreviousAction.cs
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

using Mono.Unix;

using Do.Universe;


namespace Do.Exaile
{

	public class PreviousAction : AbstractPlaybackAction
	{

		public PreviousAction ()
		{
		}

		public override string Name {
			get { return Catalog.GetString ("Previous"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Skip to the previous track in Exaile."); }
		}

		public override string Icon {
			get { return "media-skip-backward"; }
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			new Thread ((ThreadStart) delegate {
				Exaile.Client ("--prev", true);
				Exaile.Client ("--prev");
			}).Start ();
			return null;
		}
	}
}
