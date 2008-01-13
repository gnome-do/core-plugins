SUBDIRS=src

EXTRA_DIST=Evolution.mdp

ASSEMBLY_NAME=Evolution
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

evolution_references = $(DO_ADDINS_LIBS) $(DO_DBUS_LIBS) $(EVOLUTION_LIBS)

evolution_sources  = 	\
			src/AssemblyInfo.cs \
			src/EvolutionContactItemSource.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(evolution_sources)
	$(CSC) $(CSFLAGS) $(evolution_references) -target:library -out:$@ $(evolution_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
