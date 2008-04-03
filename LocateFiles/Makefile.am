ASSEMBLY_NAME=LocateFiles

EXTRA_DIST=$(ASSEMBLY_NAME).mdp AssemblyInfo.cs LocateFilesAction.cs

ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

locate_files_references = $(DO_ADDINS_LIBS)

locate_files_sources =	\
			AssemblyInfo.cs \
			LocateFilesAction.cs

locate_files_build_sources = $(addprefix $(srcdir)/, $(locate_files_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(locate_files_build_sources)
	$(CSC) $(CSFLAGS) $(locate_files_references) -target:library -out:$@ $(locate_files_build_sources)
