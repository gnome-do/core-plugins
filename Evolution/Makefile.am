if HAVE_EVOLUTION
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=Evolution

FILES = \
	src/ContactItemSource.cs


RESOURCES = \
	Resources/Evolution.addin.xml

REFERENCES = \
	System \
	System.Core \
	$(EVOLUTION_SHARP_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)
