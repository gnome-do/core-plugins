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
			RhythmboxPlayAction.cs \
			MusicItems.cs \
			RhythmboxEnqueueAction.cs \
			RhythmboxItemSource.cs

rhythmbox_build_sources = $(addprefix $(srcdir)/src/, $(rhythmbox_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(rhythmbox_build_sources)
	$(CSC) $(CSFLAGS) $(rhythmbox_references) -target:library -out:$@ $(rhythmbox_build_sources)
