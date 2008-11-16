
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Launchpad.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

LAUNCHPAD_DLL_MDB_SOURCE=bin/Debug/Launchpad.dll.mdb
LAUNCHPAD_DLL_MDB=$(BUILD_DIR)/Launchpad.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Launchpad.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

LAUNCHPAD_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(LAUNCHPAD_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(LAUNCHPAD_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/AssemblyInfo.cs \
	src/LaunchpadAction.cs \
	src/LaunchpadAnswersItem.cs \
	src/LaunchpadBlueprintsItem.cs \
	src/LaunchpadBugsItem.cs \
	src/LaunchpadCodeItem.cs \
	src/LaunchpadIcons.cs \
	src/LaunchpadPersonItem.cs \
	src/LaunchpadProjectItem.cs \
	src/LaunchpadRegisterItem.cs \
	src/LaunchpadTranslationsItem.cs 

DATA_FILES = 

RESOURCES = \
	Resources/Launchpad.png \
	Resources/LaunchpadAnswers.png \
	Resources/LaunchpadBlueprints.png \
	Resources/LaunchpadBugs.png \
	Resources/LaunchpadCode.png \
	Resources/LaunchpadRegister.png \
	Resources/LaunchpadTranslations.png \
	Resources/LaunchpadUser.png \
	src/Launchpad.addin.xml 

EXTRAS = \
	../Launchpad.old/Resources/Launchpad.png \
	launchpad.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	Mono.Posix

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

LAUNCHPAD_PC = $(BUILD_DIR)/launchpad.pc

$(eval $(call emit-deploy-wrapper,LAUNCHPAD_PC,launchpad.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
