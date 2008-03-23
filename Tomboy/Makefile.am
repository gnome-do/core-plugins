EXTRA_DIST = 	\
			TomboyDBus.cs \
			TomboyItem.cs \
			TomboyItemSource.cs \
			TomboyNewNoteCommand.cs \
			TomboySearchNotesCommand.cs


ASSEMBLY_NAME=Tomboy
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

tomboy_references = $(DO_ADDINS_LIBS) $(DO_LIBS) -r:System $(NDESK_DBUS_LIBS)

tomboy_sources  = 	\
			TomboyDBus.cs \
			TomboyItem.cs \
			TomboyItemSource.cs \
			TomboyNewNoteCommand.cs \
			TomboySearchNotesCommand.cs

tomboy_build_sources = $(addprefix $(srcdir)/, $(tomboy_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(tomboy_sources)
	$(CSC) $(CSFLAGS) $(tomboy_references) -target:library -out:$@ $(tomboy_build_sources)
