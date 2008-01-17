SUBDIRS=src

EXTRA_DIST=Evolution.mdp

ASSEMBLY_NAME=Evolution
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

evolution_references = $(DO_ADDINS_LIBS) $(EVOLUTION_SHARP_LIBS)

evolution_sources  = 	\
			AssemblyInfo.cs \
			EvolutionContactItemSource.cs

evolution_build_sources = $(addprefix $(srcdir)/src/, $(evolution_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(evolution_build_sources)
	$(CSC) $(CSFLAGS) $(evolution_references) -target:library -out:$@ $(evolution_build_sources)
