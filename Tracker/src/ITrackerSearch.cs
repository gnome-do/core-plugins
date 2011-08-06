
using System;
#if USE_DBUS_SHARP
using DBus;
#else
using NDesk.DBus;
#endif

using org.freedesktop.DBus;

namespace Tracker.Dbus
{
	[Interface ("org.freedesktop.Tracker.Search")]
	public interface ITrackerSearch
	{
		string [] Text(int live_query_id, string service, string search_text, int offset, int max_hits);
	}
}
