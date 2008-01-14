SUBDIRS=src

EXTRA_DIST=Rhythmbox.mdp

ASSEMBLY_NAME=Rhythmbox
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

rhythmbox_references = $(DO_ADDINS_LIBS)

rhythmbox_sources  = 	\
			AssemblyInfo.cs \
			Rhythmbox.cs \
			RhythmboxItems.cs \
			RhythmboxPlayCommand.cs \
			MusicItems.cs \
			RhythmboxEnqueueCommand.cs \
			RhythmboxItemSource.cs

rhythmbox_build_sources = $(addprefix $(srcdir)/src/, $(rhythmbox_sources))

all: $(ASSEMBLY)

$(ASSEMBLY): $(rhythmbox_build_sources)
	$(CSC) $(CSFLAGS) $(rhythmbox_references) -target:library -out:$@ $(rhythmbox_build_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
