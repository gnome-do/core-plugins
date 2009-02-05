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
		const int maxResults = 100;

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
			List<Item> results = new List<Item> ();
			foreach (ITextItem text in items)
				results.AddRange (Search (text.Text));

			return results;
		}

		private List<Item> Search (string query)
		{
			List<Item> files = new List<Item> ();
			try {		
				System.Diagnostics.Process tracker = new System.Diagnostics.Process ();
				tracker.StartInfo.FileName = "tracker-search";
				tracker.StartInfo.Arguments = string.Format ("--limit={0} \"{1}\"", maxResults, query);
				tracker.StartInfo.RedirectStandardOutput = true;
				tracker.StartInfo.UseShellExecute = false;
				tracker.Start ();

				string path;	
				while (null != (path = tracker.StandardOutput.ReadLine ())) {
					files.Add (Services.UniverseFactory.NewFileItem (path) as Item);
				}
			} catch (Exception e) {
				Log<TrackerSearchAction>.Error ("Could not run tracker-search --limit={0} \"{1}\": {1}", maxResults, query, e.Message);
				Log<TrackerSearchAction>.Debug (e.StackTrace);
			}	

			return files;
		}
	}
}

