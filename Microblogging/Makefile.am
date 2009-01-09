include $(top_srcdir)/build.rules.mk

ASSEMBLY=Microblog

MCS_FLAGS += -recurse:*.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/Microblogging.addin.xml \
	Resources/twitter-icon.svg \
	Resources/twitter_items.svg

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
