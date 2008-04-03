SUBDIRS = src

ASSEMBLY_NAME=Thunderbird

EXTRA_DIST=$(ASSEMBLY_NAME).mdp

ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

thunderbird_references = $(DO_ADDINS_LIBS)

thunderbird_sources =	\
			AssemblyInfo.cs \
			Mork.cs \
			ThunderbirdContactItemSource.cs

thunderbird_build_sources = $(addprefix $(srcdir)/src/, $(thunderbird_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(thunderbird_build_sources)
	$(CSC) $(CSFLAGS) $(thunderbird_references) -target:library -out:$@ $(thunderbird_build_sources)
