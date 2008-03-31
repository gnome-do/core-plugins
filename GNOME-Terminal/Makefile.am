EXTRA_DIST = OpenTerminalHereAction.boo \
			 RunInTerminalAction.boo

ASSEMBLY_NAME=GNOME-Terminal
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+=

CLEANFILES = $(ASSEMBLY){,.mdb}

gnome_terminal_references = $(DO_ADDINS_LIBS)

gnome_terminal_sources  = 	\
			OpenTerminalHereAction.boo \
			RunInTerminalAction.boo

gnome_terminal_build_sources = $(gnome_terminal_sources)

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(gnome_terminal_sources)
	$(BOOC) $(CSFLAGS) $(gnome_terminal_references) -target:library -out:$@ $(gnome_terminal_build_sources)
