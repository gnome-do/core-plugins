EXTRA_DIST=

ASSEMBLY_NAME=File
ASSEMBLY=$(ASSEMBLY_NAME).dll

CLEANFILES = $(ASSEMBLY){,.mdb}

CSFLAGS += -debug+ -debug:full

file_references=$(DO_ADDINS_LIBS)

file_sources= \
		CreateEmptyFileAction.cs

file_build_sources = $(file_sources)

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(file_build_sources)
	$(CSC) $(CSFLAGS) $(file_references) -target:library -out:$@ $(file_build_sources)

