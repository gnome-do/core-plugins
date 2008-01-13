SUBDIRS=src

ASSEMBLY_NAME=Pidgin

EXTRA_DIST=$(ASSEMBLY_NAME).mdp

ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

pidgin_references = $(DO_ADDINS_LIBS)

pidgin_sources  = 	\
			src/AssemblyInfo.cs \
			src/PidginChatCommand.cs \
			src/PidginContactItemSource.cs \
			src/Pidgin.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(pidgin_sources)
	$(CSC) $(CSFLAGS) $(pidgin_references) -target:library -out:$@ $(pidgin_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
