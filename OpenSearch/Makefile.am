
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/OpenSearch.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

OPENSEARCH_DLL_MDB_SOURCE=bin/Debug/OpenSearch.dll.mdb
OPENSEARCH_DLL_MDB=$(BUILD_DIR)/OpenSearch.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/OpenSearch.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

OPENSEARCH_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(OPENSEARCH_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(OPENSEARCH_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/AssemblyInfo.cs \
	src/OpenSearchAction.cs \
	src/OpenSearchItem.cs \
	src/OpenSearchItemSource.cs \
	src/OpenSearchParser.cs \
	src/IOpenSearchItem.cs \
	src/FirefoxOpenSearchDirectoryProvider.cs 

DATA_FILES = 

RESOURCES = \
	src/OpenSearch.addin.xml 

EXTRAS = \
	opensearch.pc.in 

REFERENCES =  \
	System \
	System.Xml \
	Mono.Posix \
	System.Web \
	Do.Addins

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

OPENSEARCH_PC = $(BUILD_DIR)/opensearch.pc

$(eval $(call emit-deploy-wrapper,OPENSEARCH_PC,opensearch.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
