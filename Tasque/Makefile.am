EXTRA_DIST = Tasque.cs \
             TasqueAction.cs \
             TasqueCategoryItem.cs \
             TasqueDBus.cs
             
ASSEMBLY_NAME=Tasque
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

tasque_references = $(DO_ADDINS_LIBS) -r:System -r:Mono.Posix $(NDESK_DBUS_LIBS) $(GCONF_SHARP_LIBS)

tasque_sources  = 	Tasque.cs \
                        TasqueAction.cs \
                        TasqueCategoryItem.cs \
                        TasqueDBus.cs

tasque_build_sources = $(addprefix $(srcdir)/, $(tasque_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(tasque_build_sources)
	$(CSC) $(CSFLAGS) $(tasque_references) -target:library -out:$@ $(tasque_build_sources)
