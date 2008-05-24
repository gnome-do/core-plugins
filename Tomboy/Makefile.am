
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Tomboy.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

TOMBOY_ADDIN_XML_SOURCE=src/Tomboy.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Tomboy.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

TOMBOY_ADDIN_XML_SOURCE=src/Tomboy.addin.xml

endif

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Tomboy.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

TOMBOY_ADDIN_XML_SOURCE=src/Tomboy.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(TOMBOY_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(TOMBOY_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/TomboyDBus.cs \
	src/TomboyItem.cs \
	src/NewNoteAction.cs \
	src/NotesItemSource.cs \
	src/SearchNotesAction.cs 

DATA_FILES = \
	src/Tomboy.addin.xml 

RESOURCES = 

EXTRAS = \
	tomboy.pc.in 

REFERENCES =  \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	System \
	$(DOADDINS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

TOMBOY_ADDIN_XML = $(BUILD_DIR)/Tomboy.addin.xml
TOMBOY_PC = $(BUILD_DIR)/tomboy.pc

$(eval $(call emit-deploy-target,TOMBOY_ADDIN_XML))
$(eval $(call emit-deploy-wrapper,TOMBOY_PC,tomboy.pc))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
