
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Rhythmbox.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

RHYTHMBOX_DLL_MDB_SOURCE=bin/Debug/Rhythmbox.dll.mdb
RHYTHMBOX_DLL_MDB=$(BUILD_DIR)/Rhythmbox.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Rhythmbox.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

RHYTHMBOX_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(RHYTHMBOX_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(RHYTHMBOX_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/MusicItems.cs \
	src/Rhythmbox.cs \
	src/RhythmboxEnqueueAction.cs \
	src/RhythmboxItems.cs \
	src/RhythmboxPlayAction.cs \
	src/AssemblyInfo.cs \
	src/RhythmboxItemSource.cs 

DATA_FILES = 

RESOURCES = \
	src/Rhythmbox.addin.xml 

EXTRAS = \
	rhythmbox.pc.in 

REFERENCES =  \
	System.Xml \
	System \
	Do.Addins \
	Mono.Posix

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

RHYTHMBOX_PC = $(BUILD_DIR)/rhythmbox.pc

$(eval $(call emit-deploy-wrapper,RHYTHMBOX_PC,rhythmbox.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
