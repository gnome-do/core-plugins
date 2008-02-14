EXTRA_DIST = GoogleMapAction.cs

ASSEMBLY_NAME=GoogleMapAction
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

google_map_references = $(DO_ADDINS_LIBS)

google_map_sources  = 	\
			GoogleMapAction.cs

google_map_build_sources = $(addprefix $(srcdir)/, $(google_map_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(google_map_sources)
	$(CSC) $(CSFLAGS) $(google_map_references) -target:library -out:$@ $(google_map_build_sources)
