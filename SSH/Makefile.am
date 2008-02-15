EXTRA_DIST=

ASSEMBLY_NAME=SSH
ASSEMBLY=$(ASSEMBLY_NAME).dll

CLEANFILES = $(ASSEMBLY){,.mdb}

CSFLAGS += -debug+ -debug:full

ssh_references=$(DO_ADDINS_LIBS) $(GCONF_SHARP_LIBS)


ssh_sources= \
		SSHAction.cs \
		SSHHosts.cs

ssh_build_sources = $(ssh_sources)

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(ssh_build_sources)
	$(CSC) $(CSFLAGS) $(ssh_references) -target:library -out:$@ $(ssh_build_sources)

