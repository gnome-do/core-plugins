include $(top_srcdir)/build.rules.mk

ASSEMBLY=GNOME-Terminal

FILES = \
	src/OpenProfileAction.cs \
	src/OpenTerminalHereAction.cs \
	src/ProfileItem.cs \
	src/ProfileItemSource.cs \
	src/RunInTerminalAction.cs

RESOURCES = \
	GNOME-Resources/Terminal.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(GCONF_SHARP_20_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
