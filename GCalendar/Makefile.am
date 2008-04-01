EXTRA_DIST = 	\
			DoGCal.cs GCalendarItemSource.cs \
			GCalendarItem.cs GCalendarEventItem.cs \
			GCalendarSearchEvents.cs GCalendarNewEvent.cs


ASSEMBLY_NAME=GCalendar
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

gcalendar_references = $(DO_ADDINS_LIBS) $(DO_LIBS) -r:System $(NDESK_DBUS_LIBS) \
	$(GCONF_SHARP_LIBS) \
	-r:Google.GData.AccessControl.dll \
	-r:Google.GData.Calendar.dll \
	-r:Google.GData.Client.dll \
	-r:Google.GData.Extensions.dll

gcalendar_sources  = 	\
			DoGCal.cs GCalendarItemSource.cs \
			GCalendarItem.cs GCalendarEventItem.cs \
			GCalendarSearchEvents.cs GCalendarNewEvent.cs

gcalendar_build_sources = $(addprefix $(srcdir)/, $(gcalendar_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(gcalendar_sources)
	$(CSC) $(CSFLAGS) $(gcalendar_references) -target:library -out:$@ $(gcalendar_build_sources)
