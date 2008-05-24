
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/GNOME-Session.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GNOME_SESSION_ADDIN_XML_SOURCE=src/GNOME-Session.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GNOME-Session.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GNOME_SESSION_ADDIN_XML_SOURCE=src/GNOME-Session.addin.xml

endif

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/GNOME-Session.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GNOME_SESSION_ADDIN_XML_SOURCE=src/GNOME-Session.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GNOME_SESSION_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(GNOME_SESSION_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/PowerManagement.cs \
	src/ScreenSaver.cs \
	src/SessionCommandItem.cs \
	src/SessionCommandsItemSource.cs 

DATA_FILES = \
	src/GNOME-Session.addin.xml 

RESOURCES = 

EXTRAS = \
	gnome-session.pc.in 

REFERENCES =  \
	System \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(DOADDINS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GNOME_SESSION_PC = $(BUILD_DIR)/gnome-session.pc
GNOME_SESSION_ADDIN_XML = $(BUILD_DIR)/GNOME-Session.addin.xml

$(eval $(call emit-deploy-wrapper,GNOME_SESSION_PC,gnome-session.pc))
$(eval $(call emit-deploy-target,GNOME_SESSION_ADDIN_XML))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
