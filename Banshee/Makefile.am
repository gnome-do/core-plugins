include $(top_srcdir)/build.rules.mk

ASSEMBLY=Banshee

FILES = \
	src/Banshee.cs \
	src/BansheeDBus.cs \
	src/NextAction.cs \
	src/PauseAction.cs \
	src/PlayAction.cs \
	src/PreviousAction.cs 

RESOURCES = \
	Resources/Banshee.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
