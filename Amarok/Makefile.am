EXTRA_DIST = Amarok.cs AmarokItems.cs AmarokItemSource.cs AmarokPlayAction.cs AssemblyInfo.cs MusicItems.cs

ASSEMBLY_NAME=Amarok
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

amarok_references = $(DO_ADDINS_LIBS) -r:Mono.Data.Sqlite -r:System.Data

amarok_sources  = 	\
				Amarok.cs \
				AmarokItems.cs \
				AmarokItemSource.cs \
				AmarokPlayAction.cs \
				AssemblyInfo.cs \
				MusicItems.cs

amarok_build_sources = $(addprefix $(srcdir)/, $(amarok_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(amarok_sources)
	$(CSC) $(CSFLAGS) $(amarok_references) -target:library -out:$@ $(amarok_build_sources)
