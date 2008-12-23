
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
	gtk-gui/Do.FilesAndFolders.Configuration.cs \
	gtk-gui/FilePlugin.Configuration.cs \
	gtk-gui/generated.cs \
	src/CompareConfig.cs \
	src/Do.FilesAndFolders/Configuration.cs \
	src/Do.FilesAndFolders/FileItemSource.cs \
	src/Do.FilesAndFolders/FolderIndex.cs \
	src/Do.FilesAndFolders/ImportantFolders.cs \
	src/Do.FilesAndFolders/IndexedFolder.cs \
	src/Do.FilesAndFolders/NewFolderAction.cs \
	src/Do.FilesAndFolders/Plugin.cs \
	src/Do.FilesAndFolders/Preferences.cs \
	src/Do.FilesAndFolders/RecentFileItemSource.cs \
	src/FileChooserParent.cs \
	src/FileCopyToAction.cs \
	src/FileDeleteAction.cs \
	src/FileMoveToAction.cs \
	src/FileMoveToTrashAction.cs \
	src/FileRenameToAction.cs \
	src/PathNodeView.cs

DATA_FILES = 

RESOURCES = \
	Resources/File.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	file.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
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
