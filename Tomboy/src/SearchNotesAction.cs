// TomboySearchNotesCommand.cs
//
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Tomboy
{

	public class SearchNotesAction : Act
	{

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Search Tomboy Notes"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Searches contents of Tomboy notes."); }
		}
		
		public override string Icon {
			get { return "gtk-find"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			TomboyDBus tb = new TomboyDBus ();
			// This action will start Tomboy if it is not
			// already running.
			tb.SearchNotes ((items.First () as ITextItem).Text);
			yield break;
		}
	}
}
