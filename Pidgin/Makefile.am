SUBDIRS=src

ASSEMBLY_NAME=Pidgin

EXTRA_DIST=$(ASSEMBLY_NAME).mdp

ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

pidgin_references = $(DO_ADDINS_LIBS)

pidgin_sources  = 	\
			AssemblyInfo.cs \
			PidginChatAction.cs \
			PidginContactItemSource.cs \
			PidginHandleContactDetailItem.cs \
			Pidgin.cs

pidgin_build_sources = $(addprefix $(srcdir)/src/, $(pidgin_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(pidgin_build_sources)
	$(CSC) $(CSFLAGS) $(pidgin_references) -target:library -out:$@ $(pidgin_build_sources)
