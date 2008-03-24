
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Banshee.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

BANSHEE_ADDIN_XML_SOURCE=src/Banshee.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Banshee.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

BANSHEE_ADDIN_XML_SOURCE=src/Banshee.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(BANSHEE_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(BANSHEE_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/AssemblyInfo.cs \
	src/Banshee.cs \
	src/BansheeAction.cs \
	src/BansheeDBus.cs \
	src/BansheeItems.cs \
	src/BansheeItemSource.cs 

DATA_FILES = \
	src/Banshee.addin.xml 

RESOURCES = 

EXTRAS = \
	banshee.pc.in 

REFERENCES =  \
	System \
	System.Xml \
	System.Data \
	Mono.Data.Sqlite \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(DOADDINS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

BANSHEE_PC = $(BUILD_DIR)/banshee.pc
BANSHEE_ADDIN_XML = $(BUILD_DIR)/Banshee.addin.xml

$(eval $(call emit-deploy-wrapper,BANSHEE_PC,banshee.pc))
$(eval $(call emit-deploy-target,BANSHEE_ADDIN_XML))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
