
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/GNOME-Terminal.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GNOME_TERMINAL_DLL_MDB_SOURCE=bin/Debug/GNOME-Terminal.dll.mdb
GNOME_TERMINAL_DLL_MDB=$(BUILD_DIR)/GNOME-Terminal.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GNOME-Terminal.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GNOME_TERMINAL_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GNOME_TERMINAL_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(GNOME_TERMINAL_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/ProfileItem.cs \
	src/ProfileItemSource.cs \
	src/OpenTerminalHereAction.cs \
	src/OpenProfileAction.cs \
	src/RunInTerminalAction.cs 

DATA_FILES = 

RESOURCES = \
	GNOME-Terminal.addin.xml 

EXTRAS = \
	gnome-terminal.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	$(GCONF_SHARP_20_LIBS) \
	Mono.Posix

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GNOME_TERMINAL_PC = $(BUILD_DIR)/gnome-terminal.pc

$(eval $(call emit-deploy-wrapper,GNOME_TERMINAL_PC,gnome-terminal.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
