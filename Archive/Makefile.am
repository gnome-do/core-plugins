EXTRA_DIST=

ASSEMBLY_NAME=Archive
ASSEMBLY=$(ASSEMBLY_NAME).dll

CLEANFILES = $(ASSEMBLY){,.mdb}

CSFLAGS += -debug+ -debug:full

appendtext_references=$(DO_ADDINS_LIBS)

appendtext_sources= \
		ExtractAction.cs

appendtext_build_sources = $(appendtext_sources)

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(appendtext_build_sources)
	$(CSC) $(CSFLAGS) $(appendtext_references) -target:library -out:$@ $(appendtext_build_sources)

