include $(top_srcdir)/build.rules.mk

ASSEMBLY=GCalendar

FILES = \
	src/CertHandler.cs \
	src/GCalendarSearchEvents.cs \
	src/GCalPreferences.cs \
	src/GCalendarEventItem.cs \
	src/GCalClient.cs \
	src/GCalendarNewEvent.cs \
	src/GCal.cs \
	src/Configuration.cs \
	src/GCalendarItem.cs \
	src/GCalendarViewActions.cs \
	src/GCalendarItemSource.cs

RESOURCES = \
	resources/calIcon.png \
	GCalendar.addin.xml 

EXTRAS = \
	gcalendar.pc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Xml \
	Mono.Posix \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	../BundledLibraries/Google.GData.AccessControl.dll \
	../BundledLibraries/Google.GData.Calendar.dll \
	../BundledLibraries/Google.GData.Client.dll \
	../BundledLibraries/Google.GData.Extensions.dll
