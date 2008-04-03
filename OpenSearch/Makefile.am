SUBDIRS=src

ASSEMBLY_NAME=OpenSearch
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

open_search_references = $(DO_ADDINS_LIBS) -r:Mono.Posix.dll -r:System.Web

open_search_sources  = 	\
    AssemblyInfo.cs \
    OpenSearch.cs \
    FirefoxOpenSearchFileProvider.cs \
    IOpenSearchFile.cs \
    IOpenSearchFileProvider.cs \
    IOpenSearchItem.cs \
    OpenSearchItem.cs \
    MozillaSearchFile.cs  \
    OpenSearchAction.cs  \
    OpenSearchFile.cs \
    OpenSearchFileManager.cs \
    OpenSearchFileType.cs \
    OpenSearchFileTypeIdentifier.cs \
    XmlNamespaceHelper.cs

open_search_build_sources = $(addprefix $(srcdir)/src/, $(open_search_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(open_search_build_sources)
	$(CSC) $(CSFLAGS) $(open_search_references) -target:library -out:$@ $(open_search_build_sources)
