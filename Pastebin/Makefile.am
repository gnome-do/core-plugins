SUBDIRS=src

ASSEMBLY_NAME=Pastebin
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

pastebin_references = $(DO_ADDINS_LIBS) -r:Mono.Posix.dll -r:System.Web

pastebin_sources  = 	\
    AssemblyInfo.cs \
    IPastebinProvider.cs \
    ITextSyntaxItem.cs \
    Paste2.cs \
    Pastebin.cs \
    PastebinAction.cs \
    TextSyntaxItem.cs

pastebin_build_sources = $(addprefix $(srcdir)/src/, $(pastebin_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(pastebin_build_sources)
	$(CSC) $(CSFLAGS) $(pastebin_references) -target:library -out:$@ $(pastebin_build_sources)
