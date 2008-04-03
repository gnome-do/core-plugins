EXTRA_DIST = 	\
			DoGCal.cs GCalendarItemSource.cs \
			GCalendarItem.cs GCalendarEventItem.cs \
			GCalendarSearchEvents.cs GCalendarNewEvent.cs \
			Google.GData.AccessControl.dll Google.GData.Calendar.dll \
			Google.GData.Client.dll Google.GData.Extensions.dll


ASSEMBLY_NAME=GCalendar
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

gcalendar_google_references = \
    Google.GData.AccessControl.dll Google.GData.Calendar.dll \
    Google.GData.Client.dll Google.GData.Extensions.dll
        
gcalendar_google_build_reference = $(addprefix -r:$(srcdir)/,$(gcalendar_google_references))

gcalendar_references = $(DO_ADDINS_LIBS) $(DO_LIBS) -r:System $(NDESK_DBUS_LIBS) \
	$(GCONF_SHARP_LIBS) $(gcalendar_google_build_reference)

gcalendar_sources  = 	\
			DoGCal.cs GCalendarItemSource.cs \
			GCalendarItem.cs GCalendarEventItem.cs \
			GCalendarSearchEvents.cs GCalendarNewEvent.cs

gcalendar_build_sources = $(addprefix $(srcdir)/, $(gcalendar_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(gcalendar_sources)
	$(CSC) $(CSFLAGS) $(gcalendar_references) -target:library -out:$@ $(gcalendar_build_sources)
