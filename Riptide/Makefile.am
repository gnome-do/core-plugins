
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Riptide.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

RIPTIDE_DLL_MDB_SOURCE=bin/Debug/Riptide.dll.mdb
RIPTIDE_DLL_MDB=$(BUILD_DIR)/Riptide.dll.mdb
MONOTORRENT_DLL_SOURCE=../BundledLibraries/MonoTorrent.dll

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Riptide.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

RIPTIDE_DLL_MDB=
MONOTORRENT_DLL_SOURCE=../BundledLibraries/MonoTorrent.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(RIPTIDE_DLL_MDB) \
	$(MONOTORRENT_DLL)  

LINUX_PKGCONFIG = \
	$(RIPTIDE_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/TorrentDownloadAction.cs \
	src/TorrentResultItem.cs \
	src/TorrentSearchAction.cs \
	src/AssemblyInfo.cs \
	gtk-gui/generated.cs \
	src/Configuration.cs \
	gtk-gui/Do.Riptide.Configuration.cs \
	src/TorrentStatusAction.cs \
	src/ActiveTorrentItem.cs \
	src/ITorrentItem.cs \
	src/WindowTorrentView.cs \
	src/TorrentDisplay.cs \
	gtk-gui/Do.Riptide.TorrentDisplay.cs \
	src/TorrentClientManager.cs \
	src/TorrentDownloadClientAction.cs \
	src/PauseTorrentAction.cs \
	src/ResumeTorrentAction.cs \
	src/StopTorrentAction.cs 

DATA_FILES = 

RESOURCES = \
	Riptide.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	../BundledLibraries/MonoTorrent.dll \
	riptide.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	System.Web \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	$(GCONF_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	Mono.Cairo

DLL_REFERENCES =  \
	../BundledLibraries/MonoTorrent.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

RIPTIDE_PC = $(BUILD_DIR)/riptide.pc
MONOTORRENT_DLL = $(BUILD_DIR)/MonoTorrent.dll

$(eval $(call emit-deploy-wrapper,RIPTIDE_PC,riptide.pc))
$(eval $(call emit-deploy-target,MONOTORRENT_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
