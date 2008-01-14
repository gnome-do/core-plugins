ASSEMBLY_NAME=LocateFiles

EXTRA_DIST=$(ASSEMBLY_NAME).mdp AssemblyInfo.cs LocateFilesCommand.cs

ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

locate_files_references = $(DO_ADDINS_LIBS)

locate_files_sources =	\
			AssemblyInfo.cs \
			LocateFilesCommand.cs

locate_files_build_sources = $(addprefix $(srcdir)/, $(locate_files_sources))

all: $(ASSEMBLY)

$(ASSEMBLY): $(locate_files_build_sources)
	$(CSC) $(CSFLAGS) $(locate_files_references) -target:library -out:$@ $(locate_files_build_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)
