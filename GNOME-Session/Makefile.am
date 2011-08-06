include $(top_srcdir)/build.rules.mk

ASSEMBLY=GNOME-Session

FILES = \
	src/PowerManagement.cs \
	src/ScreenSaver.cs \
	src/SessionCommandItem.cs \
	src/SessionCommandsItemSource.cs \
	src/SystemManagement.cs

RESOURCES = \
	Resources/GNOME-Session.addin.xml

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Core \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
