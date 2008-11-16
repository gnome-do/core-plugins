
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Flickr.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

FLICKR_DLL_MDB_SOURCE=bin/Debug/Flickr.dll.mdb
FLICKR_DLL_MDB=$(BUILD_DIR)/Flickr.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Flickr.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

FLICKR_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FLICKR_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(FLICKR_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/UploadAction.cs \
	gtk-gui/generated.cs \
	src/AccountConfig.cs \
	src/FlickrItemSource.cs \
	gtk-gui/Flickr.AccountConfig.cs \
	gtk-gui/Flickr.UploadConfig.cs \
	src/UploadConfig.cs 

DATA_FILES = 

RESOURCES = \
	Flickr.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	resources/flickr.png 

EXTRAS = \
	flickr.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	$(FLICKRNET_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	$(GCONF_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

FLICKR_PC = $(BUILD_DIR)/flickr.pc

$(eval $(call emit-deploy-wrapper,FLICKR_PC,flickr.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
