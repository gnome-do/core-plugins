EXTRA_DIST = 	\
			VNCHost.cs
			Vinagre.cs


ASSEMBLY_NAME=Vinagre
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

vinagre_references = $(DO_ADDINS_LIBS) $(DO_LIBS) -r:System $(NDESK_DBUS_LIBS)

tomboy_sources  = 	\
			VNCHost.cs
			Vinagre.cs

vinagre_build_sources = $(addprefix $(srcdir)/, $(tomboy_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(vinagre_sources)
	$(CSC) $(CSFLAGS) $(vinagre_references) -target:library -out:$@ $(vinagre_build_sources)
