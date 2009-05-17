
using System;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace Tracker.Dbus
{
	[Interface ("org.freedesktop.Tracker.Search")]
	public interface ITrackerSearch
	{
		string [] Text(int live_query_id, string service, string search_text, int offset, int max_hits);
	}
}
