
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/ImageShack.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

IMAGESHACK_DLL_MDB_SOURCE=bin/Debug/ImageShack.dll.mdb
IMAGESHACK_DLL_MDB=$(BUILD_DIR)/ImageShack.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/ImageShack.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

IMAGESHACK_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(IMAGESHACK_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(IMAGESHACK_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/ImageShackAction.cs \
	gtk-gui/generated.cs \
	gtk-gui/ImageShack.ImageShackConfig.cs \
	src/ImageShackConfig.cs 

DATA_FILES = 

RESOURCES = \
	src/ImageShack.addin.xml \
	resources/imageshack.png \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	imageshack.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	Mono.Posix \
	System.Web \
	$(GTK_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

IMAGESHACK_PC = $(BUILD_DIR)/imageshack.pc

$(eval $(call emit-deploy-wrapper,IMAGESHACK_PC,imageshack.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
