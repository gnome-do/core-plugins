ASSEMBLY_NAME=LocateFiles

EXTRA_DIST=$(ASSEMBLY_NAME).mdp

ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

locate_files_references = $(DO_ADDINS_LIBS)

locate_files_sources =	\
			AssemblyInfo.cs \
			LocateFilesCommand.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(locate_files_sources)
	$(CSC) $(CSFLAGS) $(locate_files_references) -target:library -out:$@ $(locate_files_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
