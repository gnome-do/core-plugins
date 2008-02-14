SUBDIRS=src

EXTRA_DIST=Banshee.mdp

ASSEMBLY_NAME=Banshee
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

banshee_references = $(DO_ADDINS_LIBS)

banshee_sources  = 	\
			AssemblyInfo.cs \
			Banshee.cs \
			BansheeItems.cs \
			BansheeAction.cs \
			BansheeDBus.cs \
			BansheeItemSource.cs

banshee_build_sources = $(addprefix $(srcdir)/src/, $(banshee_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(banshee_build_sources)
	$(CSC) $(CSFLAGS) $(banshee_references) -target:library -out:$@ $(banshee_build_sources)
