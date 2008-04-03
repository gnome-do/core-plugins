EXTRA_DIST = EpiphanyBookmarkItemSource.cs

ASSEMBLY_NAME=Epiphany
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

epiphany_references = $(DO_ADDINS_LIBS)

epiphany_sources  = 	\
			EpiphanyBookmarkItemSource.cs

epiphany_build_sources = $(addprefix $(srcdir)/, $(epiphany_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(epiphany_sources)
	$(CSC) $(CSFLAGS) $(epiphany_references) -target:library -out:$@ $(epiphany_build_sources)
