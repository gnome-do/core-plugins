
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


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Pidgin.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

LINUX_PKGCONFIG = \
	$(PIDGIN_PC)  


	
all: $(ASSEMBLY) $(LINUX_PKGCONFIG) 

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
	src/Pidgin.addin.xml 

EXTRAS = \
	pidgin.pc.in 

REFERENCES =  \
	System \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(NDESK_DBUS_10_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	System.Xml \
	$(DOADDINS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

PIDGIN_PC = $(BUILD_DIR)/pidgin.pc

$(eval $(call emit-deploy-wrapper,PIDGIN_PC,pidgin.pc))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
