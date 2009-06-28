
using System;

using NDesk.DBus;

using Do.Platform;

namespace Tracker.Dbus
{
	internal class Tracker
	{
		#region Nested types
		
		private sealed class NullSearch : ITrackerSearch
		{
			#region ITrackerSearch implementation
			
			string [] ITrackerSearch.Text (int live_query_id, string service, string search_text, int offset, int max_hits)
			{
				return new string [0];
			}
			
			#endregion
		}
		
		#endregion
		
		#region Constants
		
		private const string BUS_NAME = "org.freedesktop.Tracker";
		private const string OBJECT_PATH = "/org/freedesktop/Tracker/Search";
		
		#endregion
		
		#region Construction
		
		public Tracker()
		{
			try {
				Search = new NullSearch ();
				if (Bus.Session.NameHasOwner (BUS_NAME))
					Search = Bus.Session.GetObject<ITrackerSearch> (BUS_NAME, new ObjectPath (OBJECT_PATH));
			} catch (Exception e) {
				Log<Tracker>.Error ("Error aquiring Tracker dbus object: {0}", e.Message);
				Log<Tracker>.Debug (e.StackTrace);
			}
		}
		
		#endregion
		
		#region Properties
		
		public ITrackerSearch Search { get; private set; }
		
		#endregion
	}
}
