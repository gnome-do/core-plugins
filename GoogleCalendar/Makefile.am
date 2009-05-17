if HAVE_GCAL
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=GoogleCalendar

FILES = \
	src/CertHandler.cs \
	src/Configuration.cs \
	src/GCalClient.cs \
	src/GCal.cs \
	src/GCalendarEventItem.cs \
	src/GCalendarItem.cs \
	src/GCalendarItemSource.cs \
	src/GCalendarNewEvent.cs \
	src/GCalendarSearchEvents.cs \
	src/GCalendarViewActions.cs \
	src/GCalPreferences.cs

RESOURCES = \
	Resources/GoogleCalendar.addin.xml \
	Resources/icons/calIcon.png

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Xml \
	$(GDATA_SHARP_CALENDAR_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
