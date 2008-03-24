
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Amarok.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

AMAROK_ADDIN_XML_SOURCE=src/Amarok.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Amarok.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

AMAROK_ADDIN_XML_SOURCE=src/Amarok.addin.xml

endif

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug -define:DEBUG
ASSEMBLY = ../buildrepo/Amarok.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../buildrepo/

AMAROK_ADDIN_XML_SOURCE=src/Amarok.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(AMAROK_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(AMAROK_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/Amarok.cs \
	src/AmarokEnqueueAction.cs \
	src/AmarokItems.cs \
	src/AmarokItemSource.cs \
	src/AmarokPlayAction.cs \
	src/MusicItems.cs \
	src/AssemblyInfo.cs 

DATA_FILES = \
	src/Amarok.addin.xml 

RESOURCES = 

EXTRAS = \
	amarok.pc.in 

REFERENCES =  \
	System \
	$(DOADDINS_LIBS) \
	System.Xml \
	System.Data \
	Mono.Data.Sqlite

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

AMAROK_PC = $(BUILD_DIR)/amarok.pc
AMAROK_ADDIN_XML = $(BUILD_DIR)/Amarok.addin.xml

$(eval $(call emit-deploy-wrapper,AMAROK_PC,amarok.pc))
$(eval $(call emit-deploy-target,AMAROK_ADDIN_XML))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
