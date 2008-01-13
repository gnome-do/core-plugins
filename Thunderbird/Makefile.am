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

all: $(ASSEMBLY)

$(ASSEMBLY): $(thunderbird_build_sources)
	$(CSC) $(CSFLAGS) $(thunderbird_references) -target:library -out:$@ $(thunderbird_build_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
