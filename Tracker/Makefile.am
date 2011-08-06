include $(top_srcdir)/build.rules.mk

ASSEMBLY=TrackerSearch

FILES = \
	src/Tracker.cs \
	src/TrackerSearchAction.cs \
	src/ITrackerSearch.cs

RESOURCES = \
	Resources/TrackerSearch.addin.xml 

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Core \
	$(DBUS_SHARP_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
