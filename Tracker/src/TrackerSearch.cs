// TrackerSearch.cs 
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
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
//


using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace TrackerSearch
{

  public class TrackerSearchAction : Act
  {

    public override string Name {
      get { return Catalog.GetString ("Search with Tracker"); }
    }

    public override string Description {
      get { return Catalog.GetString ("Launches Tracker with the given query."); }
    }

    public override string Icon {
      get { return "tracker"; }
    }
    public override IEnumerable<Type> SupportedItemTypes {
      get { yield return typeof (ITextItem); }
    }

    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
    {
			foreach (ITextItem text in items)
				Search (text.Text);

      yield break;
    }

		void Search (string query)
    {
      try {
        Process.Start ("tracker-search-tool", string.Format ("\"{0}\"", query));
      } catch (Exception e) {
        Log<TrackerSearchAction>
					.Error ("Could not run tracker-search-tool {0}: {1}", query, e.Message);
        Log<TrackerSearchAction>.Debug (e.StackTrace);
      }
    }
  }
}

