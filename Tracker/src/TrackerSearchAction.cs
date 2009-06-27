
using System;
using System.Collections.Generic;

using Do.Platform;
using Do.Universe;

using Mono.Addins;

namespace TrackerSearch
{
	public class TrackerSearchAction : Act
	{
		const int maxResults = 100;

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Search with Tracker"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Launches Tracker with the given query."); }
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
				string [] results = new Tracker.Dbus.Tracker ().Search.Text (-1, "Files", query, 0, maxResults);
				foreach (string result in results) {
					files.Add (Services.UniverseFactory.NewFileItem (result) as Item);						
				}
			} catch (Exception e) {
				Log<TrackerSearchAction>.Error ("Error occurred while searching Tracker for {0}: {1}", query, e.Message);
				Log<TrackerSearchAction>.Debug (e.StackTrace);
			}	

			return files;
		}
	}
}
