include $(top_srcdir)/build.rules.mk

ASSEMBLY=GCalendar

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
	Resources/GCalendar.addin.xml \
	Resources/calIcon.png

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Xml \
	$(GDATA_SHARP_ACL_LIBS) \
	$(GDATA_SHARP_CALENDAR_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
