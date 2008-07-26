
EXTRA_DIST =  



if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Riptide.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Riptide.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

all: $(ASSEMBLY)

DATA_FILES = 

FILES = \
	gtk-gui/Do.Riptide.Configuration.cs \
	gtk-gui/Do.Riptide.TorrentDisplay.cs \
	gtk-gui/generated.cs \
	src/ActiveTorrentItem.cs \
	src/AssemblyInfo.cs \
	src/Configuration.cs \
	src/ITorrentItem.cs \
	src/PauseTorrentAction.cs \
	src/ResumeTorrentAction.cs \
	src/StopTorrentAction.cs \
	src/TorrentClientManager.cs \
	src/TorrentDisplay.cs \
	src/TorrentDownloadAction.cs \
	src/TorrentDownloadClientAction.cs \
	src/TorrentResultItem.cs \
	src/TorrentSearchAction.cs \
	src/TorrentStatusAction.cs \
	src/WindowTorrentView.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Riptide.addin.xml

REFERENCES =  \
	System \
	System.Xml \
	System.Web \
	Mono.Posix \
	Mono.Cairo \
	$(DOADDINS_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GCONF_SHARP_20_LIBS) \
	$(srcdir)/MonoTorrent.dll

DLL_REFERENCES = 

CLEANFILES = 

include $(top_srcdir)/Makefile.include


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
