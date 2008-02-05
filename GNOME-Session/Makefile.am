EXTRA_DIST = PowerManagement.cs ScreenSaver.cs SessionCommandItem.cs SessionCommandsItemSource.cs

ASSEMBLY_NAME=GNOME-Session
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

gnome_session_references = $(DO_ADDINS_LIBS)

gnome_session_sources  = 	\
		PowerManagement.cs \
		ScreenSaver.cs \
		SessionCommandItem.cs \
		SessionCommandsItemSource.cs

gnome_session_build_sources = $(addprefix $(srcdir)/, $(gnome_session_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(gnome_session_sources)
	$(CSC) $(CSFLAGS) $(gnome_session_references) -pkg:ndesk-dbus-1.0 -target:library -out:$@ $(gnome_session_build_sources)
