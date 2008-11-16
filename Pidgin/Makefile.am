
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Pidgin.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

PIDGIN_DLL_MDB_SOURCE=bin/Debug/Pidgin.dll.mdb
PIDGIN_DLL_MDB=$(BUILD_DIR)/Pidgin.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Pidgin.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

PIDGIN_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(PIDGIN_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(PIDGIN_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/AssemblyInfo.cs \
	src/Pidgin.cs \
	src/PidginChatAction.cs \
	src/PidginContactItemSource.cs \
	src/PidginHandleContactDetailItem.cs \
	src/PidginSavedStatusItemSource.cs \
	src/PidginSavedStatusItem.cs \
	src/PidginAccountItemSource.cs \
	src/PidginAccountItem.cs \
	src/PidginSetStatusAction.cs \
	src/PidginStatusTypeItem.cs \
	src/PidginAccountActions.cs 

DATA_FILES = 

RESOURCES = \
	src/Pidgin.addin.xml \
	Resources/internet-group-chat.svg 

EXTRAS = \
	pidgin.pc.in 

REFERENCES =  \
	System \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(NDESK_DBUS_10_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	System.Xml \
	Do.Addins \
	Mono.Posix

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

PIDGIN_PC = $(BUILD_DIR)/pidgin.pc

$(eval $(call emit-deploy-wrapper,PIDGIN_PC,pidgin.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
