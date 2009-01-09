include $(top_srcdir)/build.rules.mk

ASSEMBLY=Microblog

# Spaces suck for autofoo, so we need to handle this differently
MCS_FLAGS += -recurse:*.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/Microblogging.addin.xml \
	twitter-icon.svg \
	twitter_items.svg

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
