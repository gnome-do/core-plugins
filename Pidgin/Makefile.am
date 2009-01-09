include $(top_srcdir)/build.rules.mk

ASSEMBLY=Pidgin

# Spaces suck for autofoo, so we need to handle this differently
MCS_FLAGS += -recurse:*.cs

RESOURCES = \
	Resources/internet-group-chat.svg \
	Resources/Pidgin.addin.xml

REFERENCES = \
	System \
	System.Core \
	System.Web \
	System.Xml \
	Mono.Posix \
	$(GTK_SHARP_20_LIBS) \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
