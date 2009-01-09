include $(top_srcdir)/build.rules.mk

ASSEMBLY=Riptide

MCS_FLAGS += -recurse:src/*.cs -recurse:gtk-gui/*.cs

REFERENCES = \
	System \
	System.Core \
	System.Web \
	System.Xml \
	Mono.Posix \
	Mono.Cairo \
	$(GCONF_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

RESOURCES = \
	Riptide.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

