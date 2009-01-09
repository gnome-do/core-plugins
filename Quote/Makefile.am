include $(top_srcdir)/build.rules.mk

ASSEMBLY=Quote

MCS_FLAGS += -recurse:src/*.cs

RESOURCES = \
	Quote.addin.xml \
	quoted-globe.svg

REFERENCES = \
	System \
	System.Web \
	System.Core \
	Mono.Posix \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
