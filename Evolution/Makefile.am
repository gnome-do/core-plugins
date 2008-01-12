SUBDIRS=src

ASSEMBLY_NAME=Evolution
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY_NAME).*

evolution_references = $(DO_ADDINS_LIBS) $(DO_DBUS_LIBS) $(EVOLUTION_LIBS)

evolution_sources  = 	\
			src/AssemblyInfo.cs \
			src/EvolutionContactItemSource.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(evolution_sources)
	$(CSC) $(CSFLAGS) $(evolution_references) -target:library -out:$@ $(evolution_sources)
