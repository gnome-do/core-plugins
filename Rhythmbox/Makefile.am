SUBDIRS=src

EXTRA_DIST=Rhythmbox.mdp

ASSEMBLY_NAME=Rhythmbox
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

rhythmbox_references = $(DO_ADDINS_LIBS)

rhythmbox_sources  = 	\
			src/AssemblyInfo.cs \
			src/Rhythmbox.cs \
			src/RhythmboxItems.cs \
			src/RhythmboxPlayCommand.cs \
			src/MusicItems.cs \
			src/RhythmboxEnqueueCommand.cs \
			src/RhythmboxItemSource.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(rhythmbox_sources)
	$(CSC) $(CSFLAGS) $(rhythmbox_references) -target:library -out:$@ $(rhythmbox_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
