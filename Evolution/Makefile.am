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

all: $(ASSEMBLY)

$(ASSEMBLY): $(evolution_build_sources)
	$(CSC) $(CSFLAGS) $(evolution_references) -target:library -out:$@ $(evolution_build_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
