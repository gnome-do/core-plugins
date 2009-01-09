include $(top_srcdir)/build.rules.mk

ASSEMBLY=Rhythmbox

MCS_FLAGS += -recurse:src/*.cs

RESOURCES = \
	Resources/Rhythmbox.addin.xml

REFERENCES = \
	System \
	System.Core \
	System.Web \
	System.Xml \
	Mono.Posix \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
