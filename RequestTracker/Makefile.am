include $(top_srcdir)/build.rules.mk

ASSEMBLY=RequestTracker

FILES = \
  gtk-gui/generated.cs \
  gtk-gui/RequestTracker.RTPrefs.cs \
  src/Configuration.cs \
  src/Preferences.cs \
	src/RequestTrackerAction.cs \
  src/RequestTrackerItem.cs \
  src/RequestTrackerItems.cs

RESOURCES = \
	Resources/RequestTracker.addin.xml \
	Resources/icons/rt.png

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
