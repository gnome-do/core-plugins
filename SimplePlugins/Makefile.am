plugins_sources := $(wildcard *.cs)
plugins_binaries := $(patsubst %.cs, %.dll, $(plugins_sources))

plugins_build_sources := $(addprefix $(srcdir)/, $(plugins_sources))

EXTRA_DIST = $(plugins_sources)

CSFLAGS+= -debug+ -debug:full

plugins_references = $(DO_ADDINS_LIBS) $(DO_DBUS_LIBS)

CLEANFILES = $(plugins_binaries) $(foreach dll,$(plugins_binaries), $(dll).mdb)

plugin_DATA = $(plugins_binaries)

all: $(plugins_binaries)

%.dll : %.cs
	$(CSC) $(CSFLAGS) $(plugins_references) -target:library -out:$@ $<
