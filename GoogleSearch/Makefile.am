
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/InlineGoogleSearch.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

INLINEGOOGLESEARCH_DLL_MDB_SOURCE=bin/Debug/InlineGoogleSearch.dll.mdb
INLINEGOOGLESEARCH_DLL_MDB=$(BUILD_DIR)/InlineGoogleSearch.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/InlineGoogleSearch.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

INLINEGOOGLESEARCH_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(INLINEGOOGLESEARCH_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(INLINEGOOGLESEARCH_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/InlineGoogleSearch.cs \
	gtk-gui/generated.cs \
	src/InlineGoogleSearchConfig.cs \
	gtk-gui/InlineGoogleSearch.InlineGoogleSearchConfig.cs \
	src/GoogleSearchResult.cs \
	src/GoogleSearch.cs \
	src/ImFeelingLuckyAction.cs 

DATA_FILES = 

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	InlineGoogleSearch.addin.xml 

EXTRAS = \
	inlinegooglesearch.pc.in 

REFERENCES =  \
	Do.Addins \
	System \
	System.Web \
	System.Web.Services \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	$(GCONF_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

INLINEGOOGLESEARCH_PC = $(BUILD_DIR)/inlinegooglesearch.pc

$(eval $(call emit-deploy-wrapper,INLINEGOOGLESEARCH_PC,inlinegooglesearch.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
