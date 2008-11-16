
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/File.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

FILE_DLL_MDB_SOURCE=bin/Debug/File.dll.mdb
FILE_DLL_MDB=$(BUILD_DIR)/File.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/File.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

FILE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FILE_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(FILE_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/CreateEmptyFileAction.cs \
	src/CreateDirectory.cs \
	src/FileItemSource.cs \
	src/RecentFileItemSource.cs \
	src/FileCopyToAction.cs \
	src/FileRenameToAction.cs \
	src/FileMoveToAction.cs \
	src/FileMoveToTrashAction.cs \
	src/FileDeleteAction.cs \
	gtk-gui/generated.cs \
	src/Configuration.cs \
	gtk-gui/FilePlugin.Configuration.cs \
	src/PathNodeView.cs \
	src/CompareAction.cs \
	gtk-gui/FilePlugin.CompareConfig.cs \
	src/CompareConfig.cs 

DATA_FILES = 

RESOURCES = \
	src/File.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	file.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	$(GCONF_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

FILE_PC = $(BUILD_DIR)/file.pc

$(eval $(call emit-deploy-wrapper,FILE_PC,file.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
