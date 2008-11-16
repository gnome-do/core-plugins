
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/del.icio.us.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

DEL_ICIO_US_DLL_MDB_SOURCE=bin/Debug/del.icio.us.dll.mdb
DEL_ICIO_US_DLL_MDB=$(BUILD_DIR)/del.icio.us.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/del.icio.us.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

DEL_ICIO_US_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(DEL_ICIO_US_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(DEL_ICIO_US_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/SearchAction.cs \
	src/Delicious.cs \
	src/BookmarksItemSource.cs \
	src/TagsItemSource.cs \
	src/TagItem.cs \
	src/Configuration.cs \
	src/NewBookmarkAction.cs 

DATA_FILES = 

RESOURCES = \
	del.icio.us.addin.xml \
	resources/delicious.png 

EXTRAS = \
	del.icio.us.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	System.Xml \
	$(GCONF_SHARP_20_LIBS) \
	Mono.Posix \
	$(GTK_SHARP_20_LIBS) \
	System.Web

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

DEL_ICIO_US_PC = $(BUILD_DIR)/del.icio.us.pc

$(eval $(call emit-deploy-wrapper,DEL_ICIO_US_PC,del.icio.us.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
