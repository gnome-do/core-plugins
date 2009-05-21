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

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
